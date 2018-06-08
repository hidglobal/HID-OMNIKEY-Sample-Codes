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
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    public class ReaderCapabilitiesSample
    {
        private static void PrintCommand(string name, string input, string output, string[] value)
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

        public class TlvVersion
        {
            private void ReadTlvVersionCommand(ISmartCardReader smartCardReader)
            {
                var tlvVersion = new Readers.AViatoR.Components.TlvVersion();

                ConsoleWriter.Instance.PrintMessage("Get Tlv Version");

                string input = tlvVersion.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = tlvVersion.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadTlvVersionCommand(reader);

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
        public class DeviceId
        {
            private void ReadDeviceIdCommand(ISmartCardReader smartCardReader)
            {
                var deviceId = new Readers.AViatoR.Components.DeviceId();

                ConsoleWriter.Instance.PrintMessage("Get Device ID");

                string input = deviceId.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = deviceId.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadDeviceIdCommand(reader);

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
        public class ProductName
        {
            private void ReadProductNameCommand(ISmartCardReader smartCardReader)
            {
                var productName = new Readers.AViatoR.Components.ProductName();

                ConsoleWriter.Instance.PrintMessage("Get Product Name");

                string input = productName.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = productName.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadProductNameCommand(reader);

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
        public class ProductPlatform
        {
            private void ReadProductPlatformCommand(ISmartCardReader smartCardReader)
            {
                var productPlatform = new Readers.AViatoR.Components.ProductPlatform();

                ConsoleWriter.Instance.PrintMessage("Get Product Platform");

                string input = productPlatform.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = productPlatform.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadProductPlatformCommand(reader);

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
        public class EnabledClFeatures
        {
            private void ReadEnabledClFeaturesCommand(ISmartCardReader smartCardReader)
            {
                var enabledClFeatures = new Readers.AViatoR.Components.EnabledClFeatures();

                ConsoleWriter.Instance.PrintMessage("Get Enabled CL Features");

                string input = enabledClFeatures.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string[] value = enabledClFeatures.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadEnabledClFeaturesCommand(reader);

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
        public class FirmwareVersion
        {
            private void ReadFirmwareVersionCommand(ISmartCardReader smartCardReader)
            {
                var firmwareVersion = new Readers.AViatoR.Components.FirmwareVersion();

                ConsoleWriter.Instance.PrintMessage("Get Firmware Version");

                string input = firmwareVersion.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = firmwareVersion.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadFirmwareVersionCommand(reader);

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
        public class HfControllerVersion
        {
            private void ReadHfControllerVersionCommand(ISmartCardReader smartCardReader)
            {
                var hfControllerVersion = new Readers.AViatoR.Components.HfControllerVersion();

                ConsoleWriter.Instance.PrintMessage("Get HF Controller Version");

                string input = hfControllerVersion.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = hfControllerVersion.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadHfControllerVersionCommand(reader);

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
        public class HardwareVersion
        {
            private void ReadHardwareVersionCommand(ISmartCardReader smartCardReader)
            {
                var hardwareVersion = new Readers.AViatoR.Components.HardwareVersion();

                ConsoleWriter.Instance.PrintMessage("Get Hardware Version");

                string input = hardwareVersion.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = hardwareVersion.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadHardwareVersionCommand(reader);

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
        public class HostInterfaces
        {
            private void ReadHostInterfacesCommand(ISmartCardReader smartCardReader)
            {
                var hostInterfaces = new Readers.AViatoR.Components.HostInterfaces();

                ConsoleWriter.Instance.PrintMessage("Get Host Interfaces");

                string input = hostInterfaces.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string[] value = hostInterfaces.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadHostInterfacesCommand(reader);

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
        public class NumberOfContactSlots
        {
            private void ReadNumberOfContactSlotsCommand(ISmartCardReader smartCardReader)
            {
                var numberOfContactSlots = new Readers.AViatoR.Components.NumberOfContactSlots();

                ConsoleWriter.Instance.PrintMessage("Get Number Of Contact Slots");

                string input = numberOfContactSlots.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = numberOfContactSlots.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadNumberOfContactSlotsCommand(reader);

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
        public class NumberOfContactlessSlots
        {
            private void ReadNumberOfContactlessSlotsCommand(ISmartCardReader smartCardReader)
            {
                var numberOfContactlessSlots = new Readers.AViatoR.Components.NumberOfContactlessSlots();

                ConsoleWriter.Instance.PrintMessage("Get Number Of Contactless Slots");

                string input = numberOfContactlessSlots.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = numberOfContactlessSlots.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadNumberOfContactlessSlotsCommand(reader);

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
        public class NumberOfAntennas
        {
            private void ReadNumberOfAntennasCommand(ISmartCardReader smartCardReader)
            {
                var numberOfAntennas = new Readers.AViatoR.Components.NumberOfAntennas();

                ConsoleWriter.Instance.PrintMessage("Get Number Of Antennas");

                string input = numberOfAntennas.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = numberOfAntennas.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadNumberOfAntennasCommand(reader);

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
        public class VendorName
        {
            private void ReadVendorNameCommand(ISmartCardReader smartCardReader)
            {
                var vendorName = new Readers.AViatoR.Components.VendorName();

                ConsoleWriter.Instance.PrintMessage("Get Vendor Name");

                string input = vendorName.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = vendorName.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadVendorNameCommand(reader);

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
        public class ExchangeLevel
        {
            private void ReadExchangeLevelCommand(ISmartCardReader smartCardReader)
            {
                var exchangeLevel = new Readers.AViatoR.Components.ExchangeLevel();

                ConsoleWriter.Instance.PrintMessage("Get Exchange Level");

                string input = exchangeLevel.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = exchangeLevel.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadExchangeLevelCommand(reader);

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
        public class SerialNumber
        {
            private void ReadSerialnumberCommand(ISmartCardReader smartCardReader)
            {
                var serialNumber = new Readers.AViatoR.Components.SerialNumber();

                ConsoleWriter.Instance.PrintMessage("Get Serial Number");

                string input = serialNumber.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = serialNumber.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadSerialnumberCommand(reader);

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
        public class HfControllerType
        {
            private void ReadHfControllerTypeCommand(ISmartCardReader smartCardReader)
            {
                var hfControllerType = new Readers.AViatoR.Components.HfControllerType();

                ConsoleWriter.Instance.PrintMessage("Get HF Controller Type");

                string input = hfControllerType.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = hfControllerType.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadHfControllerTypeCommand(reader);

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
        public class SizeOfUserEeprom
        {
            private void ReadSizeOfUserEepromCommand(ISmartCardReader smartCardReader)
            {
                var sizeOfUserEeprom = new Readers.AViatoR.Components.SizeOfUserEEPROM();

                ConsoleWriter.Instance.PrintMessage("Get Size of User EEPROM");

                string input = sizeOfUserEeprom.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = sizeOfUserEeprom.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadSizeOfUserEepromCommand(reader);

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
        public class FirmwareLabel
        {
            private void ReadFirmwareLabelCommand(ISmartCardReader smartCardReader)
            {
                var firmwareLabel = new Readers.AViatoR.Components.FirmwareLabel();

                ConsoleWriter.Instance.PrintMessage("Get Firmware Label");

                string input = firmwareLabel.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string value = firmwareLabel.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadFirmwareLabelCommand(reader);

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
        public class HumanInterfaces
        {
            private void ReadHumanInterfacesCommand(ISmartCardReader smartCardReader)
            {
                var humanInterfaces = new Readers.AViatoR.Components.HumanInterfaces();

                ConsoleWriter.Instance.PrintMessage("Get Human Interfaces");

                string input = humanInterfaces.GetApdu;
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                string[] value = humanInterfaces.TranslateResponse(output);

                PrintCommand(string.Empty, input, output, value);
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

                        ReadHumanInterfacesCommand(reader);

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

