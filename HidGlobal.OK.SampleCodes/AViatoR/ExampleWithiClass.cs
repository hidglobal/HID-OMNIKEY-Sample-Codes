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
using System.Text.RegularExpressions;
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.SecureSession;
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    public class ExampleWithiClass
    {
        private static bool IsValidSessionKeyFormat(string sessionKey)
        {
            var sessionKeyPattern = new Regex("\\A[0-9A-Fa-f]{32}\\z", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));

            if (string.IsNullOrWhiteSpace(sessionKey))
                return false;

            if (!sessionKeyPattern.IsMatch(sessionKey))
                return false;

            return true;
        }

        public class LoadKeyToPcScContainerExample
        {
            private const byte KeyRelatedAccessRight = (byte)SessionAccessKeyType.UserAdminCipherKey;

            /// <summary>
            /// Admin access encryption key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string EncKey = "";
            /// <summary>
            /// Admin access mac key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string MacKey = ""; 
            private void LoadKeyCommand(ISecureChannel session, string description, byte keySlot, LoadKeyCommand.KeyType keyType, LoadKeyCommand.Persistence persistence, LoadKeyCommand.Transmission transmission, LoadKeyCommand.KeyLength keyLength, string key)
            {
                var loadKeyCommand = new Readers.AViatoR.Components.LoadKeyCommand();

                string input = loadKeyCommand.GetApdu(keySlot, keyType, persistence, transmission, keyLength, key);
                string output = session.SendCommand(input);
                ConsoleWriter.Instance.PrintCommand(description + key, input, output);
            }
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                var secureChannel = new Readers.SecureSession.SecureChannel(reader);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask("Establishing SAM Secure Session");

                    if (!IsValidSessionKeyFormat(EncKey) || !IsValidSessionKeyFormat(MacKey))
                        throw new ArgumentException("Secure session key format is incorrect, correct format of session key string is 32 character long hexadecimal string without hex specifier. Example: \"00000000000000000000000000000000\"");

                    secureChannel.Establish(EncKey + MacKey, KeyRelatedAccessRight);

                    if (secureChannel.IsSessionActive)
                    {
                        ConsoleWriter.Instance.PrintMessage("Session established");
                        ConsoleWriter.Instance.PrintSplitter();

                        LoadKeyCommand(secureChannel, "Load Mifare Key: ", 0x00,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, "FFFFFFFFFFFF");

                        LoadKeyCommand(secureChannel, "Load iCLASS volatile key: ", 0x41,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Volatile,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._8Bytes, "FFFFFFFFFFFFFFFF");

                        LoadKeyCommand(secureChannel, "Load Secure Session ReadOnlyAccess Enc key: ", 0x48,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.ReaderKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._16Bytes, "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");

                        LoadKeyCommand(secureChannel, "Load Secure Session ReadOnlyAccess Mac key: ", 0x49,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.ReaderKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._16Bytes, "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");

                        LoadKeyCommand(secureChannel, "Load Secure Session ReadWriteAccess Enc key: ", 0x46,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.ReaderKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._16Bytes, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

                        LoadKeyCommand(secureChannel, "Load Secure Session ReadWriteAccess Mac key: ", 0x47,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.ReaderKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._16Bytes, "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");

                        ConsoleWriter.Instance.PrintSplitter();
                    }
                    else
                    {
                        ConsoleWriter.Instance.PrintError("Failed to establish session");
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (secureChannel.IsSessionActive)
                    {
                        secureChannel.Terminate();
                        ConsoleWriter.Instance.PrintMessage("SAM Secure Session terminated.");
                    }
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
                
            }
        }
        public class ReadBinaryiClass16kExample
        {
            private const byte KeyRelatedAccessRight = (byte)SessionAccessKeyType.UserAdminCipherKey;
            /// <summary>
            /// Admin access encryption key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string EncKey = "";
            /// <summary>
            /// Admin access mac key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string MacKey = "";
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                var secureChannel = new Readers.SecureSession.SecureChannel(reader);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask("Establishing Secure Session");

                    if (!IsValidSessionKeyFormat(EncKey) || !IsValidSessionKeyFormat(MacKey))
                        throw new ArgumentException("Secure session key format is incorrect, correct format of session key string is 32 character long hexadecimal string without hex specifier. Example: \"00000000000000000000000000000000\"");

                    secureChannel.Establish(EncKey + MacKey, KeyRelatedAccessRight);

                    if (secureChannel.IsSessionActive)
                    {
                        ConsoleWriter.Instance.PrintMessage("Session established");
                        ConsoleWriter.Instance.PrintSplitter();

                        ReaderHelper.GeneralAuthenticateiClass(secureChannel,
                            "Authenticate without implicit selection. Key from slot: ", BookNumber.Book0,
                            PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.On,
                            GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x21);

                        ReaderHelper.ReadBinaryiClassCommand(secureChannel, "Read block without select, block: ",
                            ReadBinaryCommand.ReadOption.WithoutSelect, 0x14, 0x00);

                        ConsoleWriter.Instance.PrintSplitter();
                    }
                    else
                    {
                        ConsoleWriter.Instance.PrintError("Failed to establish session");
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (secureChannel.IsSessionActive)
                    {
                        secureChannel.Terminate();
                        ConsoleWriter.Instance.PrintMessage("Secure Session terminated.");
                    }
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class UpdateBinaryiClass16kExample
        {
            private const byte KeyRelatedAccessRight = (byte)SessionAccessKeyType.UserAdminCipherKey;
            /// <summary>
            /// Admin access encryption key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string EncKey = "";
            /// <summary>
            /// Admin access mac key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string MacKey = "";
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                var secureChannel = new Readers.SecureSession.SecureChannel(reader);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask("Establishing Secure Session");

                    if (!IsValidSessionKeyFormat(EncKey) || !IsValidSessionKeyFormat(MacKey))
                        throw new ArgumentException("Secure session key format is incorrect, correct format of session key string is 32 character long hexadecimal string without hex specifier. Example: \"00000000000000000000000000000000\"");

                    secureChannel.Establish(EncKey + MacKey, KeyRelatedAccessRight);

                    if (secureChannel.IsSessionActive)
                    {
                        ConsoleWriter.Instance.PrintMessage("Session established");
                        ConsoleWriter.Instance.PrintSplitter();

                        ReaderHelper.GeneralAuthenticateiClass(secureChannel,
                            "Authenticate without implicit selection. Key from slot: ", BookNumber.Book0,
                            PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.On,
                            GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x21);

                        ReaderHelper.UpdateBinaryCommand(secureChannel, "Update binary, target block nr: ",
                            UpdateBinaryCommand.Type.Plain, 0x14, "BACDEF0122345678");

                        ConsoleWriter.Instance.PrintSplitter();
                    }
                    else
                    {
                        ConsoleWriter.Instance.PrintError("Failed to establish session");
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (secureChannel.IsSessionActive)
                    {
                        secureChannel.Terminate();
                        ConsoleWriter.Instance.PrintMessage("Secure Session terminated.");
                    }
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }       
        }
        public class ReadBinaryiClass2ksExample
        {
            private const byte KeyRelatedAccessRight = (byte)SessionAccessKeyType.UserAdminCipherKey;
            /// <summary>
            /// Admin access encryption key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string EncKey = "";
            /// <summary>
            /// Admin access mac key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string MacKey = "";
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                var secureChannel = new Readers.SecureSession.SecureChannel(reader);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask("Establishing Secure Session");

                    if (!IsValidSessionKeyFormat(EncKey) || !IsValidSessionKeyFormat(MacKey))
                        throw new ArgumentException("Secure session key format is incorrect, correct format of session key string is 32 character long hexadecimal string without hex specifier. Example: \"00000000000000000000000000000000\"");

                    secureChannel.Establish(EncKey + MacKey, KeyRelatedAccessRight);

                    if (secureChannel.IsSessionActive)
                    {
                        ConsoleWriter.Instance.PrintMessage("Session established");
                        ConsoleWriter.Instance.PrintSplitter();

                        ReaderHelper.GeneralAuthenticateiClass(secureChannel,
                            "Authenticate without implicit selection. Key from slot: ", BookNumber.Book0,
                            PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.Off,
                            GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x21);

                        ReaderHelper.ReadBinaryiClassCommand(secureChannel, "Read block without select, block: ",
                            ReadBinaryCommand.ReadOption.WithoutSelect, 0x14, 0x00);

                        ConsoleWriter.Instance.PrintSplitter();
                    }
                    else
                    {
                        ConsoleWriter.Instance.PrintError("Failed to establish session");
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (secureChannel.IsSessionActive)
                    {
                        secureChannel.Terminate();
                        ConsoleWriter.Instance.PrintMessage("Secure Session terminated.");
                    }
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class UpdateBinaryiClass2ksExample
        {
            private const byte KeyRelatedAccesRight = (byte)SessionAccessKeyType.UserAdminCipherKey;
            /// <summary>
            /// Admin access encryption key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string EncKey = "";
            /// <summary>
            /// Admin access mac key, fill this field with valid 16 byte long key (hexadecimal string without hex specifier).
            /// Example of correct hexadecimal string key formating "00000000000000000000000000000000".
            /// </summary>
            private const string MacKey = "";
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                var secureChannel = new Readers.SecureSession.SecureChannel(reader);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask("Establishing Secure Session");

                    if (!IsValidSessionKeyFormat(EncKey) || !IsValidSessionKeyFormat(MacKey))
                        throw new ArgumentException("Secure session key format is incorrect, correct format of session key string is 32 character long hexadecimal string without hex specifier. Example: \"00000000000000000000000000000000\"");

                    secureChannel.Establish(EncKey + MacKey, KeyRelatedAccesRight);

                    if (secureChannel.IsSessionActive)
                    {
                        ConsoleWriter.Instance.PrintMessage("Session established");
                        ConsoleWriter.Instance.PrintSplitter();

                        ReaderHelper.GeneralAuthenticateiClass(secureChannel,
                            "Authenticate without implicit selection. Key from slot: ", BookNumber.Book0,
                            PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.Off,
                            GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x21);

                        ReaderHelper.UpdateBinaryCommand(secureChannel, "Update binary, target block nr: ",
                            UpdateBinaryCommand.Type.Plain, 0x14, "BACDEF0122345678");

                        ConsoleWriter.Instance.PrintSplitter();
                    }
                    else
                    {
                        ConsoleWriter.Instance.PrintError("Failed to establish session");
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (secureChannel.IsSessionActive)
                    {
                        secureChannel.Terminate();
                        ConsoleWriter.Instance.PrintMessage("Secure Session terminated.");
                    }
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class UpdateBinaryiClass2ksOK5023Example
        {
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateiClass(reader,
                        "Authenticate without implicit selection. Key from slot: ", BookNumber.Book0,
                        PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.Off,
                        GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x23);

                    ReaderHelper.UpdateBinaryCommand(reader, "Update binary, target block nr: ",
                        UpdateBinaryCommand.Type.Plain, 0x14, "BACDEF0122345678");

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class ReadBinaryiClass2ksOK5023Example
        {
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateiClass(reader,
                        "Authenticate without implicit selection. Key from slot: ", BookNumber.Book0,
                        PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.Off,
                        GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x23);

                    ReaderHelper.ReadBinaryiClassCommand(reader, "Read block without select, block: ",
                        ReadBinaryCommand.ReadOption.WithoutSelect, 0x14, 0x00);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class UpdateBinaryiClass16kOK5023Example
        {
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateiClass(reader,
                        "Authenticate with implicit selection. Key from slot: ", BookNumber.Book0,
                        PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.On,
                        GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x23);

                    ReaderHelper.UpdateBinaryCommand(reader, "Update binary with plain data, target block nr: ",
                        UpdateBinaryCommand.Type.Plain, 0x14, "BACDEF0122345678");

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class ReadBinaryiClass16kOK5023Example
        {
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateiClass(reader,
                        "Authenticate with implicit selection. Key from slot: ", BookNumber.Book0,
                        PageNumber.Page0, GeneralAuthenticateCommand.ImplicitSelection.On,
                        GeneralAuthenticateCommand.iClassKeyType.PicoPassCreditKeyKC, 0x23);

                    ReaderHelper.ReadBinaryiClassCommand(reader, "Read block without select, block: ",
                        ReadBinaryCommand.ReadOption.WithoutSelect, 0x14, 0x00);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
    }
}
