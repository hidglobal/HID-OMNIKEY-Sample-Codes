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
using System.Linq;
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.Components;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    static class ReaderCapabilitiesSample
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
                Console.WriteLine($"<-- {command}\n--> {response}\n{title}:\n\t{data.ToList().Aggregate((i, j) => i + "\n\t" + j)}");
            }
        }
        private static IReader Connect(string readerName)
        {
            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (readerState.AtrLength > 0)
                reader.Connect(ReaderSharingMode.Shared, Protocol.Any);
            else
                reader.ConnectDirect();

            return reader;
        }
        public static void ReadTlvVersion(string readerName)
        {
            var tlvVersion = new Readers.AViatoR.Components.ReaderCapabilities().TlvVersion;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = tlvVersion.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Tlv Version", command, response, tlvVersion.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadDeviceId(string readerName)
        {
            var deviceId = new Readers.AViatoR.Components.ReaderCapabilities().DeviceId;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = deviceId.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Device ID", command, response, deviceId.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadProductName(string readerName)
        {
            var productName = new Readers.AViatoR.Components.ReaderCapabilities().ProductName;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = productName.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Product Name", command, response, productName.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadProductPlatform(string readerName)
        {
            var productPlatform = new Readers.AViatoR.Components.ReaderCapabilities().ProductPlatform;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = productPlatform.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Product Platform", command, response, productPlatform.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadEnabledClFeatures(string readerName)
        {
            var enabledClFetures = new Readers.AViatoR.Components.ReaderCapabilities().EnabledClFeatures;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = enabledClFetures.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Enabled CL Features", command, response, enabledClFetures.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadFirmwareVersion(string readerName)
        {
            var firmwareVersion = new Readers.AViatoR.Components.ReaderCapabilities().FirmwareVersion;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = firmwareVersion.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Firmware Version", command, response, firmwareVersion.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadHfControllerVersion(string readerName)
        {
            var hfControllerVersion = new Readers.AViatoR.Components.ReaderCapabilities().HfControllerVersion;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = hfControllerVersion.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("HF Controller Version", command, response, hfControllerVersion.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadHardwareVersion(string readerName)
        {
            var hardwareVersion = new Readers.AViatoR.Components.ReaderCapabilities().HardwareVersion;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = hardwareVersion.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Hardware Version", command, response, hardwareVersion.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadHostInterfaces(string readerName)
        {
            var hostInterfaces = new Readers.AViatoR.Components.ReaderCapabilities().HostInterfaces;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = hostInterfaces.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Host Interfaces", command, response, hostInterfaces.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadNumberOfContactSlots(string readerName)
        {
            var numberOfContactSlots = new Readers.AViatoR.Components.ReaderCapabilities().NumberOfContactSlots;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = numberOfContactSlots.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Number of Contact Slots", command, response, numberOfContactSlots.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadNumberOfContactlessSlots(string readerName)
        {
            var numberOfContactlessSlots = new Readers.AViatoR.Components.ReaderCapabilities().NumberOfContactlessSlots;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = numberOfContactlessSlots.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Number of Contactless Slots", command, response, numberOfContactlessSlots.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadNumberOfAntennas(string readerName)
        {
            var numberOfAntennas = new Readers.AViatoR.Components.ReaderCapabilities().NumberOfAntennas;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = numberOfAntennas.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Number of Antennas", command, response, numberOfAntennas.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadHumanInterfaces(string readerName)
        {
            var humanInterfaces = new Readers.AViatoR.Components.ReaderCapabilities().HumanInterfaces;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = humanInterfaces.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Human Interfaces", command, response, humanInterfaces.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadVendorName(string readerName)
        {
            var vendorName = new Readers.AViatoR.Components.ReaderCapabilities().VendorName;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = vendorName.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Vendor Name", command, response, vendorName.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadExchangeLevel(string readerName)
        {
            var exchangeLevel = new Readers.AViatoR.Components.ReaderCapabilities().ExchangeLevel;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = exchangeLevel.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Exchange Level", command, response, exchangeLevel.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadSerialNumber(string readerName)
        {
            var serialNumber = new Readers.AViatoR.Components.ReaderCapabilities().SerialNumber;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = serialNumber.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Serial Number", command, response, serialNumber.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadHfControllerType(string readerName)
        {
            var hfControllerType = new Readers.AViatoR.Components.ReaderCapabilities().HfControllerType;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = hfControllerType.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("HF Controller Type", command, response, hfControllerType.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadSizeOfUserEeprom(string readerName)
        {
            var sizeOfUserEeprom = new Readers.AViatoR.Components.ReaderCapabilities().SizeOfUserEEPROM;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = sizeOfUserEeprom.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Size of User EEPROM", command, response, sizeOfUserEeprom.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ReadFirmwareLabel(string readerName)
        {
            var firmwareLabel = new Readers.AViatoR.Components.ReaderCapabilities().FirmwareLabel;

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = firmwareLabel.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Firmware Label", command, response, firmwareLabel.TranslateResponse(response));

            reader.Disconnect(CardDisposition.Unpower);
        }
    }
}

