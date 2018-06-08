using HidGlobal.OK.SampleCodes.AViatoR;

namespace HidGlobal.OK.SampleCodes.MenuSections
{
    public class OK5027MenuSection : IMenuSection
    {
        private readonly IMenuItem _rootMenuItem;

        public IMenuItem RootMenuItem
        {
            get { return _rootMenuItem; }
        }

        public OK5027MenuSection(string readerName, string serialNumber)
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
                    iso14443a.AddSubItem("Get ISO 14443 Type A RxTx Baud Rates", () => new Iso14443TypeAConfigurationSample.RxTxBaudRate().Run(readerName, RequestType.Get));
                    iso14443a.AddSubItem("Set ISO 14443 Type A RxTx Baud Rates", () => new Iso14443TypeAConfigurationSample.RxTxBaudRate().Run(readerName, RequestType.Set));
                    iso14443a.AddSubItem("Get ISO 14443 Type A Mifare Emulation Preferred", () => new Iso14443TypeAConfigurationSample.MifareEmulationPreferred().Run(readerName, RequestType.Get));
                    iso14443a.AddSubItem("Set ISO 14443 Type A Mifare Emulation Preferred", () => new Iso14443TypeAConfigurationSample.MifareEmulationPreferred().Run(readerName, RequestType.Set));
                }

                var iso14443b = contactlessSlotConfiguration.AddSubItem("ISO 14443 Type B");
                {
                    iso14443b.AddSubItem("Get ISO 14443 Type B RxTx Baud Rates", () => new Iso14443TypeBConfigurationSample.RxTxBaudRate().Run(readerName, RequestType.Get));
                    iso14443b.AddSubItem("Set ISO 14443 Type B RxTx Baud Rates", () => new Iso14443TypeBConfigurationSample.RxTxBaudRate().Run(readerName, RequestType.Set));
                }

                var felica = contactlessSlotConfiguration.AddSubItem("Felica");
                {
                    felica.AddSubItem("Get Felica RxTx Baud Rates", () => new FelicaConfigurationSample.RxTxBaudRate().Run(readerName, RequestType.Get));
                    felica.AddSubItem("Set Felica RxTx Baud Rates", () => new FelicaConfigurationSample.RxTxBaudRate().Run(readerName, RequestType.Set));
                }
            }

            var kwConfiguration = _rootMenuItem.AddSubItem("Keyboard Wedge Configuration");
            {
                kwConfiguration.AddSubItem("Get Credential Type", () => new KeyboardWedgeConfigurationSample.CredentialTypeCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Credential Type", () => new KeyboardWedgeConfigurationSample.CredentialTypeCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Output Format", () => new KeyboardWedgeConfigurationSample.OutputFormatCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Output Format", () => new KeyboardWedgeConfigurationSample.OutputFormatCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Output Flags", () => new KeyboardWedgeConfigurationSample.OutputFlagsCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Output Flags", () => new KeyboardWedgeConfigurationSample.OutputFlagsCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Range Offset", () => new KeyboardWedgeConfigurationSample.RangeOffsetCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Range Offset", () => new KeyboardWedgeConfigurationSample.RangeOffsetCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Range Length", () => new KeyboardWedgeConfigurationSample.RangeLengthCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Range Length", () => new KeyboardWedgeConfigurationSample.RangeLengthCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Post Strokes Start", () => new KeyboardWedgeConfigurationSample.PostStrokesStartCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Post Strokes Start", () => new KeyboardWedgeConfigurationSample.PostStrokesStartCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Pre Post Strokes", () => new KeyboardWedgeConfigurationSample.PrePostStrokesCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Pre Post Strokes", () => new KeyboardWedgeConfigurationSample.PrePostStrokesCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Keyboard Layout", () => new KeyboardWedgeConfigurationSample.KeyboardLayoutCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Keyboard Layout", () => new KeyboardWedgeConfigurationSample.KeyboardLayoutCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Extended Character Support", () => new KeyboardWedgeConfigurationSample.ExtendedCharacterSupportCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Extended Character Support", () => new KeyboardWedgeConfigurationSample.ExtendedCharacterSupportCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Character Differences", () => new KeyboardWedgeConfigurationSample.CharacterDifferencesCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Character Differences", () => new KeyboardWedgeConfigurationSample.CharacterDifferencesCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Configuration Card Support", () => new KeyboardWedgeConfigurationSample.ConfigurationCardSupportCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Configuration Card Support", () => new KeyboardWedgeConfigurationSample.ConfigurationCardSupportCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Get Led Idle State", () => new KeyboardWedgeConfigurationSample.LedIdleStateCommand().Run(readerName, RequestType.Get));
                kwConfiguration.AddSubItem("Set Led Idle State", () => new KeyboardWedgeConfigurationSample.LedIdleStateCommand().Run(readerName, RequestType.Set));

                kwConfiguration.AddSubItem("Configuration Card Write Data", () => new KeyboardWedgeConfigurationSample.ConfigurationCardWriteDataCommand().Run(readerName));
                kwConfiguration.AddSubItem("Configuration Card Security Key Update (be careful it will update default keys stroed in the reader)", () => new KeyboardWedgeConfigurationSample.ConfigurationCardSecurityKeyUpdateCommand().Run(readerName));
            }
        }
    }
}
