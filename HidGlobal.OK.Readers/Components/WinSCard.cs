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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace HidGlobal.OK.Readers.Components
{
    /// <summary>
    /// Wrapper to native WinSCard.dll functions.
    /// </summary>
    internal static class WinSCard
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int _maxBufferSize = 0x10000;

        #region Utilities
        public static string GetAttributeDescription<T>(T attribiute)
        {
            var fieldinformation = attribiute.GetType().GetField(attribiute.ToString());
            try
            {
                var fieldAttribiutes = (DescriptionAttribute[])fieldinformation.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (fieldAttribiutes.Length > 0) ? fieldAttribiutes[0].Description : attribiute.ToString();
            }
            catch
            {
                return "Code: " + attribiute.ToString();
            }
        }
        #endregion
        #region Data Conversion
        public static byte[] ConvertData(string data, System.Text.Encoding characterEncoding)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            var convertedItem = new List<byte>();
            convertedItem.AddRange(characterEncoding.GetBytes(data));
            // terminate with null
            convertedItem.Add(0);
            return convertedItem.ToArray();
        }

        public static byte[] ConvertData(string[] data, System.Text.Encoding characterEncoding)
        {
            if (data == null)
                return null;

            var convertedData = new List<byte>();

            foreach (string item in data)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                var convertedItem = characterEncoding.GetBytes(item);
                convertedData.AddRange(convertedItem);
                // Terminate with null
                convertedData.Add(0);
            }
            // Terminate with null
            convertedData.Add(0);

            return convertedData.ToArray();
        }

        public static string[] ConvertData(byte[] data, System.Text.Encoding characterEncoding)
        {
            if (data == null)
                return null;

            var multiNullTerminatedString = characterEncoding.GetString(data);
            string[] convertedData = multiNullTerminatedString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            return convertedData;
        }

        public static string ConvertData(byte[] data, System.Text.Encoding characterEncoding, int stringLength = 0)
        {
            if (data == null)
                return null;

            int count = data.Length;

            if ((stringLength > 0) && (data.Length > stringLength))
                count = stringLength;

            return characterEncoding.GetString(data, 0, count);
        }
        #endregion
        #region Establish Context
        public static ErrorCodes EstablishContext(Scope scope, out IntPtr contextHandle)
        {
            var context = IntPtr.Zero;
            ErrorCodes retCode = 0;
            try
            {
                retCode = (ErrorCodes)SCardEstablishContext((int)scope, IntPtr.Zero, IntPtr.Zero, out context);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error($"Error Code: {retCode}\n{GetAttributeDescription(retCode)}");
                    context = IntPtr.Zero;
                }
                contextHandle = context;
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                contextHandle = IntPtr.Zero;
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardEstablishContext", CharSet = CharSet.Ansi)]
        private static extern int SCardEstablishContext([In] int scope, 
                                                        [In] IntPtr rfu0, 
                                                        [In] IntPtr rfu1, 
                                                        [Out] out IntPtr contextHandle);
        #endregion
        #region Is Valid Context
        public static ErrorCodes IsValidContext(IntPtr contextHandle)
        {
            ErrorCodes retCode = 0;
            try
            {
                retCode = (ErrorCodes)SCardIsValidContext(contextHandle);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));

                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardIsValidContext", CharSet = CharSet.Ansi)]
        private static extern int SCardIsValidContext([In] IntPtr contextHandle);
        #endregion
        #region Release Context
        public static ErrorCodes ReleaseContext(IntPtr contexHandle)
        {
            ErrorCodes retCode = 0;
            try
            {
                retCode = (ErrorCodes)SCardReleaseContext(contexHandle);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS) 
                    log.Error(GetAttributeDescription(retCode));
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardReleaseContext", CharSet = CharSet.Ansi)]
        private static extern int SCardReleaseContext([In] IntPtr contextHandle);
        #endregion
        #region Connect
        public static ErrorCodes Connect(IntPtr contextHandle, string readerName, System.Text.Encoding characterEncoding, ReaderSharingMode readerSharingMode, ref Protocol protocol, out IntPtr cardHandle)
        {
            ErrorCodes retCode = 0;
            var tempHandle = IntPtr.Zero;
            int tempProtocol1 = (int)protocol;
            int tempProtocol2 = (int)protocol;

            byte[] reader = ConvertData(readerName, characterEncoding);
            try
            {
                retCode = (ErrorCodes)SCardConnect(contextHandle, reader, (int)readerSharingMode, tempProtocol1, out tempHandle, out tempProtocol2);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    cardHandle = IntPtr.Zero;
                    protocol = Protocol.None;
                }
                else
                {
                    cardHandle = tempHandle;
                    protocol = (Protocol)tempProtocol2;
                }
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                protocol = Protocol.None;
                cardHandle = IntPtr.Zero;
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardConnect", CharSet = CharSet.Ansi)]
        private static extern int SCardConnect([In] IntPtr contextHandle, 
                                               [In] byte[] readerName, 
                                               [In] int sharedMode, 
                                               [In] int preferredProtocols, 
                                               [Out] out IntPtr cardHandle, 
                                               [Out] out int activeProtocol);
        #endregion
        #region Disconnect
        public static ErrorCodes Disconnect(IntPtr cardHandle, CardDisposition disposition)
        {
            ErrorCodes retCode = 0;
            try
            {
                retCode = (ErrorCodes)SCardDisconnect(cardHandle, (int)disposition);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));
                else
                    cardHandle = IntPtr.Zero;
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }
        [DllImport("WinSCard.dll", EntryPoint = "SCardDisconnect", CharSet = CharSet.Ansi)]
        private static extern int SCardDisconnect([In] IntPtr cardHandle, [In] int disposition);
        #endregion
        #region Reconnect
        public static ErrorCodes Reconnect(IntPtr cardHandle, ReaderSharingMode readerSharingMode, CardDisposition initialization, ref Protocol protocol)
        {
            ErrorCodes retCode = 0;
            int tempProtocol1 = (int)protocol;
            int tempProtocol2 = (int)protocol;

            try
            {
                retCode = (ErrorCodes)SCardReconnect(cardHandle, (int)readerSharingMode, tempProtocol1, (int)initialization, out tempProtocol2);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    protocol = Protocol.None;
                else
                    protocol = (Protocol)tempProtocol2;
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }

        }
        [DllImport("WinSCard.dll", EntryPoint = "SCardReconnect", CharSet = CharSet.Ansi)]
        private static extern int SCardReconnect([In] IntPtr cardHandle,
                                                 [In] int sharedMode,
                                                 [In] int preferredProtocols,
                                                 [In] int initialization,
                                                 [Out, Optional] out int activeProtocol);
        #endregion
        #region List Readers
        public static ErrorCodes ListReaders(IntPtr contextHandle, string[] groups, System.Text.Encoding characterEncoding, out string[] readerNames)
        {
            ErrorCodes retCode = 0;
            // If groups is set to null, selectet groups will be null
            byte[] selectedGroups = ConvertData(groups, characterEncoding);
            
            try
            {
                // Get buffer size
                int bufferSize = 0;

                retCode = (ErrorCodes)SCardListReaders(contextHandle, selectedGroups, null, ref bufferSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    readerNames = null;
                    return retCode;
                }
                // Set buffer
                var buffer = new byte[bufferSize];
                // Get reader names
                retCode = (ErrorCodes)SCardListReaders(contextHandle, selectedGroups, buffer, ref bufferSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    readerNames = null;
                    return retCode;
                }

                readerNames = ConvertData(buffer, characterEncoding);
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                readerNames = null;
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardListReaders", CharSet = CharSet.Ansi)]
        private static extern int SCardListReaders([In] IntPtr contextHandle, 
                                                   [In, Optional] byte[] groups, 
                                                   [Out] byte[] readerNames, 
                                                   [In, Out] ref int readers);
        #endregion
        #region List Groups
        public static ErrorCodes ListReaderGroups(IntPtr contextHandle, System.Text.Encoding characterEncoding, out string[] groupNames)
        {
            ErrorCodes retCode = 0;
            try
            {
                int bufferSize = 0;
                retCode = (ErrorCodes)SCardListReaderGroups(contextHandle, null, ref bufferSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS) 
                {
                    log.Error(GetAttributeDescription(retCode));
                    groupNames = null;
                    return retCode;
                }

                var buffer = new byte[bufferSize];

                retCode = (ErrorCodes)SCardListReaderGroups(contextHandle, buffer, ref bufferSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    groupNames = null;
                    return retCode;
                }

                groupNames = ConvertData(buffer, characterEncoding);
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                groupNames = null;
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardListReaderGroups", CharSet = CharSet.Ansi)]
        private static extern int SCardListReaderGroups([In] IntPtr contextHandle, 
                                                        [Out] byte[] groupNames, 
                                                        [In, Out] ref int groups);
        #endregion
        #region IntroduceReader
        public static ErrorCodes IntroduceReader(IntPtr contextHandle, string readerName, string deviceName, System.Text.Encoding characterEncoding)
        {
            ErrorCodes retCode = 0;
            if (string.IsNullOrWhiteSpace(readerName) || string.IsNullOrWhiteSpace(deviceName))
            {
                log.Warn("Unable to invoke SCardIntroduceReader with null or empty device/reader name parametr");
                return ErrorCodes.SCARD_E_INVALID_PARAMETER;
            }
            byte[] reader = ConvertData(readerName, characterEncoding);
            byte[] device = ConvertData(deviceName, characterEncoding);
            try
            {
                retCode = (ErrorCodes)SCardIntroduceReader(contextHandle, reader, device);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));

                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }   
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardIntroduceReader", CharSet = CharSet.Ansi)]
        private static extern int SCardIntroduceReader([In] IntPtr contextHandle, 
                                                       [In] byte[] readerName, 
                                                       [In] byte[] deviceName);
        #endregion
        #region Forget Reader
        public static ErrorCodes ForgetReader(IntPtr contextHandle, string readerName, System.Text.Encoding characterEncoding)
        {
            ErrorCodes retCode = 0;
            if (string.IsNullOrWhiteSpace(readerName))
            {
                log.Warn("Unable to invoke SCardForgetReader with null or empty reader name parametr");
                return ErrorCodes.SCARD_E_INVALID_PARAMETER;
            }

            byte[] reader = ConvertData(readerName, characterEncoding);

            try
            {
                retCode = (ErrorCodes)SCardForgerReader(contextHandle, reader);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS) 
                    log.Error(GetAttributeDescription(retCode));
                return retCode; 
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardForgetReader", CharSet = CharSet.Ansi)]
        private static extern int SCardForgerReader([In] IntPtr contextHandle, [In] byte[] readerName);
        #endregion
        #region Introduce Reader Group
        public static ErrorCodes IntroduceReaderGroup(IntPtr contextHandle, string groupName, System.Text.Encoding characterEncoding)
        {
            ErrorCodes retCode = 0;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                log.Warn("Unable to invoke SCardIntroduceReaderGroup with null or empty group name parametr");
                return ErrorCodes.SCARD_E_INVALID_PARAMETER;
            }

            byte[] group = ConvertData(groupName, characterEncoding);

            try
            {
                retCode = (ErrorCodes)SCardIntroduceReaderGroup(contextHandle, group);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardIntroduceReaderGroup", CharSet = CharSet.Ansi)]
        private static extern int SCardIntroduceReaderGroup([In] IntPtr contextHandle, [In] byte[] groupName);
        #endregion
        #region Add Reader to Group
        public static ErrorCodes AddReaderToGroup(IntPtr contextHandle, string readerName, string groupName, System.Text.Encoding characterEncoding)
        {
            ErrorCodes retCode = 0;
            if (string.IsNullOrWhiteSpace(readerName) || string.IsNullOrWhiteSpace(groupName))
            {
                log.Warn("Unable to invoke SCardAddReaderToGroup with null or empty group/reader name parametr");
                return ErrorCodes.SCARD_E_INVALID_PARAMETER;
            }

            byte[] reader = ConvertData(readerName, characterEncoding);
            byte[] group = ConvertData(groupName, characterEncoding);
            try
            {
                retCode = (ErrorCodes)SCardAddReaderToGroup(contextHandle, reader, group);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));

                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }
        
        [DllImport("WinSCard.dll", EntryPoint = "SCardAddReaderToGroup", CharSet = CharSet.Ansi)]
        private static extern int SCardAddReaderToGroup([In] IntPtr contextHandle, 
                                                        [In] byte[] readerName, 
                                                        [In] byte[] groupName);
        #endregion
        #region Remove Reader From Group
        public static ErrorCodes RemoveReaderFromGroup(IntPtr contextHandle, string readerName, string groupName, System.Text.Encoding characterEncoding)
        {
            ErrorCodes retCode = 0;
            if (string.IsNullOrWhiteSpace(readerName) || string.IsNullOrWhiteSpace(groupName))
            {
                log.Warn("Unable to invoke SCardAddReaderToGroup with null or empty group/reader name parametr");
                return ErrorCodes.SCARD_E_INVALID_PARAMETER;
            }

            byte[] reader = ConvertData(readerName, characterEncoding);
            byte[] group = ConvertData(groupName, characterEncoding);

            try
            {
                retCode = (ErrorCodes)SCardRemoveReaderFromGroup(contextHandle, reader, group);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));

                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardRemoveReaderFromGroup", CharSet = CharSet.Ansi)]
        private static extern int SCardRemoveReaderFromGroup([In] IntPtr contextHandle, 
                                                             [In] byte[] readerName, 
                                                             [In] byte[] groupName);
        #endregion
        #region Forget Reader Group
        public static ErrorCodes ForgetReaderGroup(IntPtr contextHandle, string groupName, System.Text.Encoding characterEncoding)
        {
            ErrorCodes retCode = 0;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                log.Warn("Unable to invoke SCardForgetReaderGroup with null or empty group name parametr");
                return ErrorCodes.SCARD_E_INVALID_PARAMETER;
            }

            byte[] group = ConvertData(groupName, characterEncoding);

            try
            {
                retCode = (ErrorCodes)SCardForgetReaderGroup(contextHandle, group);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardForgetReaderGroup", CharSet = CharSet.Ansi)]
        private static extern int SCardForgetReaderGroup([In] IntPtr contextHandle, [In] byte[] groupName);
        #endregion
        #region Status
        public static ErrorCodes Status(IntPtr cardHandle, out string readerName, System.Text.Encoding characterEncoding, out CardState state, out Protocol protocol, out byte[] atr)
        {
            const int maxAtrSize = 36;
            ErrorCodes retCode = 0;
            try
            {
                
                var tempAtr = new byte[maxAtrSize];
                int tempAtrLength = tempAtr.Length;
                var nameBuffer = new byte[200];
                int nameBufferLength = nameBuffer.Length;
                int tempProtocol = 0;
                int tempState = 1;

                retCode = (ErrorCodes)SCardStatus(cardHandle, nameBuffer, ref nameBufferLength, out tempState, out tempProtocol, tempAtr, ref tempAtrLength);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS) 
                {
                    log.Error(GetAttributeDescription(retCode));
                    readerName = string.Empty;
                    state = CardState.Unknown;
                    protocol = Protocol.None;
                    atr = null;
                    return retCode;
                }

                readerName = ConvertData(nameBuffer, characterEncoding)[0];
                state = (CardState)tempState;
                protocol = (Protocol)tempProtocol;
                atr = tempAtr.Take(tempAtrLength).ToArray();
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                readerName = null;
                protocol = Protocol.None;
                state = CardState.Unknown;
                atr = null;
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardStatus", CharSet = CharSet.Ansi)]
        private static extern int SCardStatus([In] IntPtr cardHandle, 
                                              [Out] byte[] readerName, 
                                              [In, Out, Optional] ref int readerNameLength, 
                                              [Out, Optional] out int state, 
                                              [Out, Optional] out int protocol, 
                                              [Out] byte[] atr, 
                                              [In, Out, Optional] ref int atrLength);
        #endregion
        #region Control
        public static ErrorCodes Control(IntPtr cardHandle, ReaderControlCode controlCode, byte[] inputData, out byte[] outputData)
        {
            ErrorCodes retCode = 0;
            var tempBuffer = new byte[_maxBufferSize];
            var input = new byte[inputData.Length];
            try
            {
                int dataSize = 0;
                Array.Copy(inputData, input, inputData.Length);
                retCode = (ErrorCodes)SCardControl(cardHandle, (int)controlCode, input, input.Length, tempBuffer, tempBuffer.Length, out dataSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    outputData = null;
                    return retCode;
                }

                var tempData = new byte[dataSize];
                Array.Copy(tempBuffer, tempData, dataSize);
                outputData = tempData;
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                outputData = null;
                return retCode;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardControl", CharSet = CharSet.Ansi)]
        private static extern int SCardControl([In] IntPtr cardHandle, 
                                               [In] int controlCode, 
                                               [In] byte[] inputBuffer, 
                                               [In] int inputBufferSize, 
                                               [In, Out] byte[] outputBuffer, 
                                               [In] int outputBufferSize, 
                                               [Out] out int bytesReturned);
        #endregion
        #region Transmit
        public static ErrorCodes Transmit(IntPtr cardHandle, Protocol activeProtocol, byte[] inputData, out byte[] outputData)
        {
            ErrorCodes retCode = 0;
            var tempBuffer = new byte[_maxBufferSize];
            var input = new byte[inputData.Length];
            try
            {
                int dataSize = tempBuffer.Length;
                Array.Copy(inputData, input, inputData.Length);
                var inputPci = new IORequest
                {
                    Protocol = (int)activeProtocol,
                    PciLength = 8,
                };
                var outputPci = new IORequest
                {
                    Protocol = (int)activeProtocol,
                    PciLength = 8,
                };

                retCode = (ErrorCodes)SCardTransmit(cardHandle, ref inputPci, input, input.Length, ref outputPci, tempBuffer, ref dataSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    outputData = null;
                    return retCode;
                }

                var tempData = new byte[dataSize];
                Array.Copy(tempBuffer, tempData, dataSize);
                outputData = tempData;
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                outputData = null;
                return retCode;
            }
        }
        
        [DllImport("WinSCard.dll", EntryPoint = "SCardTransmit", CharSet = CharSet.Ansi)]
        private static extern int SCardTransmit([In] IntPtr cardHandle, 
                                                [In, Out] ref IORequest sendPci, 
                                                [In] byte[] inputBuffer, 
                                                [In] int sendLength, 
                                                [In, Out, Optional] ref IORequest recvPci, 
                                                [Out] byte[] outputBuffer, 
                                                [In, Out] ref int recvLength);
        #endregion        
        #region Get Attribiute
        public static ErrorCodes GetAttrib(IntPtr cardHandle, Attribiutes attributeId, out byte[] attributeData)
        {
            ErrorCodes retCode = 0;
            var dataBuffer = new byte[_maxBufferSize];
            try
            {
                int dataSize = dataBuffer.Length;
                retCode = (ErrorCodes)SCardGetAttrib(cardHandle, (int)attributeId, dataBuffer, ref dataSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    attributeData = null;
                    return retCode;
                }

                var tempData = new byte[dataSize];
                Array.Copy(dataBuffer, tempData, dataSize);
                attributeData = tempData;
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                attributeData = null;
                return retCode;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardGetAttrib", CharSet = CharSet.Ansi)]
        private static extern int SCardGetAttrib([In] IntPtr cardHandle,
                                                 [In] int attributeId,
                                                 [Out] byte[] attributeData,
                                                 [In, Out] ref int attributeDataLength);
        #endregion
        #region Set Attribute
        public static ErrorCodes SetAttrib(IntPtr cardHandle, Attribiutes attributeId, byte[] attributeData)
        {
            ErrorCodes retCode = 0;
            var dataBuffer = new byte[attributeData.Length];
            Array.Copy(attributeData, dataBuffer, attributeData.Length);
            try
            {
                int dataSize = dataBuffer.Length;
                retCode = (ErrorCodes)SCardSetAttrib(cardHandle, (int)attributeId, dataBuffer, dataSize);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));

                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return retCode;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardSetAttrib", CharSet = CharSet.Ansi)]
        private static extern int SCardSetAttrib([In] IntPtr cardHandle, 
                                                 [In] int attributeId, 
                                                 [In] byte[] attributeData, 
                                                 [In] int attributeDataLen);
        #endregion
        #region Cancel
        public static ErrorCodes Cancel(IntPtr contextHandle)
        {
            try
            {
                ErrorCodes retCode = (ErrorCodes)SCardCancel(contextHandle);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS) 
                    log.Error(GetAttributeDescription(retCode));
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardCancel", CharSet = CharSet.Ansi)]
        private static extern int SCardCancel([In] IntPtr contextHandle);
        #endregion
        #region Begin Transaction
        public static ErrorCodes BeginTransaction(IntPtr cardHandle)
        {
            try
            {
                ErrorCodes retCode = (ErrorCodes)SCardBeginTransaction(cardHandle);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardBeginTransaction", CharSet = CharSet.Ansi)]
        private static extern int SCardBeginTransaction([In] IntPtr cardHandle);
        #endregion
        #region End Transaction
        public static ErrorCodes EndTransaction(IntPtr cardHandle, CardDisposition disposition)
        {
            try
            {
                ErrorCodes retCode = (ErrorCodes)SCardEndTransaction(cardHandle, (int)disposition);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                    log.Error(GetAttributeDescription(retCode));
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }
        
        [DllImport("WinSCard.dll", EntryPoint = "SCardEndTransaction", CharSet = CharSet.Ansi)]
        public static extern int SCardEndTransaction([In] IntPtr cardHandle, [In] int disposition);
        #endregion
        #region GetStatusChange
        public static ErrorCodes GetStatusChange(IntPtr contextHandle, int timeout, ref ReaderState[] readerStates, int numberOfElements)
        {
            ErrorCodes retCode = 0;
            try
            {
                var tempReaderStates = new ReaderState[readerStates.Length];
                Array.Copy(readerStates, tempReaderStates, readerStates.Length);

                retCode = (ErrorCodes)SCardGetStatusChange(contextHandle, timeout, tempReaderStates, numberOfElements);
                if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                {
                    log.Error(GetAttributeDescription(retCode));
                    return retCode;
                }
                Array.Copy(tempReaderStates, readerStates, tempReaderStates.Length);
                return retCode;
            }
            catch (Exception error)
            {
                log.Fatal(null, error);
                return ErrorCodes.SCARD_F_UNKNOWN_ERROR;
            }
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardGetStatusChangeA", CharSet = CharSet.Ansi)]
        private static extern int SCardGetStatusChange([In] IntPtr contextHandle, 
                                                      [In] int timeout, 
                                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ReaderState[] readerStates, 
                                                      [In] int size);
        #endregion

        [DllImport("WinSCard.dll", EntryPoint = "SCardFreeMemory", CharSet = CharSet.Ansi)]
        public static extern int SCardFreeMemory([In] IntPtr contextHandle, [In] IntPtr MemoryHandle);
    }
}
