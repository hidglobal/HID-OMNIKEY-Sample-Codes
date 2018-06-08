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
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    public enum RequestType
    {
        Get,
        Set,
    }

    public class Iso14443TypeAConfigurationSample
    {
        private static void PrintCommand(string name, string input, string output, IEnumerable<string> value)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
            if (value != null)
            {
                ConsoleWriter.Instance.PrintMessage("Output data:");
                foreach (var data in value)
                {
                    ConsoleWriter.Instance.PrintMessage($"\t{data}");
                }
            }
        }
        private static void PrintCommand(string name, string input, string output, string value)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
            if (!string.IsNullOrWhiteSpace(value))
            {
                ConsoleWriter.Instance.PrintMessage($"Output data: {value}");
            }
        }
        public class CredentialEnable
        {
            private void GetCredentialEnable(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeAEnable();

                ConsoleWriter.Instance.PrintMessage("Get Iso 14443 A Enable");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = command.TranslateGetResponse(output);

                PrintCommand(string.Empty, input, output, value);
            }
            private void SetCredentialEnable(ISmartCardReader smartCardReader, bool setEnable)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeAEnable();

                ConsoleWriter.Instance.PrintMessage("Set Iso 14443 A Enable");

                string input = command.SetApdu(setEnable);
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                
                PrintCommand(string.Empty, input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetCredentialEnable(reader);
                                break;

                            case RequestType.Set:
                                SetCredentialEnable(reader, true);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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
        public class RxTxBaudRate
        {
            private void GetRxTxBaudRate(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeARxTxBaudRate();

                ConsoleWriter.Instance.PrintMessage("Get Iso 14443 A Baud Rate");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                var value = command.TranslateGetResponse(output);
                
                PrintCommand("ISO 14443 Type A RxTx Baud Rates", input, output, value);
            }
            private void SetRxTxBaudRate(ISmartCardReader smartCardReader, BaudRate enabledBaudRates)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeARxTxBaudRate();

                ConsoleWriter.Instance.PrintMessage("Set Iso 14443 A Enable");

                string input = command.SetApdu(enabledBaudRates);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                PrintCommand("Enable all available baud rates", input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetRxTxBaudRate(reader);
                                break;

                            case RequestType.Set:
                                var selectedBaudRates = new BaudRate()
                                {
                                    Rx106 = true,
                                    Rx212 = true,
                                    Rx424 = true,
                                    Rx848 = true,
                                    Tx106 = true,
                                    Tx212 = true,
                                    Tx424 = true,
                                    Tx848 = true
                                };

                                SetRxTxBaudRate(reader, selectedBaudRates);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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
        public class MifareEmulationPreferred
        {
            private void GetMifareEmulationPrefered(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.MifarePreferred();

                ConsoleWriter.Instance.PrintMessage("Get Iso 14443A Mifare Emulation Preferred");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = command.TranslateGetResponse(output);

                PrintCommand(string.Empty, input, output, value);
            }
            private void SetMifareEmulationPrefered(ISmartCardReader smartCardReader, bool setEnable)
            {
                var command = new Readers.AViatoR.Components.MifarePreferred();

                ConsoleWriter.Instance.PrintMessage("Set Iso 14443A Mifare Emulation Preferred");

                string input = command.SetApdu(setEnable);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                PrintCommand(string.Empty, input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetMifareEmulationPrefered(reader);
                                break;

                            case RequestType.Set:
                                SetMifareEmulationPrefered(reader, true);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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
        public class MifareKeyCache
        {
            private void GetMifareKeyCache(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.MifareKeyCache();

                ConsoleWriter.Instance.PrintMessage("Get Iso 14443A Mifare Key Cache");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = command.TranslateGetResponse(output);

                PrintCommand(string.Empty, input, output, value);
            }
            private void SetMifareKeyCache(ISmartCardReader smartCardReader, bool setEnable)
            {
                var command = new Readers.AViatoR.Components.MifareKeyCache();

                ConsoleWriter.Instance.PrintMessage("Set Iso 14443A Mifare Key Cache");

                string input = command.SetApdu(setEnable);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                PrintCommand(string.Empty, input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetMifareKeyCache(reader);
                                break;

                            case RequestType.Set:
                                SetMifareKeyCache(reader, true);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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

    public class Iso14443TypeBConfigurationSample
    {
        private static void PrintCommand(string name, string input, string output, IEnumerable<string> value)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
            if (value != null)
            {
                ConsoleWriter.Instance.PrintMessage("Output data:");
                foreach (var data in value)
                {
                    ConsoleWriter.Instance.PrintMessage($"\t{data}");
                }
            }
        }
        private static void PrintCommand(string name, string input, string output, string value)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
            if (!string.IsNullOrWhiteSpace(value))
            {
                ConsoleWriter.Instance.PrintMessage($"Output data: {value}");
            }
        }
        public class CredentialEnable
        {
            private void GetCredentialEnable(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeBEnable();

                ConsoleWriter.Instance.PrintMessage("Get Iso 14443 B Enable");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = command.TranslateGetResponse(output);

                PrintCommand(string.Empty, input, output, value);
            }
            private void SetCredentialEnable(ISmartCardReader smartCardReader, bool setEnable)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeBEnable();

                ConsoleWriter.Instance.PrintMessage("Set Iso 14443 B Enable");

                string input = command.SetApdu(setEnable);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                PrintCommand(string.Empty, input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetCredentialEnable(reader);
                                break;

                            case RequestType.Set:
                                SetCredentialEnable(reader, true);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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
        public class RxTxBaudRate
        {
            private void GetRxTxBaudRate(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeBRxTxBaudRate();

                ConsoleWriter.Instance.PrintMessage("Get Iso 14443 B Baud Rate");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                var value = command.TranslateGetResponse(output);

                PrintCommand("ISO 14443 Type B RxTx Baud Rates", input, output, value);
            }
            private void SetRxTxBaudRate(ISmartCardReader smartCardReader, BaudRate enabledBaudRates)
            {
                var command = new Readers.AViatoR.Components.Iso14443TypeBRxTxBaudRate();

                ConsoleWriter.Instance.PrintMessage("Set Iso 14443 B Enable");

                string input = command.SetApdu(enabledBaudRates);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                PrintCommand("Enable all available baud rates", input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetRxTxBaudRate(reader);
                                break;

                            case RequestType.Set:
                                var selectedBaudRates = new BaudRate()
                                {
                                    Rx106 = true,
                                    Rx212 = true,
                                    Rx424 = true,
                                    Rx848 = true,
                                    Tx106 = true,
                                    Tx212 = true,
                                    Tx424 = true,
                                    Tx848 = true
                                };

                                SetRxTxBaudRate(reader, selectedBaudRates);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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

    public class FelicaConfigurationSample
    {
        private static void PrintCommand(string name, string input, string output, IEnumerable<string> value)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
            if (value != null)
            {
                ConsoleWriter.Instance.PrintMessage("Output data:");
                foreach (var data in value)
                {
                    ConsoleWriter.Instance.PrintMessage($"\t{data}");
                }
            }
        }
        private static void PrintCommand(string name, string input, string output, string value)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
            if (!string.IsNullOrWhiteSpace(value))
            {
                ConsoleWriter.Instance.PrintMessage($"Output data: {value}");
            }
        }
        public class CredentialEnable
        {
            private void GetCredentialEnable(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.FelicaEnable();

                ConsoleWriter.Instance.PrintMessage("Get Felica Enable");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = command.TranslateGetResponse(output);

                PrintCommand(string.Empty, input, output, value);
            }
            private void SetCredentialEnable(ISmartCardReader smartCardReader, bool setEnable)
            {
                var command = new Readers.AViatoR.Components.FelicaEnable();

                ConsoleWriter.Instance.PrintMessage("Set Felica Enable");

                string input = command.SetApdu(setEnable);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                PrintCommand(string.Empty, input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetCredentialEnable(reader);
                                break;

                            case RequestType.Set:
                                SetCredentialEnable(reader, true);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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
        public class RxTxBaudRate
        {
            private void GetRxTxBaudRate(ISmartCardReader smartCardReader)
            {
                var command = new Readers.AViatoR.Components.FelicaRxTxBaudRate();

                ConsoleWriter.Instance.PrintMessage("Get Felica Baud Rate");

                string input = command.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                var value = command.TranslateGetResponse(output);

                PrintCommand("Felica RxTx Baud Rates", input, output, value);
            }
            private void SetRxTxBaudRate(ISmartCardReader smartCardReader, BaudRate enabledBaudRates)
            {
                var command = new Readers.AViatoR.Components.FelicaRxTxBaudRate();

                ConsoleWriter.Instance.PrintMessage("Set Felica Enable");

                string input = command.SetApdu(enabledBaudRates);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                PrintCommand("Enable all available baud rates", input, output, string.Empty);
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

                        switch (requestType)
                        {
                            case RequestType.Get:
                                GetRxTxBaudRate(reader);
                                break;

                            case RequestType.Set:
                                var selectedBaudRates = new BaudRate()
                                {
                                    Rx106 = true,
                                    Rx212 = true,
                                    Rx424 = true,
                                    Rx848 = true,
                                    Tx106 = true,
                                    Tx212 = true,
                                    Tx424 = true,
                                    Tx848 = true
                                };

                                SetRxTxBaudRate(reader, selectedBaudRates);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                        }

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






    static class ReaderContactlessSlotConfigurationSample
    {
        private static void PrintData(string title, string command, string response, string data)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"<-- {command}\n--> {response}\n{title}: {data}");
        }

        private static void PrintData(string title, string command, string response, string[] data)
        {
            if (data == null || data.Length <= 0)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"<-- {command}\n--> {response}\n{title}: n/a");
            }
            else if (data.Length == 1)
                PrintData(title, command, response, data[0]);
            else
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine(
                    $"<-- {command}\n--> {response}\n{title}:\n\t{data.ToList().Aggregate((i, j) => i + "\n\t" + j)}");
            }
        }

        private static ISmartCardReader Connect(string readerName)
        {
            var reader = new SmartCardReader(readerName);

            var readerState = ContextHandler.Instance.GetReaderState(reader.PcscReaderName, ReaderStates.Unaware);
            if (readerState.AtrLength > 0)
                reader.Connect(ReaderSharingMode.Shared, Protocol.Any);
            else
                reader.ConnectDirect();

            return reader;
        }

        // ISO 14443 Type A
        public static void ReadIso14443TypeAConfiguration(string readerName)
        {
            var iso14443TypeAEnable = new Readers.AViatoR.Components.Iso14443TypeAEnable();
            var iso14443TypeARxTxBaudRate = new Readers.AViatoR.Components.Iso14443TypeARxTxBaudRate();
            var mifareKeyCache = new Readers.AViatoR.Components.MifareKeyCache();
            var mifarePreferred = new Readers.AViatoR.Components.MifarePreferred();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            string command;
            string response;

            // Read ISO 14443 Type A enable
            command = iso14443TypeAEnable.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("ISO 14443 Type A", command, response, iso14443TypeAEnable.TranslateGetResponse(response));

            // Read ISO 14443 Type A Rx Tx Baud Rates 
            command = iso14443TypeARxTxBaudRate.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            BaudRate baudRates = iso14443TypeARxTxBaudRate.TranslateGetResponse(response);
            string[] data = new string[]
            {
                $"Rx 106 kbps: {baudRates.Rx106}, Tx 106 kbps: {baudRates.Tx106}",
                $"Rx 212 kbps: {baudRates.Rx212}, Tx 212 kbps: {baudRates.Tx212}",
                $"Rx 424 kbps: {baudRates.Rx424}, Tx 424 kbps: {baudRates.Tx424}",
                $"Rx 848 kbps: {baudRates.Rx848}, Tx 848 kbps: {baudRates.Tx848}",
            };
            PrintData("ISO 14443 Type A RxTx Baud Rates", command, response, data);

            // Read Mifare Key Cache
            command = mifareKeyCache.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Mifare Key Cache", command, response, mifareKeyCache.TranslateGetResponse(response));

            // Read Mifare Preferred
            command = mifarePreferred.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Mifare Preferred", command, response, mifarePreferred.TranslateGetResponse(response));

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetIso14443TypeAEnable(string readerName)
        {
            var iso14443TypeAEnable = new Readers.AViatoR.Components.Iso14443TypeAEnable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = iso14443TypeAEnable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set ISO 14443 Type A", command, response, "Enable");

            // disable
            // string command = iso14443TypeAEnable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set ISO 14443 Type A", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetIso14443TypeARxTxBaudRate(string readerName)
        {
            var iso14443TypeARxTxBaudRate = new Readers.AViatoR.Components.Iso14443TypeARxTxBaudRate();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // set true to enable, false to disable, Rx106 and Tx106 are always available and cannot be disabled
            var baudRates = new BaudRate()
            {
                Rx212 = true,
                Rx424 = true,
                Rx848 = true,
                Tx212 = true,
                Tx424 = true,
                Tx848 = true,
            };

            string command = iso14443TypeARxTxBaudRate.SetApdu(baudRates);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set ISO 14443 Type A RxTx Baud Rates", command, response, "");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetMifareKeyCache(string readerName)
        {
            var mifareKeyCache = new Readers.AViatoR.Components.MifareKeyCache();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = mifareKeyCache.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Mifare Key Cache", command, response, "Enable");

            // disable
            // string command = mifareKeyCache.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Mifare Key Cache", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetMifarePreferred(string readerName)
        {
            var mifarePreferred = new Readers.AViatoR.Components.MifarePreferred();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = mifarePreferred.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Mifate Preferred", command, response, "Enable");

            // disable
            // string command = mifarePreferred.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Mifate Preferred", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        // ISO 14443 Type B
        public static void ReadIso14443TypeBConfiguration(string readerName)
        {
            var iso14443TypeBEnable = new Readers.AViatoR.Components.Iso14443TypeBEnable();
            var iso14443TypeBRxTxBaudRate = new Readers.AViatoR.Components.Iso14443TypeBRxTxBaudRate();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            string command;
            string response;

            // Read ISO 14443 Type B enable
            command = iso14443TypeBEnable.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("ISO 14443 Type B", command, response, iso14443TypeBEnable.TranslateGetResponse(response));

            // Read ISO 14443 Type B Rx Tx Baud Rates 
            command = iso14443TypeBRxTxBaudRate.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            BaudRate baudRates = iso14443TypeBRxTxBaudRate.TranslateGetResponse(response);
            string[] data = new string[]
            {
                $"Rx 106 kbps: {baudRates.Rx106}, Tx 106 kbps: {baudRates.Tx106}",
                $"Rx 212 kbps: {baudRates.Rx212}, Tx 212 kbps: {baudRates.Tx212}",
                $"Rx 424 kbps: {baudRates.Rx424}, Tx 424 kbps: {baudRates.Tx424}",
                $"Rx 848 kbps: {baudRates.Rx848}, Tx 848 kbps: {baudRates.Tx848}",
            };
            PrintData("ISO 14443 Type B RxTx Baud Rates", command, response, data);

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetIso14443TypeBEnable(string readerName)
        {
            var iso14443TypeBEnable = new Readers.AViatoR.Components.Iso14443TypeBEnable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = iso14443TypeBEnable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set ISO 14443 Type B", command, response, "Enable");

            // disable
            // string command = iso14443TypeBEnable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set ISO 14443 Type B", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetIso14443TypeBRxTxBaudRate(string readerName)
        {
            var iso14443TypeBRxTxBaudRate = new Readers.AViatoR.Components.Iso14443TypeBRxTxBaudRate();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // set true to enable, false to disable, Rx106 and Tx106 are always available and cannot be disabled
            var baudRates = new BaudRate()
            {
                Rx212 = true,
                Rx424 = true,
                Rx848 = true,
                Tx212 = true,
                Tx424 = true,
                Tx848 = true,
            };

            string command = iso14443TypeBRxTxBaudRate.SetApdu(baudRates);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set ISO 14443 Type B RxTx Baud Rates", command, response, "");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        // ISO 15693
        public static void ReadIso15693Configuration(string readerName)
        {
            var iso15693Enable = new Readers.AViatoR.Components.Iso15693Enable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // Read ISO 15693 enable
            string command = iso15693Enable.GetApdu;
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("ISO 15693", command, response, iso15693Enable.TranslateGetResponse(response));

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetIso15693Enable(string readerName)
        {
            var iso15693Enable = new Readers.AViatoR.Components.Iso15693Enable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = iso15693Enable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set ISO 15693", command, response, "Enable");

            // disable
            // string command = iso15693Enable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set ISO 15693", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        // Felica
        public static void ReadFelicaConfiguration(string readerName)
        {
            var felicaEnable = new Readers.AViatoR.Components.FelicaEnable();
            var felicaRxTxBaudRate = new Readers.AViatoR.Components.FelicaRxTxBaudRate();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            string command;
            string response;

            // Read Felica enable
            command = felicaEnable.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Felica", command, response, felicaEnable.TranslateGetResponse(response));

            // Read Felica Rx Tx Baud Rates 
            command = felicaRxTxBaudRate.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            BaudRate baudRates = felicaRxTxBaudRate.TranslateGetResponse(response);
            string[] data = new string[]
            {
                $"Rx 106 kbps: {baudRates.Rx106}, Tx 106 kbps: {baudRates.Tx106}",
                $"Rx 212 kbps: {baudRates.Rx212}, Tx 212 kbps: {baudRates.Tx212}",
                $"Rx 424 kbps: {baudRates.Rx424}, Tx 424 kbps: {baudRates.Tx424}",
                $"Rx 848 kbps: {baudRates.Rx848}, Tx 848 kbps: {baudRates.Tx848}",
            };
            PrintData("Felica RxTx Baud Rates", command, response, data);

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetFelicaEnable(string readerName)
        {
            var felicaEnable = new Readers.AViatoR.Components.FelicaEnable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = felicaEnable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Felica", command, response, "Enable");

            // disable
            // string command = felicaEnable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Felica", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetFelicaRxTxBaudRate(string readerName)
        {
            var felicaRxTxBaudRate = new Readers.AViatoR.Components.FelicaRxTxBaudRate();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // set true to enable, false to disable, Rx106 and Tx106 are always available and cannot be disabled
            var baudRates = new BaudRate()
            {
                Rx212 = true,
                Rx424 = true,
                Rx848 = true,
                Tx212 = true,
                Tx424 = true,
                Tx848 = true,
            };

            string command = felicaRxTxBaudRate.SetApdu(baudRates);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Felica RxTx Baud Rates", command, response, "");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        // iClass
        public static void ReadIclass15693Configuration(string readerName)
        {
            var iclass15693Enable = new Readers.AViatoR.Components.iClass15693Enable();
            
            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // Read iclass 15693 enable
            string command = iclass15693Enable.GetApdu;
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("iClass 15693 Enable", command, response, iclass15693Enable.TranslateGetResponse(response));

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetIclass15693Enable(string readerName)
        {
            var iclass15693Enable = new Readers.AViatoR.Components.iClass15693Enable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = iclass15693Enable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct
                ? smartCardReader.Transmit(command)
                : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set iClass 15693", command, response, "Enable");

            // disable
            // string command = iclass15693Enable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set iClass 15693", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }
       
        // Contactless Common
        public static void SetEmdSupressionEnable(string readerName)
        {
            var emdSupressionEnable = new Readers.AViatoR.Components.EmdSuppresionEnable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = emdSupressionEnable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set EMD Supression", command, response, "Enable");

            // disable
            // string command = emdSupressionEnable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set EMD Supression", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }
        public static void SetPollingRfModuleEnable(string readerName)
        {
            var pollingRfModuleEnable = new Readers.AViatoR.Components.PollingRFModuleEnable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = pollingRfModuleEnable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Polling RF Module", command, response, "Enable");

            // disable
            // string command = pollingRfModuleEnable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Polling RF Module", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }
        public static void SetSleepModeCardDetectionEnable(string readerName)
        {
            var sleepModeCardDetectionEnable = new Readers.AViatoR.Components.SleepModeCardDetectionEnable();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // enable
            string command = sleepModeCardDetectionEnable.SetApdu(true);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Sleep Mode Card Detection", command, response, "Enable");

            // disable
            // string command = sleepModeCardDetectionEnable.SetApdu(false);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Sleep Mode Card Detection", command, response, "Disable");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }
        public static void SetSleepModePollingFrequency(string readerName)
        {
            var sleepModePollingFrequencyEnable = new Readers.AViatoR.Components.SleepModePollingFrequency();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // Set 1.3 Hz polling frequency
            string command = sleepModePollingFrequencyEnable.SetApdu(PollingFrequency.T800ms);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Sleep Mode Polling Frequency", command, response, "1.3 Hz");

            // Set 5 Hz polling frequency
            // string command = sleepModePollingFrequencyEnable.SetApdu(PollingFrequency.T200ms);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Sleep Mode Polling Frequency", command, response, "5 Hz");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }
        public static void SetPollingSearchOrder(string readerName)
        {
            var pollingSearchOrder = new Readers.AViatoR.Components.PollingSearchOrderConfig();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // Set serch order : ISO 14443a, ISO 14443b, iClass, Felica, ISO 15693
            string command = pollingSearchOrder.SetApdu(PollingSearchOrder.Iso14443a, PollingSearchOrder.Iso14443b, PollingSearchOrder.Iclass, PollingSearchOrder.Felica, PollingSearchOrder.Iso15693);
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Polling Search Order", command, response, "ISO 14443a, ISO 14443b, iClass, Felica, ISO 15693");

            // Set serch order : ISO 14443a, none, none, none, none , be advice that reader will not recognize cards that are not included in search order
            // string command = pollingSearchOrder.SetApdu(PollingSearchOrder.Iso14443a);
            // string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Polling Search Order", command, response, "ISO 14443a, none, none, none, none");

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadOK5022CommonConfiguration(string readerName)
        {
            var emdSupressionEnable = new Readers.AViatoR.Components.EmdSuppresionEnable();
            var sleepModeCardDetectionEnable = new Readers.AViatoR.Components.SleepModeCardDetectionEnable();
            var pollingSearchOrder = new Readers.AViatoR.Components.PollingSearchOrderConfig();
            var sleepModePollingFrequency = new Readers.AViatoR.Components.SleepModePollingFrequency();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // Read EMD Suppresion Enable
            string command = emdSupressionEnable.GetApdu;
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("EMD Supression", command, response, emdSupressionEnable.TranslateGetResponse(response));

            // Read Sleep Mode Card Detection Enable
            command = sleepModeCardDetectionEnable.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Sleep Mode Card Detection", command, response, sleepModeCardDetectionEnable.TranslateGetResponse(response));

            // Read Sleep Mode Polling Frequency
            command = sleepModePollingFrequency.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);

            string data = string.Empty;
            switch (sleepModePollingFrequency.TranslateGetResponse(response))
            {
                case PollingFrequency.T24ms:
                    data = "41 Hz";
                    break;
                case PollingFrequency.T48ms:
                    data = "20 Hz";
                    break;
                case PollingFrequency.T96ms:
                    data = "10 Hz";
                    break;
                case PollingFrequency.T200ms:
                    data = "5 Hz";
                    break;
                case PollingFrequency.T400ms:
                    data = "2.5 Hz";
                    break;
                case PollingFrequency.T800ms:
                    data = "1.3 Hz";
                    break;
                case PollingFrequency.T1400ms:
                    data = "0.7 Hz";
                    break;
                case PollingFrequency.T3100ms:
                    data = "0.3 Hz";
                    break;
                case PollingFrequency.T6200ms:
                    data = "0.15 Hz";
                    break;
                case PollingFrequency.T12300ms:
                    data = "0.08 Hz";
                    break;
            }
            PrintData("Sleep Mode Polling Frequency", command, response, data);
            
            // Read Polling Search Order
            command = pollingSearchOrder.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            var responseData = new List<string>();
            foreach (PollingSearchOrder element in pollingSearchOrder.TranslateGetResponse(response))
            {
                responseData.Add(element.ToString());
            }

            PrintData("PollingSearchOrder", command, response, responseData.ToArray());

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadOK5023CommonConfiguration(string readerName)
        {
            var emdSupressionEnable = new Readers.AViatoR.Components.EmdSuppresionEnable();
            var pollingSearchOrder = new Readers.AViatoR.Components.PollingSearchOrderConfig();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // Read EMD Suppresion Enable
            string command = emdSupressionEnable.GetApdu;
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("EMD Supression", command, response, emdSupressionEnable.TranslateGetResponse(response));

            // Read Polling Search Order
            command = pollingSearchOrder.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            var responseData = new List<string>();
            foreach (PollingSearchOrder element in pollingSearchOrder.TranslateGetResponse(response))
            {
                responseData.Add(element.ToString());
            }

            PrintData("PollingSearchOrder", command, response, responseData.ToArray());

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadOK5422CommonConfiguration(string readerName)
        {
            var pollingRfModuleEnable = new Readers.AViatoR.Components.PollingRFModuleEnable();
            var emdSupressionEnable = new Readers.AViatoR.Components.EmdSuppresionEnable();
            var sleepModeCardDetectionEnable = new Readers.AViatoR.Components.SleepModeCardDetectionEnable();
            var sleepModePollingFrequency = new Readers.AViatoR.Components.SleepModePollingFrequency();

            ISmartCardReader smartCardReader = Connect(readerName);

            if (!smartCardReader.IsConnected)
                return;

            // Read Polling RF Module Enable
            string command = pollingRfModuleEnable.GetApdu;
            string response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Polling RF Module", command, response, pollingRfModuleEnable.TranslateGetResponse(response));

            // Read EMD Suppresion Enable
            command = emdSupressionEnable.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("EMD Supression", command, response, emdSupressionEnable.TranslateGetResponse(response));

            // Read Sleep Mode Card Detection Enable
            command = sleepModeCardDetectionEnable.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Sleep Mode Card Detection", command, response, sleepModeCardDetectionEnable.TranslateGetResponse(response));

            // Read Sleep Mode Polling Frequency
            command = sleepModePollingFrequency.GetApdu;
            response = smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);

            string data = string.Empty;
            switch (sleepModePollingFrequency.TranslateGetResponse(response))
            {
                case PollingFrequency.T24ms:
                    data = "41 Hz";
                    break;
                case PollingFrequency.T48ms:
                    data = "20 Hz";
                    break;
                case PollingFrequency.T96ms:
                    data = "10 Hz";
                    break;
                case PollingFrequency.T200ms:
                    data = "5 Hz";
                    break;
                case PollingFrequency.T400ms:
                    data = "2.5 Hz";
                    break;
                case PollingFrequency.T800ms:
                    data = "1.3 Hz";
                    break;
                case PollingFrequency.T1400ms:
                    data = "0.7 Hz";
                    break;
                case PollingFrequency.T3100ms:
                    data = "0.3 Hz";
                    break;
                case PollingFrequency.T6200ms:
                    data = "0.15 Hz";
                    break;
                case PollingFrequency.T12300ms:
                    data = "0.08 Hz";
                    break;
            }
            PrintData("Sleep Mode Polling Frequency", command, response, data);

            smartCardReader.Disconnect(CardDisposition.Unpower);
        }

    }
}