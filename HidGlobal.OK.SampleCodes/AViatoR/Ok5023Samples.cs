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
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    class Ok5023Samples
    {
        private string _readerName;
        public Menu Menu { get; private set; }

        public Ok5023Samples(string readerName)
        {
            _readerName = readerName;
            CreateMenu();
        }

        private void CreateMenu()
        {
            var readerCapabilities = new Menu("Reader Capabilities", true)
            {
                new MenuEntry("Tlv Version",                 () => new ReaderCapabilitiesSample.TlvVersion().Run(_readerName),               WaitKeyPress),
                new MenuEntry("Device ID",                   () => new ReaderCapabilitiesSample.DeviceId().Run(_readerName),                 WaitKeyPress),
                new MenuEntry("Product Name",                () => new ReaderCapabilitiesSample.ProductName().Run(_readerName),              WaitKeyPress),
                new MenuEntry("Product Platform",            () => new ReaderCapabilitiesSample.ProductPlatform().Run(_readerName),          WaitKeyPress),
                new MenuEntry("Enabled CL Feaures",          () => new ReaderCapabilitiesSample.EnabledClFeatures().Run(_readerName),        WaitKeyPress),
                new MenuEntry("Firmware Version",            () => new ReaderCapabilitiesSample.FirmwareVersion().Run(_readerName),          WaitKeyPress),
                new MenuEntry("HF Controller Version",       () => new ReaderCapabilitiesSample.HfControllerVersion().Run(_readerName),      WaitKeyPress),
                new MenuEntry("Hardware Version",            () => new ReaderCapabilitiesSample.HardwareVersion().Run(_readerName),          WaitKeyPress),
                new MenuEntry("Host Interfaces",             () => new ReaderCapabilitiesSample.HostInterfaces().Run(_readerName),           WaitKeyPress),
                new MenuEntry("Number of Contact Slots",     () => new ReaderCapabilitiesSample.NumberOfContactSlots().Run(_readerName),     WaitKeyPress),
                new MenuEntry("Number of Contactless Slots", () => new ReaderCapabilitiesSample.NumberOfContactlessSlots().Run(_readerName), WaitKeyPress),
                new MenuEntry("Number of Antennas",          () => new ReaderCapabilitiesSample.NumberOfAntennas().Run(_readerName),         WaitKeyPress),
                new MenuEntry("Vendor Name",                 () => new ReaderCapabilitiesSample.VendorName().Run(_readerName),               WaitKeyPress),
                new MenuEntry("Exchange Level",              () => new ReaderCapabilitiesSample.ExchangeLevel().Run(_readerName),            WaitKeyPress),
                new MenuEntry("Serial Number",               () => new ReaderCapabilitiesSample.SerialNumber().Run(_readerName),             WaitKeyPress),
                new MenuEntry("HF Controller Type",          () => new ReaderCapabilitiesSample.HfControllerType().Run(_readerName),         WaitKeyPress),
                new MenuEntry("Size of User EEPROM",         () => new ReaderCapabilitiesSample.SizeOfUserEeprom().Run(_readerName),         WaitKeyPress),
                new MenuEntry("Firmware Label",              () => new ReaderCapabilitiesSample.FirmwareLabel().Run(_readerName),            WaitKeyPress),
            };
            var userEeprom = new Menu("User EEPROM", true)
            {
                new MenuEntry("Read Example",  () => new ReaderEepromSample.ReadEeprom().Run(_readerName),  WaitKeyPress),
                new MenuEntry("Write Example", () => new ReaderEepromSample.WriteEeprom().Run(_readerName), WaitKeyPress),
            };
            var configurationControl = new Menu("Configuration Control", true)
            {
                new MenuEntry("Reboot Reader",            () => new ReaderConfigurationControlSample.RebootDevice().Run(_readerName),           WaitKeyPress),
                new MenuEntry("Restore Factory Defaults", () => new ReaderConfigurationControlSample.RestoreFactoryDefaults().Run(_readerName), WaitKeyPress),
                new MenuEntry("Apply Settings",           () => new ReaderConfigurationControlSample.ApplySettings().Run(_readerName),          WaitKeyPress),
            };
            var iso14443a = new Menu("ISO 14443 Type A", true)
            {
                new MenuEntry("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadIso14443TypeAConfiguration(_readerName), WaitKeyPress),
                new MenuEntry("Set ISO 14443 Type A", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeAEnable(_readerName), WaitKeyPress),
                new MenuEntry("Set ISO 14443 Type A RxTx Baud Rates", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeARxTxBaudRate(_readerName), WaitKeyPress),
                new MenuEntry("Set Mifare Key Cache", () => ReaderContactlessSlotConfigurationSample.SetMifareKeyCache(_readerName), WaitKeyPress),
                new MenuEntry("Set Mifare Preferred", () => ReaderContactlessSlotConfigurationSample.SetMifarePreferred(_readerName), WaitKeyPress),
            };
            var iso14443b = new Menu("ISO 14443 Type B", true)
            {
                new MenuEntry("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadIso14443TypeBConfiguration(_readerName), WaitKeyPress),
                new MenuEntry("Set ISO 14443 Type B", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeBEnable(_readerName), WaitKeyPress),
                new MenuEntry("Set ISO 14443 Type B RxTx Baud Rates", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeBRxTxBaudRate(_readerName), WaitKeyPress),
            };
            var felica = new Menu("Felica", true)
            {
                new MenuEntry("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadFelicaConfiguration(_readerName), WaitKeyPress),
                new MenuEntry("Set Felica", () => ReaderContactlessSlotConfigurationSample.SetFelicaEnable(_readerName), WaitKeyPress),
                new MenuEntry("Set Felica RxTx Baud Rates", () => ReaderContactlessSlotConfigurationSample.SetFelicaRxTxBaudRate(_readerName), WaitKeyPress),
            };
            var iclass15693 = new Menu("iClass 15693", true)
            {
                new MenuEntry("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadIclass15693Configuration(_readerName), WaitKeyPress),
                new MenuEntry("Set iClass 15693 Enable", () => ReaderContactlessSlotConfigurationSample.SetIclass15693Enable(_readerName), WaitKeyPress),
            };
            var contactlessCommon = new Menu("Contactless Common", true)
            {
                new MenuEntry("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadOK5023CommonConfiguration(_readerName), WaitKeyPress),
                new MenuEntry("Set EMD Supression", () => ReaderContactlessSlotConfigurationSample.SetEmdSupressionEnable(_readerName), WaitKeyPress),
                new MenuEntry("Set Polling Search Order", () => ReaderContactlessSlotConfigurationSample.SetPollingSearchOrder(_readerName), WaitKeyPress),
            };
            var contactlessSlotConfiguration = new Menu("Contactless Slot Configuration", true)
            {
                new MenuEntry("ISO 14443 Type A Configuration", iso14443a),
                new MenuEntry("ISO 14443 Type B Configuration", iso14443b),
                new MenuEntry("Felica Configuration", felica),
                new MenuEntry("iClass Configuration", iclass15693),
                new MenuEntry("Contactless Common", contactlessCommon),
            };
            var mifareExamples = new Menu("Mifare Classic 4K Examples", true)
            {
                new MenuEntry("Load key without Secure Session Example", () => new ExampleWithMifareClassic.LoadKeyExample().Run(_readerName),                           WaitKeyPress),
                new MenuEntry("Get Card UID Example",                    () => new GetDataExample().Run(_readerName),                                                    WaitKeyPress),
                new MenuEntry("Get Card Historical Bytes Example",       () => new GetHistoricalBytesExample().Run(_readerName),                                         WaitKeyPress),
                new MenuEntry("Read Example",                            () => new ExampleWithMifareClassic.ReadBinaryMifareClassic1kExample().Run(_readerName),         WaitKeyPress),
                new MenuEntry("Write Example",                           () => new ExampleWithMifareClassic.UpdateBinaryMifareClassic1kExample().Run(_readerName),       WaitKeyPress),
                new MenuEntry("Increment Example",                       () => new ExampleWithMifareClassic.IncrementMifareClassic1kForOK5023Example().Run(_readerName), WaitKeyPress),
                new MenuEntry("Decrement Example",                       () => new ExampleWithMifareClassic.DecrementMifareClassic1kForOK5023Example().Run(_readerName), WaitKeyPress),
            };

            var iclassExamples = new Menu("iCLASS Examples", true)
            {
                new MenuEntry("Get Card CSN Example",     () => new GetDataExample().Run(_readerName),                                       WaitKeyPress),
                new MenuEntry("Read iClass 2ks Example",  () => new ExampleWithiClass.ReadBinaryiClass2ksOK5023Example().Run(_readerName),   WaitKeyPress),
                new MenuEntry("Write iClass 2ks Example", () => new ExampleWithiClass.UpdateBinaryiClass2ksOK5023Example().Run(_readerName), WaitKeyPress),
                new MenuEntry("Read iClass 16k Example",  () => new ExampleWithiClass.ReadBinaryiClass16kOK5023Example().Run(_readerName),   WaitKeyPress),
                new MenuEntry("Write iClass 16k Example", () => new ExampleWithiClass.UpdateBinaryiClass16kOK5023Example().Run(_readerName), WaitKeyPress),
            };

            var contactlessCardExamples = new Menu("Contactless Card Examples", true)
            {
                new MenuEntry("Mifare Classic 4K Examples", mifareExamples),
                new MenuEntry("iCLASS Examples", iclassExamples),
            };

            var desfireExamples = new Menu("Desfire Examples", true)
            {
                new MenuEntry("Format Card Example", () => new SEProcessorSample.DesfireFormatCardExample(_readerName).Run(), WaitKeyPress),
                new MenuEntry("Read/Write Data Example", () => new SEProcessorSample.DesfireReadWriteDataExample(_readerName).Run(), WaitKeyPress),
            };

            var seProcessorExamples = new Menu("Secure Processor Examples", true)
            {
                new MenuEntry("Load key to PCSC containter Example", () => new SEProcessorSample.LoadKeyToPcScContainer().Run(_readerName), WaitKeyPress),
                new MenuEntry("Load Key to SE processor Example", () => new SEProcessorSample.LoadKeyExample(_readerName).Run(), WaitKeyPress),
                new MenuEntry("Read PACS Data Example", () => new SEProcessorSample.ReadPACSDataExample(_readerName).Run(), WaitKeyPress),
                new MenuEntry("Desfire Examples", desfireExamples),
            };
            
            Menu = new Menu($"{_readerName} Menu:", true)
            {
                new MenuEntry("Reader Capabilities", readerCapabilities),
                new MenuEntry("User EEPROM", userEeprom),
                new MenuEntry("Configuration Control", configurationControl),
                new MenuEntry("Contactless Slot Configuration", contactlessSlotConfiguration),
                new MenuEntry("Contactless Card Examples", contactlessCardExamples),
                new MenuEntry("Secure Processor Examples", seProcessorExamples),
            };
        }

        private void WaitKeyPress()
        {
            Console.WriteLine("Press key to continue");
            Console.ReadLine();
        }

    }
}