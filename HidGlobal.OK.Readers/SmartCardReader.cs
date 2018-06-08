/*****************************************************************************************
    (c) 2017-2018 HID Global Corporation/ASSA ABLOY AB.  All rights reserved.

      Redistribution and use in source and binary forms, with or without modification,
      are permitted provided that the following conditions are met:
         - Redistributions of source code must retain the above copyright notice,
           this list of conditions and the following disclaimer.
         - Redistributions in binary form must reproduce the above copyright notice,
           this list of conditions and the following disclaimer in the documentation
           and/or other materials provided with the distribution.
           THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
           AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
           THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
           ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
           FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
           (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
           LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
           ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
           (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
           THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.Utilities;
using HidGlobal.OK.Readers.WinSCard;


namespace HidGlobal.OK.Readers
{
    public class SmartCardReader : ISmartCardReader
    {
        private byte[] _buffer;
        private IntPtr Handle { get; set; }
        public Encoding Encoding { get; set; }
        public Protocol ActiveProtocol { get; private set; }
        public ReaderSharingMode ConnectionMode { get; private set; }
        public CardState ActiveCardState { get; private set; }
        public string PcscReaderName { get; }
        public bool IsConnected => Handle != IntPtr.Zero;

        public SmartCardReader(string readerName)
        {
            PcscReaderName = readerName;
            Handle = IntPtr.Zero;
            ActiveProtocol = Protocol.None;
            ConnectionMode = ReaderSharingMode.NotSet;
            ActiveCardState = CardState.Unknown;
            Encoding = Encoding.ASCII;
            _buffer = new byte[ushort.MaxValue + 9];
            _disposedValue = false;

            // maximum response size for extended apdu = ushort.MaxValue + 3
            // maximum command size foe extended apdu = ushort.MaxValue + 9
        }

        public void Connect(ReaderSharingMode mode, Protocol preferredProtocol)
        {
            var readerNameBytes = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, PcscReaderName);
            var connectResult = WinSCardWrapper.Connect(ContextHandler.Instance.Handle, readerNameBytes.ToArray(), mode, preferredProtocol);

            Handle = connectResult.ConnectionHandle;
            ActiveProtocol = connectResult.Protocol;
            ConnectionMode = mode;
        }

        public void ConnectDirect()
        {
            Connect(ReaderSharingMode.Direct, Protocol.None);
        }

        public void Disconnect(CardDisposition disposeAction)
        {
            WinSCardWrapper.Disconnect(Handle, disposeAction);

            Handle = IntPtr.Zero;
            ActiveProtocol = Protocol.None;
            ConnectionMode = ReaderSharingMode.NotSet;
            ActiveCardState = CardState.Unknown;
        }

        public void Reconnect(CardInitialization initialization, ReaderSharingMode mode, Protocol preferredProtocol)
        {
            var connectResult = WinSCardWrapper.Reconnect(Handle, mode, initialization, preferredProtocol);

            Handle = connectResult.ConnectionHandle;
            ActiveProtocol = connectResult.Protocol;
            ConnectionMode = mode;
        }

        public IReadOnlyList<byte> Control(ReaderControlCode controlCode, IReadOnlyList<byte> dataBytes)
        {
            WinSCardWrapper.Control(Handle, controlCode, dataBytes.ToArray(), ref _buffer, out var dataSize);

            return _buffer.Take(dataSize).ToArray();
        }

        public string Control(ReaderControlCode controlCode, string dataBytes)
        {
            var response = Control(controlCode, BinaryHelper.ConvertOctetStringToBytes(dataBytes)).ToArray();

            return BinaryHelper.ConvertBytesToOctetString(response);
        }

        public IReadOnlyList<byte> Transmit(IReadOnlyList<byte> apdu)
        {
            WinSCardWrapper.Transmit(Handle, ActiveProtocol, apdu.ToArray(), ref _buffer, out var dataSize);

            return _buffer.Take(dataSize).ToArray();
        }

        public string Transmit(string apdu)
        {
            var response = Transmit(BinaryHelper.ConvertOctetStringToBytes(apdu)).ToArray();

            return BinaryHelper.ConvertBytesToOctetString(response);
        }

        public IReadOnlyList<byte> Transmit(ref IoRequest sendPci, ref IoRequest recivePci, IReadOnlyList<byte> apdu)
        {
            WinSCardWrapper.Transmit(Handle, ref sendPci, ref recivePci, apdu.ToArray(), ref _buffer, out var dataSize);

            return _buffer.Take(dataSize).ToArray();
        }

        public string Transmit(ref IoRequest sendPci, ref IoRequest recivePci, string apdu)
        {
            var response = Transmit(ref sendPci, ref recivePci, BinaryHelper.ConvertOctetStringToBytes(apdu)).ToArray();

            return BinaryHelper.ConvertBytesToOctetString(response);
        }

        public ConnectionStatus CheckConnectionStatus()
        {
            var dataConnectionStatus = WinSCardWrapper.GetConnectionStatus(Handle);

            ActiveProtocol = dataConnectionStatus.ActiveProtocol;
            ActiveCardState = dataConnectionStatus.Status;

            return new ConnectionStatus
            {
                ActiveProtocol = dataConnectionStatus.ActiveProtocol,
                CardAnswerToReset = dataConnectionStatus.CardAnswerToReset,
                ReaderAliases =
                    (IReadOnlyList<string>)BinaryHelper.ConvertMultiNullTerminatedStringFromBytesToStringArray(
                        Encoding, dataConnectionStatus.ConnectedReaderNames),
                Status = dataConnectionStatus.Status
            };
        }

        public IReadOnlyList<byte> GetAttribiute(Attribiutes attribiuteId)
        {
            WinSCardWrapper.GetAttrib(Handle, attribiuteId, ref _buffer, out var dataSize);

            return (IReadOnlyList<byte>)_buffer.Take(dataSize);
        }

        public void SetAttribiute(Attribiutes attribiuteId, IReadOnlyList<byte> attribute)
        {
            WinSCardWrapper.SetAttrib(Handle, attribiuteId, attribute.ToArray());
        }

        public void BeginTransaction()
        {
            WinSCardWrapper.BeginTransaction(Handle);
        }

        public void EndTransaction(CardDisposition disposition)
        {
            WinSCardWrapper.EndTransaction(Handle, disposition);
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                if (IsConnected)
                {
                    Disconnect(CardDisposition.Unpower);
                }
            }

            _buffer = null;
            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
