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

namespace HidGlobal.OK.Readers.Components
{
    public interface ICardHandle : IDisposable
    {
        ErrorCodes Connect(IntPtr contextHandle, string readerName, ReaderSharingMode mode, Protocol preferredProtocol);
        ErrorCodes ConnectDirect(IntPtr contextHandle, string readerName);
        ErrorCodes ConnectExclusive(IntPtr contextHandle, string readerName, Protocol preferredProtocol);
        ErrorCodes ConnectShared(IntPtr contextHandle, string readerName, Protocol preferredProtocol);
        string ConnectedReaderName { get; }
        Protocol ActiveProtocol { get; }
        ReaderSharingMode ActiveConnectionMode { get; }
        bool IsConnected();
        ErrorCodes Disconnect();
        ErrorCodes Disconnect(CardDisposition disposeAction);
        ErrorCodes Reconnect(CardDisposition initialization, ReaderSharingMode mode, Protocol preferredProtocol);      
        ErrorCodes Control(ReaderControlCode controlCode, byte[] apdu, out byte[] response);
        ErrorCodes Transmit(byte[] apdu, out byte[] response);
        ErrorCodes Status(out byte[] atr);

        ErrorCodes GetAttribiute(Attribiutes attribiuteId, out byte[] attributeData);
        ErrorCodes SetAttribiute(Attribiutes attribiuteId, byte[] attribute);
        ErrorCodes BeginTransaction();
        ErrorCodes EndTransaction(CardDisposition disposition);
    }
}
