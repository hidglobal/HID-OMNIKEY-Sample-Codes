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
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.SecureSession;
using HidGlobal.OK.Readers.SecureSession.SamSecureSession;
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    public class SEProcessorSample
    {
        public abstract class ExampleBase
        {
            private const string SamSecureSessionMasterKey = "00000000000000000000000000000000"; // replace with valid Key
            private const byte SamSecureSessionKeyNumber = 0x80; // End-User key role
            private string _readerName;

            protected abstract void ExecuteExample(SamSecureSession session);

            public void Run()
            {
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask("Establishing SAM Secure Session");

                    var reader = new Reader(Program.WinscardContext.Handle, _readerName);

                    using (var secureSession = new SamSecureSession(reader))
                    {
                        secureSession.Establish(SamSecureSessionMasterKey, SamSecureSessionKeyNumber);

                        if (secureSession.IsSessionActive)
                        {
                            ConsoleWriter.Instance.PrintMessage("Session established");

                            ExecuteExample(secureSession);

                            ConsoleWriter.Instance.PrintSplitter();
                        }
                        else
                        {
                            ConsoleWriter.Instance.PrintError("Failed to establish session");
                        }
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError("Exception's been thrown: message = " + e.Message);
                }
            }

            protected ExampleBase(string readerName)
            {
                _readerName = readerName;
            }

            protected void PrintCommand(string name, string input, string output, SEPCommand.SEPCommandResponse response)
            {
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintCommand(name, input, output);

                ConsoleWriter.Instance.PrintMessage("Command response: \n\treturnCode = " + response.ReturnCode);
            }

            protected void SendAuthNativeCommand(SamSecureSession session, byte keyNumber, string keyReference)
            {
                var command = new SEPDESFireAuthNative();

                var input = command.GetApdu(keyNumber, keyReference);
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Auth Native", input, output, response);
            }
        }

        public class LoadKeyExample : ExampleBase
        {
            public LoadKeyExample(string readerName) : base(readerName)
            {
            }

            protected override void ExecuteExample(SamSecureSession session)
            {
                SendLoadKeyCommand(session, SEPLoadKey.Persistence.Persistent, "030101", "00000000000000000000000000000000");
                SendLoadKeyCommand(session, SEPLoadKey.Persistence.Persistent, "030102", "101112131415161718191a1b1c1d1e1f");
            }

            private void SendLoadKeyCommand(SamSecureSession session, SEPLoadKey.Persistence persistence, string keyReference, string keyValue)
            {
                var command = new SEPLoadKey();

                var input = command.GetApdu(persistence, keyReference, keyValue);
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Load Key", input, output, response);
            }
        }

        public class DesfireFormatCardExample : ExampleBase
        {
            public DesfireFormatCardExample(string readerName) : base(readerName)
            {
            }

            protected override void ExecuteExample(SamSecureSession session)
            {
                SendAuthNativeCommand(session, 0x00, "030101");
                SendFormatCardCommand(session);
            } 

            private void SendFormatCardCommand(SamSecureSession session)
            {
                var command = new SEPDESFireFormatCard();

                var input = command.GetApdu();
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Format Card", input, output, response);
            }
        }

        public class DesfireReadWriteDataExample : ExampleBase
        {
            public DesfireReadWriteDataExample(string readerName) : base(readerName)
            {
            }

            protected override void ExecuteExample(SamSecureSession session)
            {
                SendAuthNativeCommand(session, 0x00, "030101");
                SendCreateApplicationCommand(session, "010208", 0x0f, 0x07);
                SendSelectApplicationCommand(session, "010208");
                SendCreateStandardDataFileCommand(session, 0x00, SEPCommand.CommunicationMode.Plain, "00EEEE", "000100");
                SendWriteDataCommand(session, 0x00, "00", "00112233445566778899AABBCCDDEEFF", SEPCommand.CommunicationMode.Plain, SEPCommand.CommitFlag.NoCommit);
                SendReadDataCommand(session, 0x00, "00", 0x10, SEPCommand.CommunicationMode.Plain);
            }

            private void SendCreateApplicationCommand(SamSecureSession session, string applicationNumber, byte masterKeySettings, byte numberOfKeys)
            {
                var command = new SEPDESFireCreateApplication();

                var input = command.GetApdu(applicationNumber, masterKeySettings, numberOfKeys);
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Create Application", input, output, response);
            }

            private void SendSelectApplicationCommand(SamSecureSession session, string applicationNumber)
            {
                var command = new SEPDESFireSelectApplication();

                var input = command.GetApdu(applicationNumber);
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Select Application", input, output, response);
            }

            private void SendCreateStandardDataFileCommand(SamSecureSession session, byte fileNumber,
                SEPCommand.CommunicationMode communicationMode,
                string accessRights, string fileSize)
            {
                var command = new SEPDESFireCreateStandardDataFile();

                var input = command.GetApdu(fileNumber, communicationMode, accessRights, fileSize);
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Create Standard Data File", input, output, response);
            }

            private void SendWriteDataCommand(SamSecureSession session, byte fileNumber, string offset, string dataToBeWritten,
                SEPCommand.CommunicationMode communicationMode,
                SEPCommand.CommitFlag commitFlag)
            {
                var command = new SEPDESFireWriteData();

                var input = command.GetApdu(fileNumber, offset, dataToBeWritten, communicationMode, commitFlag);
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Write Data", input, output, response);
            }

            private void SendReadDataCommand(SamSecureSession session, byte fileNumber, string offset, byte numberOfBytesToBeRead,
                SEPCommand.CommunicationMode communicationMode)
            {
                var command = new SEPDESFireReadData();

                var input = command.GetApdu(fileNumber, offset, numberOfBytesToBeRead, communicationMode);
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Read Data", input, output, response);
                PrintDetailedResponse(response);
            }

            private void PrintDetailedResponse(SEPCommand.SEPCommandResponse response)
            {
                var readDataResponse = response as SEPDESFireReadData.SEPDESFireReadDataResponse;
                if (readDataResponse != null)
                {
                    ConsoleWriter.Instance.PrintMessage("\tdata = " + readDataResponse.DataHexString);
                }
                else
                {
                    ConsoleWriter.Instance.PrintError("response was empty");
                }
            }
        }

        public class ReadPACSDataExample : ExampleBase
        {
            public ReadPACSDataExample(string readerName) : base(readerName)
            {
            }

            protected override void ExecuteExample(SamSecureSession session)
            {
                SendReadPACSDataCommand(session);
            }

            private void SendReadPACSDataCommand(SamSecureSession session)
            {
                var command = new SEPReadPACSData();

                var input = command.GetApdu();
                var output = session.SendCommand(input);
                var response = command.TranslateResponse(output);

                PrintCommand("Read PACS Data", input, output, response);
                PrintDetailedResponse(response);
            }

            private void PrintDetailedResponse(SEPCommand.SEPCommandResponse response)
            {
                var readPACSDataResponse = response as SEPReadPACSData.SEPReadPACSDataResponse;
                if (readPACSDataResponse != null)
                {
                    ConsoleWriter.Instance.PrintMessage("\tcontentElementData = " + readPACSDataResponse.ContentElementData);
                    ConsoleWriter.Instance.PrintMessage("\tsecureObjectOID = " + readPACSDataResponse.SecureObjectOID);
                    ConsoleWriter.Instance.PrintMessage("\tmediaType = " + readPACSDataResponse.MediaType);
                }
                else
                {
                    ConsoleWriter.Instance.PrintError("response was empty");
                }
            }
        }

        public class LoadKeyToPcScContainer
        {
            private const string SamSecureSessionMasterKey = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"; // replace with valid Key
            private const byte SamSecureSessionKeyNumber = 0x80; // End-User key role

            private void LoadKeyCommand(ISecureChannel session, string description, byte keySlot, LoadKeyCommand.KeyType keyType, LoadKeyCommand.Persistence persistence, LoadKeyCommand.Transmission transmission, LoadKeyCommand.KeyLength keyLength, string key)
            {
                var loadKeyCommand = new Readers.AViatoR.Components.LoadKeyCommand();

                string input = loadKeyCommand.GetApdu(keySlot, keyType, persistence, transmission, keyLength, key);
                string output = session.SendCommand(input);
                ConsoleWriter.Instance.PrintCommand(description + key, input, output);
            }
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                var secureChannel = new Readers.SecureSession.SamSecureSession.SamSecureSession(reader);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask("Establishing SAM Secure Session");

                    secureChannel.Establish(SamSecureSessionMasterKey, SamSecureSessionKeyNumber);

                    if (secureChannel.IsSessionActive)
                    {
                        ConsoleWriter.Instance.PrintMessage("Session established");

                        LoadKeyCommand(secureChannel, "Load Mifare Key: ", 0x00,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, "FFFFFFFFFFFF");

                        LoadKeyCommand(secureChannel, "Load iCLASS key, book 0 page 7 App 2: ", 0x31,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._8Bytes, "FFFFFFFFFFFFFFFF");

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
    }
}
