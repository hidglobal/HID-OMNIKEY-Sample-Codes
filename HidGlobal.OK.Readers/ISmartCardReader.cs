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
using HidGlobal.OK.Readers.Components;

namespace HidGlobal.OK.Readers
{
    public interface ISmartCardReader : IDisposable
    {
        /// <summary>
        /// Reader name seen by Windows Smart Card Resource Manager.
        /// </summary>
        string PcscReaderName { get; }
        /// <summary>
        /// Smart card protocol used in current connection.
        /// </summary>
        Protocol ActiveProtocol { get; }
        /// <summary>
        /// Reader share mode used in current connection.s
        /// </summary>
        ReaderSharingMode ConnectionMode { get; }
        /// <summary>
        /// Connection status.
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// Establishes a connection between the calling application and a smart card.
        /// </summary>
        /// <param name="mode">Reader share mode to be used in current connection.</param>
        /// <param name="preferredProtocol">Smart card protocol to be used in current connection.</param>
        void Connect(ReaderSharingMode mode, Protocol preferredProtocol);
        /// <summary>
        /// Establishes a connection between the calling application and a smart card in Direct mode, 
        /// can be used to send control commands to reader without any smart card present.
        /// </summary>
        void ConnectDirect();
        /// <summary>
        /// Reestablishes an existing connection from the calling application to the smart card.
        /// </summary>
        /// <param name="initialization">Type of initialization that should be performed on the card.</param>
        /// <param name="mode">Reader share mode to be used in current connection.</param>
        /// <param name="protocol">Smart card protocol to be used in current connection.</param>
        void Reconnect(CardInitialization initialization, ReaderSharingMode mode, Protocol protocol);
        /// <summary>
        /// Terminates a connection between the calling application and a smart card.
        /// </summary>
        /// <param name="disposition">Action to take on the card in the connected reader on close.</param>
        void Disconnect(CardDisposition disposition = CardDisposition.Unpower);
        /// <summary>
        ///  The function waits for the completion of all other transactions before it begins. After the transaction starts, 
        /// all other applications are blocked from accessing the smart card while the transaction is in progress.
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// The SCardEndTransaction function completes a previously declared transaction, 
        /// allowing other applications to resume interactions with the card.
        /// </summary>
        /// <param name="disposition">Action to take on the card in the connected reader on close.</param>
        void EndTransaction(CardDisposition disposition);
        /// <summary>
        /// State of smart card in the reader.
        /// </summary>
        /// <returns></returns>
        ConnectionStatus CheckConnectionStatus();
        /// <summary>
        /// Gets direct control of the reader after Connect is called.
        /// </summary>
        /// <param name="controlCode"></param>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        IReadOnlyList<byte> Control(ReaderControlCode controlCode, IReadOnlyList<byte> dataBytes);
        /// <summary>
        /// Gets direct control of the reader after Connect is called.
        /// </summary>
        /// <param name="controlCode"></param>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        string Control(ReaderControlCode controlCode, string dataBytes);
        /// <summary>
        /// Sends a service request to a smart card.
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        IReadOnlyList<byte> Transmit(IReadOnlyList<byte> apdu);
        /// <summary>
        /// Sends a service request to a smart card.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<byte> Transmit(ref IoRequest sendPci, ref IoRequest recivePci, IReadOnlyList<byte> apdu);
        /// <summary>
        /// Sends a service request to a smart card.
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        string Transmit(string apdu);
        /// <summary>
        /// Sends a service request to a smart card.
        /// </summary>
        /// <returns></returns>
        string Transmit(ref IoRequest sendPci, ref IoRequest recivePci, string apdu);

        /// <summary>
        /// Gets the current reader's attributes from a given reader, driver, or smart card.
        /// </summary>
        /// <param name="attribiuteId">Identifier for the attribute to get.</param>
        /// <returns></returns>
        IReadOnlyList<byte> GetAttribiute(Attribiutes attribiuteId);
        /// <summary>
        /// Sets a given reader attribute.
        /// </summary>
        /// <param name="attribiuteId">Identifier for the attribute to set.</param>
        /// <param name="attribiute"></param>
        /// <returns></returns>
        void SetAttribiute(Attribiutes attribiuteId, IReadOnlyList<byte> attribiute);
    }
}
