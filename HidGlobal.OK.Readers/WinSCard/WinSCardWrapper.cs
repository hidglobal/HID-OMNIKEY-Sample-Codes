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
using System.Runtime.InteropServices;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.WinSCard
{
    internal static class WinSCardWrapper
    {
        private const string WinscardDll = "WinSCard.dll";
        private const int SuccessCode = 0x00000000;
        private const int InsufficientBufferCode = 0x0000007A;
        private const CharSet DefinedCharSet = CharSet.Ansi;
        private const bool DefinedSetLastError = true;
        
        #region P/Invoke

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardEstablishContext")]
        private static extern int SCardEstablishContext([In] int scope,
            [In] IntPtr rfu0,
            [In] IntPtr rfu1,
            [Out] out IntPtr contextHandle);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardIsValidContext")]
        private static extern int SCardIsValidContext([In] IntPtr contextHandle);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardReleaseContext")]
        private static extern int SCardReleaseContext([In] IntPtr contextHandle);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardConnect")]
        private static extern int SCardConnect([In] IntPtr contextHandle,
            [In] byte[] readerName,
            [In] int sharedMode,
            [In] int preferredProtocols,
            [Out] out IntPtr cardHandle,
            [Out] out int activeProtocol);


        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardDisconnect")]
        private static extern int SCardDisconnect([In] IntPtr connectionHandle, [In] int disposition);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardReconnect")]
        private static extern int SCardReconnect([In] IntPtr connectionHandle,
            [In] int sharedMode,
            [In] int preferredProtocols,
            [In] int initialization,
            [Out] out int activeProtocol);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardListReaders")]
        private static extern int SCardListReaders([In] IntPtr contextHandle,
            [In, Optional] byte[] groups,
            [Out] byte[] readerNames,
            [In, Out] ref int readers);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardListReaderGroups")]
        private static extern int SCardListReaderGroups([In] IntPtr contextHandle,
            [In, Out] ref byte[] groupNames,
            [In, Out] ref int groups);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardListReaderGroups")]
        private static extern int SCardListReaderGroupsBufferSize([In] IntPtr contextHandle,
            [In] byte[] groupNames,
            [In, Out] ref int groups);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardIntroduceReader")]
        private static extern int SCardIntroduceReader([In] IntPtr contextHandle,
            [In] byte[] readerName,
            [In] byte[] deviceName);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardForgetReader")]
        private static extern int SCardForgerReader([In] IntPtr contextHandle, [In] byte[] readerName);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardIntroduceReaderGroup")]
        private static extern int SCardIntroduceReaderGroup([In] IntPtr contextHandle, [In] byte[] groupName);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardAddReaderToGroup")]
        private static extern int SCardAddReaderToGroup([In] IntPtr contextHandle,
            [In] byte[] readerName,
            [In] byte[] groupName);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardRemoveReaderFromGroup")]
        private static extern int SCardRemoveReaderFromGroup([In] IntPtr contextHandle,
            [In] byte[] readerName,
            [In] byte[] groupName);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardForgetReaderGroup")]
        private static extern int SCardForgetReaderGroup([In] IntPtr contextHandle, [In] byte[] groupName);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardStatus")]
        private static extern int SCardStatus([In] IntPtr connectionHandle,
            [Out] byte[] readerName,
            [In, Out] ref int readerNameLength,
            [Out] out int state,
            [Out] out int protocol,
            [Out] byte[] atr,
            [In, Out] ref int atrLength);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardControl")]
        private static extern int SCardControl([In] IntPtr connectionHandle,
            [In] int controlCode,
            [In] byte[] inputBuffer,
            [In] int inputBufferSize,
            [Out] byte[] outputBuffer,
            [In] int outputBufferSize,
            [Out] out int bytesReturned);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardTransmit")]
        private static extern int SCardTransmit([In] IntPtr cardHandle,
            [In, Out] ref IoRequest sendPci,
            [In] byte[] inputBuffer,
            [In] int sendLength,
            [In, Out] ref IoRequest recvPci,
            [Out] byte[] outputBuffer,
            [In, Out] ref int recvLength);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardGetAttrib")]
        private static extern int SCardGetAttrib([In] IntPtr cardHandle,
            [In] int attributeId,
            [Out] byte[] attributeData,
            [In, Out] ref int attributeDataLength);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardSetAttrib")]
        private static extern int SCardSetAttrib([In] IntPtr cardHandle,
            [In] int attributeId,
            [In] byte[] attributeData,
            [In] int attributeDataLen);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardCancel")]
        private static extern int SCardCancel([In] IntPtr contextHandle);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardBeginTransaction")]
        private static extern int SCardBeginTransaction([In] IntPtr cardHandle);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardEndTransaction")]
        public static extern int SCardEndTransaction([In] IntPtr cardHandle, [In] int disposition);

        [DllImport(WinscardDll, CharSet = DefinedCharSet, SetLastError = DefinedSetLastError, EntryPoint = "SCardGetStatusChangeA")]
        private static extern int SCardGetStatusChange([In] IntPtr contextHandle,
            [In] int timeout,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ReaderState[] readerStates,
            [In] int size);

        #endregion

        #region Wrapper

        public static IntPtr EstablishContext(Scope scope)
        {
            var retCode = SCardEstablishContext((int)scope, IntPtr.Zero, IntPtr.Zero, out var context);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardEstablishContext");

            return context;
        }

        public static void IsValidContext(IntPtr contextHandle)
        {
            var retCode = SCardIsValidContext(contextHandle);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardIsValidContext");
        }

        public static void ReleaseContext(IntPtr contexHandle)
        {
            var retCode = SCardReleaseContext(contexHandle);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardReleaseContext");
        }

        public struct ConnectResult
        {
            public IntPtr ConnectionHandle;
            public Protocol Protocol;
        }

        public static ConnectResult Connect(IntPtr contextHandle, byte[] readerNameBytes, ReaderSharingMode readerSharingMode, Protocol protocol)
        {
            var retCode = SCardConnect(contextHandle, readerNameBytes, (int)readerSharingMode, (int)protocol, out var tempHandle, out var activeProtocol);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardConnect");
            
            return new ConnectResult { ConnectionHandle = tempHandle, Protocol = (Protocol)activeProtocol };
        }

        public static void Disconnect(IntPtr cardHandle, CardDisposition disposition)
        {
            var retCode = SCardDisconnect(cardHandle, (int)disposition);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardDisconnect");
        }

        public static ConnectResult Reconnect(IntPtr connectionHandle, ReaderSharingMode readerSharingMode, CardInitialization initialization, Protocol protocol)
        {
            var retCode = SCardReconnect(connectionHandle, (int)readerSharingMode, (int)protocol, (int)initialization, out var activeProtocol);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardReconnect");
            
            return new ConnectResult { ConnectionHandle = connectionHandle, Protocol = (Protocol)activeProtocol };
        }

        private static int GetReaderListBufferSize(IntPtr contextHandle, byte[] groups)
        {
            var bufferSize = 0;

            var retCode = SCardListReaders(contextHandle, groups, null, ref bufferSize);

            if (SuccessCode != retCode)
                throw
                    ExceptionHelper.PrepareException(retCode, "SCardListReaders");

            return bufferSize;
        }

        public static byte[] ListReaders(IntPtr contextHandle, byte[] groups)
        {
            var bufferSize = GetReaderListBufferSize(contextHandle, groups);
            
            var buffer = new byte[bufferSize];
            
            var retCode = SCardListReaders(contextHandle, groups, buffer, ref bufferSize);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardListReaders");
            
            return buffer;
        }

        private static int GetReaderGroupListBufferSize(IntPtr contextHandle)
        {
            var bufferSize = 0;

            var retCode = SCardListReaderGroupsBufferSize(contextHandle, null, ref bufferSize);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardListReaderGroups");

            return bufferSize;
        }

        public static byte[] ListReaderGroups(IntPtr contextHandle)
        {
            var bufferSize = GetReaderGroupListBufferSize(contextHandle);

            var buffer = new byte[bufferSize];

            var retCode = SCardListReaderGroups(contextHandle, ref buffer, ref bufferSize);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardListReaderGroups");
            
            return buffer;
        }

        public static void IntroduceReader(IntPtr contextHandle, byte[] readerName, byte[] deviceName)
        {
            if (readerName == null)
                throw new ArgumentNullException(nameof(readerName));

            if (deviceName == null)
                throw new ArgumentNullException(nameof(deviceName));
            
            var retCode = SCardIntroduceReader(contextHandle, readerName, deviceName);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardIntroduceReader");
        }

        public static void ForgetReader(IntPtr contextHandle, byte[] readerName)
        {
            if (readerName == null)
                throw new ArgumentNullException(nameof(readerName));

            var retCode = SCardForgerReader(contextHandle, readerName);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardForgerReader");
        }

        public static void IntroduceReaderGroup(IntPtr contextHandle, byte[] groupName)
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            var retCode = SCardIntroduceReaderGroup(contextHandle, groupName);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardIntroduceReaderGroup");
        }

        public static void AddReaderToGroup(IntPtr contextHandle, byte[] readerName, byte[] groupName)
        {
            if (readerName == null)
                throw new ArgumentNullException(nameof(readerName));

            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            var retCode = SCardAddReaderToGroup(contextHandle, readerName, groupName);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardAddReaderToGroup");
        }
        
        public static void RemoveReaderFromGroup(IntPtr contextHandle, byte[] readerName, byte[] groupName)
        {
            if (readerName == null)
                throw new ArgumentNullException(nameof(readerName));

            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            var retCode = SCardRemoveReaderFromGroup(contextHandle, readerName, groupName);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardRemoveReaderFromGroup");
        }
        
        public static void ForgetReaderGroup(IntPtr contextHandle, byte[] groupName)
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            var retCode = SCardForgetReaderGroup(contextHandle, groupName);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardForgetReaderGroup");
        }

        public struct ConnectionStatus
        {
            public IReadOnlyList<byte> ConnectedReaderNames;
            public CardState Status;
            public Protocol ActiveProtocol;
            public IReadOnlyList<byte> CardAnswerToReset;
        }

        private struct BuffersData
        {
            public int ReaderNamesBufferLength;
            public int AnswerToResetBufferLength;
        }

        private static BuffersData GetBuffersLength(IntPtr connectionHandle)
        {
            var readerNamesBufferSize = 0;
            var answerToResetBufferSize = 0;

            // Expecting to get ERROR_INSUFFICIENT_BUFFER

            var retCode = SCardStatus(connectionHandle, null, ref readerNamesBufferSize, out var _, out var _, null, ref answerToResetBufferSize);

            if (InsufficientBufferCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardStatus");
            
            return new BuffersData
            {
                ReaderNamesBufferLength = readerNamesBufferSize,
                AnswerToResetBufferLength = answerToResetBufferSize
            };
        }

        public static ConnectionStatus GetConnectionStatus(IntPtr connectionHandle)
        {
            var buffersData = GetBuffersLength(connectionHandle);

            var readerNamesBytes = new byte[buffersData.ReaderNamesBufferLength];
            var answerToResetBytes = new byte[buffersData.AnswerToResetBufferLength];

            var retCode = SCardStatus(connectionHandle, readerNamesBytes, ref buffersData.ReaderNamesBufferLength, out var cardState, out var activeProtocol, answerToResetBytes, ref buffersData.AnswerToResetBufferLength);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardStatus");
            
            return new ConnectionStatus
            {
                ActiveProtocol = (Protocol) activeProtocol,
                CardAnswerToReset = answerToResetBytes,
                ConnectedReaderNames = readerNamesBytes,
                Status = (CardState) cardState
            };
        }

        public static void Control(IntPtr connectionHandle, ReaderControlCode controlCode, byte[] inputData, ref byte[] buffer, out int dataSize)
        {
            var retCode = SCardControl(connectionHandle, (int)controlCode, inputData, inputData.Length, buffer, buffer.Length, out dataSize);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardControl");
        }

        public static void Transmit(IntPtr connectionHandle, Protocol activeProtocol, byte[] inputData, ref byte[] outputData, out int dataSize)
        {
            var inputPci = new IoRequest {Protocol = (int) activeProtocol};
            var outputPci = new IoRequest {Protocol = (int) activeProtocol};

            inputPci.PciLength = Marshal.SizeOf(inputPci);
            outputPci.PciLength = Marshal.SizeOf(outputPci);

            Transmit(connectionHandle, ref inputPci, ref outputPci, inputData, ref outputData, out dataSize);
        }

        public static void Transmit(IntPtr connectionHandle, ref IoRequest inputPci, ref IoRequest outputPci, byte[] inputData, ref byte[] outputData, out int dataSize)
        {
            var outputBufferSize = outputData.Length;
            var retCode = SCardTransmit(connectionHandle, ref inputPci, inputData, inputData.Length, ref outputPci, outputData, ref outputBufferSize);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardTransmit");
            
            dataSize = outputBufferSize;
        }

        public static void GetAttrib(IntPtr connectionHandle, Attribiutes attributeId, ref byte[] dataBuffer, out int dataSize)
        {
            var dataBufferSize = dataBuffer.Length;
            var retCode = SCardGetAttrib(connectionHandle, (int)attributeId, dataBuffer, ref dataBufferSize);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardGetAttrib");
            
            dataSize = dataBufferSize;
        }

        public static void SetAttrib(IntPtr connectionHandle, Attribiutes attributeId, byte[] attributeData)
        {
            var retCode = SCardSetAttrib(connectionHandle, (int)attributeId, attributeData, attributeData.Length);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardSetAttrib");
        }

        public static void Cancel(IntPtr contextHandle)
        {
            var retCode = SCardCancel(contextHandle);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardCancel");
        }

        public static void BeginTransaction(IntPtr connectionHandle)
        {
            var retCode = SCardBeginTransaction(connectionHandle);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardBeginTransaction");
        }

        public static void EndTransaction(IntPtr connectionHandle, CardDisposition disposition)
        {
            var retCode = SCardEndTransaction(connectionHandle, (int) disposition);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardEndTransaction");
        }
        public static void GetStatusChange(IntPtr contextHandle, int timeout, ref ReaderState[] readerStates, int numberOfElements)
        {
            var retCode = SCardGetStatusChange(contextHandle, timeout, readerStates, numberOfElements);

            if (SuccessCode != retCode) throw ExceptionHelper.PrepareException(retCode, "SCardGetStatusChangeA");
        }

        #endregion
    }
}
