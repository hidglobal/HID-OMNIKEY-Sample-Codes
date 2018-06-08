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

using HidGlobal.OK.SampleCodes.AViatoR;

namespace HidGlobal.OK.SampleCodes.MenuSections
{
    public class OK5022MenuSection : IMenuSection
    {
        private readonly IMenuItem _rootMenuItem;

        public IMenuItem RootMenuItem
        {
            get { return _rootMenuItem; }
        }

        public OK5022MenuSection(string readerName, string serialNumber)
        {
            var description = string.IsNullOrWhiteSpace(serialNumber)
                ? $"PCSC Reader Name: {readerName}"
                : $"PCSC Reader Name: {readerName}\nSerial Number: {serialNumber}";

            _rootMenuItem = new MenuItem(description);

            var readerCapabilities = _rootMenuItem.AddSubItem("Reader Capabilities");
            {
                readerCapabilities.AddSubItem("Tlv Version", () => new ReaderCapabilitiesSample.TlvVersion().Run(readerName));
                readerCapabilities.AddSubItem("Device ID", () => new ReaderCapabilitiesSample.DeviceId().Run(readerName));
                readerCapabilities.AddSubItem("Product Name", () => new ReaderCapabilitiesSample.ProductName().Run(readerName));
                readerCapabilities.AddSubItem("Product Platform", () => new ReaderCapabilitiesSample.ProductPlatform().Run(readerName));
                readerCapabilities.AddSubItem("Enabled CL Feaures", () => new ReaderCapabilitiesSample.EnabledClFeatures().Run(readerName));
                readerCapabilities.AddSubItem("Firmware Version", () => new ReaderCapabilitiesSample.FirmwareVersion().Run(readerName));
                readerCapabilities.AddSubItem("HF Controller Version", () => new ReaderCapabilitiesSample.HfControllerVersion().Run(readerName));
                readerCapabilities.AddSubItem("Hardware Version", () => new ReaderCapabilitiesSample.HardwareVersion().Run(readerName));
                readerCapabilities.AddSubItem("Host Interfaces", () => new ReaderCapabilitiesSample.HostInterfaces().Run(readerName));
                readerCapabilities.AddSubItem("Number of Contact Slots", () => new ReaderCapabilitiesSample.NumberOfContactSlots().Run(readerName));
                readerCapabilities.AddSubItem("Number of Contactless Slots", () => new ReaderCapabilitiesSample.NumberOfContactlessSlots().Run(readerName));
                readerCapabilities.AddSubItem("Number of Antennas", () => new ReaderCapabilitiesSample.NumberOfAntennas().Run(readerName));
                readerCapabilities.AddSubItem("Human Interfaces", () => new ReaderCapabilitiesSample.HumanInterfaces().Run(readerName));
                readerCapabilities.AddSubItem("Vendor Name", () => new ReaderCapabilitiesSample.VendorName().Run(readerName));
                readerCapabilities.AddSubItem("Exchange Level", () => new ReaderCapabilitiesSample.ExchangeLevel().Run(readerName));
                readerCapabilities.AddSubItem("Serial Number", () => new ReaderCapabilitiesSample.SerialNumber().Run(readerName));
                readerCapabilities.AddSubItem("HF Controller Type", () => new ReaderCapabilitiesSample.HfControllerType().Run(readerName));
                readerCapabilities.AddSubItem("Size of User EEPROM", () => new ReaderCapabilitiesSample.SizeOfUserEeprom().Run(readerName));
                readerCapabilities.AddSubItem("Firmware Label", () => new ReaderCapabilitiesSample.FirmwareLabel().Run(readerName));
            }

            var userEeprom = _rootMenuItem.AddSubItem("User EEPROM");
            {
                userEeprom.AddSubItem("Read Example", () => new ReaderEepromSample.ReadEeprom().Run(readerName));
                userEeprom.AddSubItem("Write Example", () => new ReaderEepromSample.WriteEeprom().Run(readerName));
            }

            var configurationControl = _rootMenuItem.AddSubItem("Configuration Control");
            {
                configurationControl.AddSubItem("Reboot Reader", () => new ReaderConfigurationControlSample.RebootDevice().Run(readerName));
                configurationControl.AddSubItem("Restore Factory Defaults", () => new ReaderConfigurationControlSample.RestoreFactoryDefaults().Run(readerName));
                configurationControl.AddSubItem("Apply Settings", () => new ReaderConfigurationControlSample.ApplySettings().Run(readerName));
            }

            var contactlessSlotConfiguration = _rootMenuItem.AddSubItem("Contactless Slot Configuration");
            {
                var iso14443a = contactlessSlotConfiguration.AddSubItem("ISO 14443 Type A");
                {
                    iso14443a.AddSubItem("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadIso14443TypeAConfiguration(readerName));
                    iso14443a.AddSubItem("Set ISO 14443 Type A", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeAEnable(readerName));
                    iso14443a.AddSubItem("Set ISO 14443 Type A RxTx Baud Rates", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeARxTxBaudRate(readerName));
                    iso14443a.AddSubItem("Set Mifare Key Cache", () => ReaderContactlessSlotConfigurationSample.SetMifareKeyCache(readerName));
                    iso14443a.AddSubItem("Set Mifare Preferred", () => ReaderContactlessSlotConfigurationSample.SetMifarePreferred(readerName));
                }

                var iso14443b = contactlessSlotConfiguration.AddSubItem("ISO 14443 Type B");
                {
                    iso14443b.AddSubItem("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadIso14443TypeBConfiguration(readerName));
                    iso14443b.AddSubItem("Set ISO 14443 Type B", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeBEnable(readerName));
                    iso14443b.AddSubItem("Set ISO 14443 Type B RxTx Baud Rates", () => ReaderContactlessSlotConfigurationSample.SetIso14443TypeBRxTxBaudRate(readerName));
                }

                var iso15693 = contactlessSlotConfiguration.AddSubItem("ISO 15693");
                {
                    iso15693.AddSubItem("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadIso15693Configuration(readerName));
                    iso15693.AddSubItem("Set ISO 15693", () => ReaderContactlessSlotConfigurationSample.SetIso15693Enable(readerName));
                }

                var felica = contactlessSlotConfiguration.AddSubItem("Felica");
                {
                    felica.AddSubItem("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadFelicaConfiguration(readerName));
                    felica.AddSubItem("Set Felica", () => ReaderContactlessSlotConfigurationSample.SetFelicaEnable(readerName));
                    felica.AddSubItem("Set Felica RxTx Baud Rates", () => ReaderContactlessSlotConfigurationSample.SetFelicaRxTxBaudRate(readerName));
                }

                var iclass15693 = contactlessSlotConfiguration.AddSubItem("iClass 15693");
                {
                    iclass15693.AddSubItem("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadIclass15693Configuration(readerName));
                    iclass15693.AddSubItem("Set iClass 15693", () => ReaderContactlessSlotConfigurationSample.SetIclass15693Enable(readerName));
                }

                var contactlessCommon = contactlessSlotConfiguration.AddSubItem("Contactless Common");
                {
                    contactlessCommon.AddSubItem("Read Configuration", () => ReaderContactlessSlotConfigurationSample.ReadOK5022CommonConfiguration(readerName));
                    contactlessCommon.AddSubItem("Set EMD Supression", () => ReaderContactlessSlotConfigurationSample.SetEmdSupressionEnable(readerName));
                    contactlessCommon.AddSubItem("Set Polling Search Order", () => ReaderContactlessSlotConfigurationSample.SetPollingSearchOrder(readerName));
                    contactlessCommon.AddSubItem("Set Sleep Mode Card Detection", () => ReaderContactlessSlotConfigurationSample.SetSleepModeCardDetectionEnable(readerName));
                    contactlessCommon.AddSubItem("Set Sleep Mode Polling Frequency", () => ReaderContactlessSlotConfigurationSample.SetSleepModePollingFrequency(readerName));
                }
            }

            var contactlessCardExamples = _rootMenuItem.AddSubItem("Contactless Card Examples");
            {
                var mifareExamples = contactlessCardExamples.AddSubItem("Mifare Classic 4K Examples");
                {
                    mifareExamples.AddSubItem("Load key without Secure Session Example", () => new ExampleWithMifareClassic.LoadKeyExample().Run(readerName));
                    mifareExamples.AddSubItem("Get Card UID Example", () => new GetDataExample().Run(readerName));
                    mifareExamples.AddSubItem("Get Card Historical Bytes Example", () => new GetHistoricalBytesExample().Run(readerName));
                    mifareExamples.AddSubItem("Read Example", () => new ExampleWithMifareClassic.ReadBinaryMifareClassic1kExample().Run(readerName));
                    mifareExamples.AddSubItem("Write Example", () => new ExampleWithMifareClassic.UpdateBinaryMifareClassic1kExample().Run(readerName));
                    mifareExamples.AddSubItem("Increment Example", () => new ExampleWithMifareClassic.IncrementMifareClassic1kExample().Run(readerName));
                    mifareExamples.AddSubItem("Decrement Example", () => new ExampleWithMifareClassic.DecrementMifareClassic1kExample().Run(readerName));
                }

                var iclassExamples = contactlessCardExamples.AddSubItem("iClass Examples");
                {
                    iclassExamples.AddSubItem("Load iClass and Secure Session keys Example", () => new ExampleWithiClass.LoadKeyToPcScContainerExample().Run(readerName));
                    iclassExamples.AddSubItem("Get Card CSN Example", () => new GetDataExample().Run(readerName));
                    iclassExamples.AddSubItem("Read Binary iClass 16k Example", () => new ExampleWithiClass.ReadBinaryiClass16kExample().Run(readerName));
                    iclassExamples.AddSubItem("Update Binary iClass 16k Example", () => new ExampleWithiClass.UpdateBinaryiClass16kExample().Run(readerName));
                    iclassExamples.AddSubItem("Read Binary iClass 2ks Example", () => new ExampleWithiClass.ReadBinaryiClass2ksExample().Run(readerName));
                    iclassExamples.AddSubItem("Update Binary iClass 2ks Example", () => new ExampleWithiClass.UpdateBinaryiClass2ksExample().Run(readerName));
                }

                var iso15693Examples = contactlessCardExamples.AddSubItem("ISO 15693 Examples");
                {
                    iso15693Examples.AddSubItem("ISO 15693 Get Card CSN Example", () => new GetDataExample().Run(readerName));
                    iso15693Examples.AddSubItem("Read Binary NXP iCode Example", () => new ExampleWithIso15693.ReadBinaryNXPiCode().Run(readerName));
                    iso15693Examples.AddSubItem("Update Binary NXP iCode Example", () => new ExampleWithIso15693.UpdateBinaryNXPiCode().Run(readerName));
                }

                contactlessCardExamples.AddSubItem("Seos Example", () => new GetDataExample().Run(readerName));
            }
        }
    }
}
