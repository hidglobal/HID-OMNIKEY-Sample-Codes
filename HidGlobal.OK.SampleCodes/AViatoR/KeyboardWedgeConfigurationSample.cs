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
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.AViatoR.Components.ConfigurationCardSupport;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.Utilities;
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    public class KeyboardWedgeConfigurationSample
    {
        private static void PrintCommand(string name, string input, string output)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
        }

        public class CredentialTypeCommand
        {
            private void GetCredentialType(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup)
            {
                var command = new GetKeyboardWedgeCardTypeCommand();
                var apdu = command.GetApdu(configGroup);
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeCardTypeCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Credential Type for configuration group {configGroup.ToString()} is set to {response.CardType.ToString()} card.");
            }

            private void SetCredentialType(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup, CardType cardType)
            {
                var command = new SetKeyboardWedgeCardTypeCommand();
                var apdu = command.GetApdu(configGroup, cardType);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Credential Type for Configuration group {configGroup.ToString()} to a {cardType.ToString()} card.");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetCredentialType(smartCardReader, "Get Credential Type.", ConfigurationType.KeyboardWedgeConfig1);
                }
                else
                {
                    SetCredentialType(smartCardReader, $"Set Credential Type.", ConfigurationType.KeyboardWedgeConfig1,
                        CardType.DesFire);
                    SetCredentialType(smartCardReader, $"Set Credential Type.", ConfigurationType.KeyboardWedgeConfig2,
                        CardType.MifareClassic);
                    SetCredentialType(smartCardReader, $"Set Credential Type.", ConfigurationType.KeyboardWedgeConfig3,
                        CardType.Iclass);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class OutputFlagsCommand
        {
            private void GetOutputFlags(ISmartCardReader smartCardReader, string comment, ConfigurationType configGroup)
            {
                var command = new GetKeyboardWedgeOutputFlagsCommand();
                var apdu = command.GetApdu(configGroup);
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeOutputFlagsCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Output Flags for configuration group {configGroup.ToString()} are set to: \n Data type - {response.OutputType.ToString()}, Bit order - {response.BitOrder.ToString()}, Byte order - {response.ByteOrder.ToString()} .");
            }

            private void SetOutputFlags(ISmartCardReader smartCardReader, string comment, ConfigurationType configGroup,
                OutputType outputType, DataOrder bitOrder, DataOrder byteOrder)
            {
                var command = new SetKeyboardWedgeOutputFlagsCommand();
                var apdu = command.GetApdu(configGroup, outputType, bitOrder, byteOrder);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Output Flags for Configuration group {configGroup.ToString()} to: \n Data type - {outputType.ToString()}, Bit order - {bitOrder.ToString()}, Byte order - {byteOrder.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetOutputFlags(smartCardReader, "Get Output Flags.", ConfigurationType.KeyboardWedgeConfig1);
                }
                else
                {
                    SetOutputFlags(smartCardReader, $"Set Output Flags.", ConfigurationType.KeyboardWedgeConfig1,
                        OutputType.Pacs, DataOrder.Normal, DataOrder.Normal);
                    SetOutputFlags(smartCardReader, $"Set Output Flags.", ConfigurationType.KeyboardWedgeConfig2,
                        OutputType.Uid, DataOrder.Reversed, DataOrder.Normal);
                    SetOutputFlags(smartCardReader, $"Set Output Flags.", ConfigurationType.KeyboardWedgeConfig3,
                        OutputType.Pacs, DataOrder.Normal, DataOrder.Reversed);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class OutputFormatCommand
        {
            private void GetOutputFormat(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup)
            {
                var command = new GetKeyboardWedgeOutputFormatCommand();
                var apdu = command.GetApdu(configGroup);
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeOutputFormatCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Output Format for configuration group {configGroup.ToString()} is set to {response.OutputFormat.ToString()} .");
            }

            private void SetOutputFormat(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup, OutputFormat outputFormat)
            {
                var command = new SetKeyboardWedgeOutputFormatCommand();
                var apdu = command.GetApdu(configGroup, outputFormat);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Output Format for Configuration group {configGroup.ToString()} to a {outputFormat.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetOutputFormat(smartCardReader, "Get Output Format.", ConfigurationType.KeyboardWedgeConfig1);
                }
                else
                {
                    SetOutputFormat(smartCardReader, $"Set Output Format.", ConfigurationType.KeyboardWedgeConfig1,
                        OutputFormat.HexUpperCase);
                    SetOutputFormat(smartCardReader, $"Set Output Format.", ConfigurationType.KeyboardWedgeConfig1,
                        OutputFormat.Bcd);
                    SetOutputFormat(smartCardReader, $"Set Output Format.", ConfigurationType.KeyboardWedgeConfig1,
                        OutputFormat.Bin);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class RangeLengthCommand
        {
            private void GetRangeLength(ISmartCardReader smartCardReader, string comment, ConfigurationType configGroup)
            {
                var command = new GetKeyboardWedgeRangeLengthCommand();
                var apdu = command.GetApdu(configGroup);
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeRangeLengthCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Range Length for configuration group {configGroup.ToString()} is set to {response.RangeLength.ToString()} .");
            }

            private void SetRangeLength(ISmartCardReader smartCardReader, string comment, ConfigurationType configGroup,
                byte length)
            {
                var command = new SetKeyboardWedgeRangeLengthCommand();
                var apdu = command.GetApdu(configGroup, length);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Range Length for Configuration group {configGroup.ToString()} to {length.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetRangeLength(smartCardReader, "Get Rgange Offset.", ConfigurationType.KeyboardWedgeConfig1);
                }
                else
                {
                    SetRangeLength(smartCardReader, $"Set Range Length.", ConfigurationType.KeyboardWedgeConfig1, 3);
                    SetRangeLength(smartCardReader, $"Set Range Length.", ConfigurationType.KeyboardWedgeConfig1, 16);
                    SetRangeLength(smartCardReader, $"Set Range Length.", ConfigurationType.KeyboardWedgeConfig1, 0xFF);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class RangeOffsetCommand
        {
            private void GetRangeOffset(ISmartCardReader smartCardReader, string comment, ConfigurationType configGroup)
            {
                var command = new GetKeyboardWedgeRangeStartCommand();
                var apdu = command.GetApdu(configGroup);
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeRangeStartCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Range Offset for configuration group {configGroup.ToString()} is set to {response.RangeStart.ToString()} .");
            }

            private void SetRangeOffset(ISmartCardReader smartCardReader, string comment, ConfigurationType configGroup,
                byte offset)
            {
                var command = new SetKeyboardWedgeRangeStartCommand();
                var apdu = command.GetApdu(configGroup, offset);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Range Offset for Configuration group {configGroup.ToString()} to {offset.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetRangeOffset(smartCardReader, "Get Rgange Offset.", ConfigurationType.KeyboardWedgeConfig1);
                }
                else
                {
                    SetRangeOffset(smartCardReader, $"Set Range Offset.", ConfigurationType.KeyboardWedgeConfig1, 3);
                    SetRangeOffset(smartCardReader, $"Set Range Offset.", ConfigurationType.KeyboardWedgeConfig1, 16);
                    SetRangeOffset(smartCardReader, $"Set Range Offset.", ConfigurationType.KeyboardWedgeConfig1, 0);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class PostStrokesStartCommand
        {
            private void GetPostStrokesStart(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup)
            {
                var command = new GetKeyboardWedgePostStrokesStartCommand();
                var apdu = command.GetApdu(configGroup);
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgePostStrokesStartCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Post Strokes Start for configuration group {configGroup.ToString()} is set to {response.PostStrokesStart.ToString()} .");
            }

            private void SetPostStrokesStart(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup, byte offset)
            {
                var command = new SetKeyboardWedgePostStrokesStartCommand();
                var apdu = command.GetApdu(configGroup, offset);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Post Strokes Start for Configuration group {configGroup.ToString()} to {offset.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetPostStrokesStart(smartCardReader, "Get Post Strokes Start.",
                        ConfigurationType.KeyboardWedgeConfig1);
                }
                else
                {
                    SetPostStrokesStart(smartCardReader, $"Set Post Strokes Start.",
                        ConfigurationType.KeyboardWedgeConfig1, 3);
                    SetPostStrokesStart(smartCardReader, $"Set Post Strokes Start.",
                        ConfigurationType.KeyboardWedgeConfig1, 31);
                    SetPostStrokesStart(smartCardReader, $"Set Post Strokes Start.",
                        ConfigurationType.KeyboardWedgeConfig1, 16);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class PrePostStrokesCommand
        {
            private void GetPrePostStrokes(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup)
            {
                var command = new GetKeyboardWedgePrePostStrokesCommand();
                var apdu = command.GetApdu(configGroup);
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgePrePostStrokesCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                var strokes = String.Join("", Encoding.ASCII.GetChars(response.PrePostStrokes));
                ConsoleWriter.Instance.PrintMessage(
                    $"Pre Post Strokes for configuration group {configGroup.ToString()} are set to: \'{strokes.ToString()}\' .");
            }

            private void SetPrePostStrokes(ISmartCardReader smartCardReader, string comment,
                ConfigurationType configGroup, string strokes)
            {
                var rawStrokes = new List<byte>();

                foreach (var stroke in strokes)
                {
                    rawStrokes.Add(Convert.ToByte(stroke));
                }

                var command = new SetKeyboardWedgePrePostStrokesCommand();
                var apdu = command.GetApdu(configGroup, rawStrokes.ToArray());

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Pre Post Strokes for Configuration group {configGroup.ToString()} to: \'{strokes.ToString()}\' .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetPrePostStrokes(smartCardReader, "Get Pre Post Strokes.", ConfigurationType.KeyboardWedgeConfig1);
                }
                else
                {
                    SetPrePostStrokes(smartCardReader, $"Set Pre Post Strokes.", ConfigurationType.KeyboardWedgeConfig1,
                        "pre- post-");
                    SetPrePostStrokes(smartCardReader, $"Set Pre Post Strokes.", ConfigurationType.KeyboardWedgeConfig1,
                        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                    SetPrePostStrokes(smartCardReader, $"Set Pre Post Strokes.", ConfigurationType.KeyboardWedgeConfig1,
                        "aaaaaaaaaaaaabbbbbbbbbbbbbbbbbbb");
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class KeyboardLayoutCommand
        {
            private void GetKeyboardLayout(ISmartCardReader smartCardReader, string comment)
            {
                var command = new GetKeyboardWedgeKeyboardLayoutCommand();
                var apdu = command.GetApdu();
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeKeyboardLayoutCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Keyboard Layout is set to {response.KeyboardLayout.ToString()} .");
            }

            private void SetKeyboardLayout(ISmartCardReader smartCardReader, string comment,
                KeyboardLayout keyboardLayout)
            {
                var command = new SetKeyboardWedgeKeyboardLayoutCommand();
                var apdu = command.GetApdu(keyboardLayout);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Set Keyboard Layout to {keyboardLayout.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetKeyboardLayout(smartCardReader, "Get Keyboard Layout.");
                }
                else
                {
                    SetKeyboardLayout(smartCardReader, $"Set Keyboard Layout.", KeyboardLayout.DefaultUsLayout);
                    SetKeyboardLayout(smartCardReader, $"Set Keyboard Layout.", KeyboardLayout.SecondLayout);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class ExtendedCharacterSupportCommand
        {
            private void GetExtendedCharacterSupport(ISmartCardReader smartCardReader, string comment)
            {
                var command = new GetKeyboardWedgeSupportForExtendedCharsCommand();
                var apdu = command.GetApdu();
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeSupportForExtendedCharsCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Extended Character Support is set to {response.ExtendedCharacterSupport.ToString()} .");
            }

            private void SetExtendedCharacterSupport(ISmartCardReader smartCardReader, string comment,
                ExtendedCharacterSupport extendedCharacterSupport)
            {
                var command = new SetKeyboardWedgeSupportForExtendedCharsCommand();
                var apdu = command.GetApdu(extendedCharacterSupport);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Set Extended Character Support to {extendedCharacterSupport.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetExtendedCharacterSupport(smartCardReader, "Get Extended Character Support.");
                }
                else
                {
                    SetExtendedCharacterSupport(smartCardReader, $"Set Extended Character Support.",
                        ExtendedCharacterSupport.Linux);
                    SetExtendedCharacterSupport(smartCardReader, $"Set Extended Character Support.",
                        ExtendedCharacterSupport.MacOs);
                    SetExtendedCharacterSupport(smartCardReader, $"Set Extended Character Support.",
                        ExtendedCharacterSupport.Windows);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class CharacterDifferencesCommand
        {
            private void GetCharacterDifferences(ISmartCardReader smartCardReader, string comment)
            {
                var command = new GetKeyboardWedgeCharactersDiffCommand();
                var apdu = command.GetApdu();
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetKeyboardWedgeCharactersDiffCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                var charDiffs = response.CharacterDifferences;
                ConsoleWriter.Instance.PrintMessage("Character Differences array:\n" +
                                                    "\tMapped charracter \tModifier key      \tKeyboard scan code");
                foreach (var characterDiff in charDiffs)
                {
                    ConsoleWriter.Instance.PrintMessage(
                        $"\t      {characterDiff.Character.ToString()}(\"{Convert.ToChar(characterDiff.Character)}\")    " +
                        $"\t      {characterDiff.Modifiers.ToString()}           " +
                        $"\t      {characterDiff.KeyVal.ToString()}");
                }

            }

            private void SetCharacterDifferences(ISmartCardReader smartCardReader, string comment,
                CharacterDiff[] charDiffs)
            {
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage("Set Character Differences to:\n" +
                                                    "\tMapped charracter \tModifier key      \tKeyboard scan code");
                foreach (var characterDiff in charDiffs)
                {
                    ConsoleWriter.Instance.PrintMessage(
                        $"\t      {characterDiff.Character.ToString()}(\"{Convert.ToChar(characterDiff.Character)}\")    " +
                        $"\t      {characterDiff.Modifiers.ToString()}           " +
                        $"\t      {characterDiff.KeyVal.ToString()}");
                }

                var command = new SetKeyboardWedgeCharactersDiffCommand();
                var apdu = command.GetApdu(charDiffs);

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetCharacterDifferences(smartCardReader, "Get Character Differences.");
                }
                else
                {
                    SetCharacterDifferences(smartCardReader, $"Set Character Differences.", new CharacterDiff[] { });

                    SetCharacterDifferences(smartCardReader, $"Set Character Differences.",
                        new CharacterDiff[] {new CharacterDiff {Character = 0x5A, KeyVal = 0x1C, Modifiers = 0x2}});

                    SetCharacterDifferences(smartCardReader, $"Set Character Differences.",
                        new CharacterDiff[]
                        {
                            new CharacterDiff {Character = 0x5A, KeyVal = 0x1C, Modifiers = 0x2},
                            new CharacterDiff {Character = 0x5B, KeyVal = 0x1C, Modifiers = 0x2}
                        });

                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class ConfigurationCardSupportCommand
        {
            private void GetConfigurationCardSupport(ISmartCardReader smartCardReader, string comment)
            {
                var command = new GetConfigurationCardSupportEnabledCommand();
                var apdu = command.GetApdu();
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetConfigurationCardSupportEnabledCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage(
                    $"Configuration Card Support is set to {response.FeatureSupport.ToString()} .");
            }

            private void SetConfigurationCardSupport(ISmartCardReader smartCardReader, string comment,
                FeatureSupport featureSupport)
            {
                var command = new SetConfigurationCardSupportEnabledCommand();
                var apdu = command.GetApdu(featureSupport);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Set Configuration Card Support to {featureSupport.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetConfigurationCardSupport(smartCardReader, "Get Configuration Card Support.");
                }
                else
                {
                    SetConfigurationCardSupport(smartCardReader, $"Set Configuration Card Support.",
                        FeatureSupport.Disabled);
                    SetConfigurationCardSupport(smartCardReader, $"Set Configuration Card Support.",
                        FeatureSupport.Enabled);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class LedIdleStateCommand
        {
            private void GetLedIdleState(ISmartCardReader smartCardReader, string comment)
            {
                var command = new GetLedIdleStateCommand();
                var apdu = command.GetApdu();
                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                var response = command.TranslateResponse(rsp) as GetLedIdleStateCommandResponse;

                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Led Idle State is set to {response.LedIdleState.ToString()} .");
            }

            private void SetLedIdleState(ISmartCardReader smartCardReader, string comment, LedIdleState ledIdleState)
            {
                var command = new SetLedIdleStateCommand();
                var apdu = command.GetApdu(ledIdleState);

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Set Led Idle State to {ledIdleState.ToString()} .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader, RequestType requestType)
            {
                if (requestType == RequestType.Get)
                {
                    GetLedIdleState(smartCardReader, "Get Led Idle State.");
                }
                else
                {
                    SetLedIdleState(smartCardReader, $"Set Led Idle State.", LedIdleState.Off);
                    SetLedIdleState(smartCardReader, $"Set Led Idle State.", LedIdleState.On);
                }
            }

            public void Run(string readerName, RequestType requestType)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader, requestType);

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

        public class ConfigurationCardWriteDataCommand
        {
            private readonly ICollection<byte> ProductIdentifier = new byte[] { 0x50, 0x27 };
            private const byte MajorConfigurationVersion = 0x01;
            private const byte MinorConfigurationVersion = 0x00;

            private void ConfigurationCardWriteData(ISmartCardReader smartCardReader, string comment, IEnumerable<IApduCommand> configuration)
            {

                var configurationCard = new ConfigurationCard(ProductIdentifier, MajorConfigurationVersion, MinorConfigurationVersion);
                configurationCard.AddRange(configuration);

                try
                {
                    smartCardReader.Connect(ReaderSharingMode.Shared, Protocol.Any);

                    foreach (var command in configurationCard.GetConfigurationCardCreationCommands())
                    {
                        var response = smartCardReader.Transmit(command.GetBytes().ToArray());

                        if (!response.SequenceEqual(new byte[] {0x90, 0x00}))
                        {
                            throw new Exception(
                                $"Returned apdu response: 0x{HidGlobal.OK.Readers.Utilities.BinaryHelper.ConvertBytesToOctetString(response.ToArray())}");
                        }

                        PrintCommand(comment, BitConverter.ToString(command.GetBytes().ToArray()).Replace("-", ""), BitConverter.ToString(response.ToArray()).Replace("-", ""));
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Write operation failed. Ensure that MIFARE DESFire EV1 is in read range.");
                }
                finally
                {
                    smartCardReader.Disconnect(CardDisposition.Unpower);
                    ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    ConsoleWriter.Instance.PrintSplitter();
                    configurationCard.Dispose();
                }

            }

            void ExecuteExample(ISmartCardReader smartCardReader)
            {

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Create Configuration Card contining Keyborad Wedge Character Differences.");
                var configuration = new List<IApduCommand>();
                configuration.Add(new SetKeyboardWedgeCharactersDiffCommand().GetApdu(new CharacterDiff[]
                    {new CharacterDiff {Character = 0x5A, KeyVal = 0x1C, Modifiers = 0x2},}));

                ConfigurationCardWriteData(smartCardReader, "Create Configuration Card 1st sample.", configuration );


                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Create Configuration Card contining Keyborad Wedge Configuration 1 changes in case Credential type, Output format and Data type.");
                configuration.Clear();
                configuration.Add( new SetKeyboardWedgeCardTypeCommand().GetApdu(ConfigurationType.KeyboardWedgeConfig1, CardType.DesFire) );
                configuration.Add( new SetKeyboardWedgeOutputFormatCommand().GetApdu(ConfigurationType.KeyboardWedgeConfig1, OutputFormat.Bin) );
                configuration.Add( new SetKeyboardWedgeOutputFlagsCommand().GetApdu(ConfigurationType.KeyboardWedgeConfig1, OutputType.Pacs, DataOrder.Normal, DataOrder.Normal) );
                
                ConfigurationCardWriteData(smartCardReader, "Create Configuration Card 2nd sample.", configuration );


                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Create Configuration Card contining General Configuration changes in case enabling Mifare emulation feature.");
                configuration.Clear();
                var command = new Readers.AViatoR.Components.MifarePreferred().SetApdu(true);
                var apduCommand = Readers.Utilities.BinaryHelper.ConvertOctetStringToIApduInterface(command);

                configuration.Add(apduCommand);
                ConfigurationCardWriteData(smartCardReader, "Create Configuration Card 3rd sample.", configuration);
            }

            public void Run(string readerName)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ExecuteExample(reader);
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

        public class ConfigurationCardSecurityKeyUpdateCommand
        {
            private void ConfigurationCardSecurityKeyUpdate(ISmartCardReader smartCardReader, string comment, string key)
            {
                var command = new HidGlobal.OK.Readers.AViatoR.Components.ConfigurationCardSecurityKeyUpdateCommand();
                var apdu = command.GetApdu(HidGlobal.OK.Readers.Utilities.BinaryHelper.ConvertOctetStringToBytes(key));

                ConsoleWriter.Instance.PrintSplitter();
                ConsoleWriter.Instance.PrintMessage($"Configuration Card Security Key Update to \'{key.ToString()}\' .");

                var rsp = ReaderHelper.SendCommand(smartCardReader, apdu.GetBytes().ToArray());
                PrintCommand(comment, apdu.ToString(), BinaryHelper.ConvertBytesToOctetString(rsp.ToArray()));
            }

            void ExecuteExample(ISmartCardReader smartCardReader)
            {
                ConfigurationCardSecurityKeyUpdate(smartCardReader, $"Configuration Card Security Key Update.", "0123456789abcdef");
            }

            public void Run(string readerName)
            {
                using (var reader = new SmartCardReader(readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader);

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
}