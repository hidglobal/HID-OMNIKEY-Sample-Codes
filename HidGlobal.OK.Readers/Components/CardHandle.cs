/*****************************************************************************************
    (c) 2017 HID Global Corporation/ASSA ABLOY AB.  All rights reserved.

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
using System.Text;

namespace HidGlobal.OK.Readers.Components
{
    public class CardHandle : ICardHandle
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private IntPtr Handle { get; set; }

        public Encoding Encoding { get; set;}

        public Protocol ActiveProtocol { get; private set; }

        public ReaderSharingMode ActiveConnectionMode { get; private set; }

        public CardState ActiveCardState { get; private set; }

        public string ConnectedReaderName { get; private set; }

        public CardHandle()
        {
            Handle = IntPtr.Zero;
            ActiveProtocol = Protocol.None;
            ActiveConnectionMode = ReaderSharingMode.Exclusive;
            ActiveCardState = CardState.Unknown;
            ConnectedReaderName = string.Empty;
            Encoding = Encoding.ASCII;
        }

        public ErrorCodes Connect(IntPtr contextHandle, string readerName, ReaderSharingMode mode, Protocol preferredProtocol)
        {
            IntPtr handle = IntPtr.Zero;
            Protocol tempProtocol = preferredProtocol;
            
            var retCode = WinSCard.Connect(contextHandle, readerName, Encoding, mode, ref tempProtocol, out handle);
            if (retCode!= ErrorCodes.SCARD_S_SUCCESS)
            {
                log.Error(WinSCard.GetAttributeDescription(retCode));
                Handle = IntPtr.Zero;
                return retCode;
            }
            Handle = handle;
            ActiveProtocol = tempProtocol;
            ActiveConnectionMode = mode;
            ConnectedReaderName = readerName;
            return retCode;
        }

        public ErrorCodes ConnectDirect(IntPtr contextHandle, string readerName)
        {
            return Connect(contextHandle, readerName, ReaderSharingMode.Direct, Protocol.None);
        }

        public ErrorCodes ConnectExclusive(IntPtr contextHandle, string readerName, Protocol preferredProtocol)
        {
            return Connect(contextHandle, readerName, ReaderSharingMode.Exclusive, preferredProtocol);
        }

        public ErrorCodes ConnectShared(IntPtr contextHandle, string readerName, Protocol preferredProtocol)
        {
            return Connect(contextHandle, readerName, ReaderSharingMode.Shared, preferredProtocol);
        }

        public bool IsConnected()
        {
            return Handle != IntPtr.Zero;
        }

        public ErrorCodes Disconnect(CardDisposition disposeAction)
        {
            var retCode = WinSCard.Disconnect(Handle, disposeAction);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS) 
                return retCode;

            Handle = IntPtr.Zero;
            ActiveProtocol = Protocol.None;
            ConnectedReaderName = string.Empty;
            return retCode;
        }

        public ErrorCodes Disconnect()
        {
            return Disconnect(CardDisposition.Eject);
        }

        public ErrorCodes Reconnect(CardDisposition initialization, ReaderSharingMode mode, Protocol preferredProtocol)
        {
            Protocol activeProtocol = preferredProtocol;
            var retCode = WinSCard.Reconnect(Handle, mode, initialization, ref activeProtocol);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                return retCode;

            ActiveProtocol = activeProtocol;
            ActiveConnectionMode = mode;
            return retCode;
        }

        public ErrorCodes Control(ReaderControlCode controlCode, byte[] input, out byte[] output)
        {
            var retCode = WinSCard.Control(Handle, controlCode, input, out output);
            if (retCode!= ErrorCodes.SCARD_S_SUCCESS)
                log.Error(WinSCard.GetAttributeDescription(retCode));
            return retCode;
        }

        public ErrorCodes Transmit(byte[] input, out byte[] output)
        {
            var retCode = WinSCard.Transmit(Handle, ActiveProtocol, input, out output);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                log.Error(WinSCard.GetAttributeDescription(retCode));
            return retCode;
        }

        public ErrorCodes Status(out byte[] atr)
        {
            string name = string.Empty;
            CardState tempState = CardState.Unknown;
            Protocol tempProtocol = Protocol.None;
            var retCode = WinSCard.Status(Handle, out name, Encoding, out tempState, out tempProtocol, out atr);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
            {
                log.Error(WinSCard.GetAttributeDescription(retCode));
                atr = null;
                return retCode;
            }
            ActiveCardState = tempState;
            ActiveProtocol = tempProtocol;
            ConnectedReaderName = name;
            return retCode;
            
        }

        public ErrorCodes GetAttribiute(Attribiutes attribiuteId, out byte[] attributeData)
        {
            var tempData = new byte[0];
            var retCode = WinSCard.GetAttrib(Handle, attribiuteId, out tempData);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
            {
                log.Error(WinSCard.GetAttributeDescription(retCode));
                attributeData = null;
                return retCode;
            }
            attributeData = tempData;
            return retCode;
        }

        public ErrorCodes SetAttribiute(Attribiutes attribiuteId, byte[] attribute)
        {
            var retCode = WinSCard.SetAttrib(Handle, attribiuteId, attribute);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                log.Error(WinSCard.GetAttributeDescription(retCode));
            return retCode;
        }

        public ErrorCodes BeginTransaction()
        {
            var retCode = WinSCard.BeginTransaction(Handle);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                log.Error(WinSCard.GetAttributeDescription(retCode));
            return retCode;
        }

        public ErrorCodes EndTransaction(CardDisposition disposition)
        {
            var retCode = WinSCard.EndTransaction(Handle, disposition);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                log.Error(WinSCard.GetAttributeDescription(retCode));
            return retCode;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                if (IsConnected())
                {
                    Disconnect();
                }

                disposedValue = true;
            }
        }

        ~CardHandle()
        {
           Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
