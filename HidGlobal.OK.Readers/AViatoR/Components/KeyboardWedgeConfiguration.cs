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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.AViatoR.Components
{

    #region Base Class

    public struct CharacterDiff
    {
        public byte Character;
        public byte Modifiers;
        public byte KeyVal;
    }

    public class KeyboardWedgeConfigCommandResponse
    {
        public bool IsSuccessfulResponse { get; protected set; }

        public byte? ErrorCycle { get; private set; }
        public byte? ErrorCode { get; private set; }

        public byte Sw1 { get; private set; }
        public byte Sw2 { get; private set; }

        public KeyboardWedgeConfigCommandResponse(IEnumerable<byte> response) => TranslateResponse(response);

        protected virtual void ReadBody(BinaryReader reader) => reader.ReadByte();

        protected virtual void ReadSwCodes(BinaryReader reader)
        {
            Sw1 = reader.ReadByte();
            Sw2 = reader.ReadByte();
        }

        protected virtual void ReadHeader(BinaryReader reader)
        {
            IsSuccessfulResponse = false;

            var responseTag = reader.ReadByte();
            if (responseTag == 0xbd)
            {
                IsSuccessfulResponse = true;
            }
            else if (responseTag == 0x9e)
            {
                reader.ReadByte();
                ErrorCycle = reader.ReadByte();
                ErrorCode = reader.ReadByte();
            }
        }

        protected virtual bool ValidateResponse()
        {
            return true;
        }

        private void TranslateResponse(IEnumerable<byte> response)
        {
            try
            {
                using (var stream = new MemoryStream(response.ToArray()))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        ReadHeader(reader);

                        if (IsSuccessfulResponse)
                        {
                            ReadBody(reader);
                        }

                        ReadSwCodes(reader);
                    }
                }

                if (IsSuccessfulResponse)
                {
                    IsSuccessfulResponse = ValidateResponse();
                }
            }
            catch
            {
                IsSuccessfulResponse = false;
            }
        }
    }

    public abstract class KeyboardWedgeConfigCommand
    {
        protected enum ApplicationTag : byte
        {
            ReaderInformation = 0xA2,
            DeviceSpecific = 0xBC,
        };

        protected enum OperationTag : byte
        {
            Get = 0xA0,
            Set = 0xA1,
        };

        protected enum ConfigurationSectionTag : byte
        {
            CommonContactlessConfiguration = 0xA4,
            CommonHardwareConfiguration = 0xAF,
        };

        protected const byte KeyboardCountryDefinitionTag = 0xAB;
        protected const byte ExtendedCharacterSupportTag = 0xAC;

        public virtual KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response) =>
            new KeyboardWedgeConfigCommandResponse(response);

        protected IApduCommand GenerateApdu(byte applicationTag, byte operationTag, params byte[] followingTags)
        {
            return GenerateApdu(new byte[0], applicationTag, operationTag, followingTags);
        }

        protected IApduCommand GenerateApdu(IEnumerable<byte> data, byte applicationTag, byte operationTag,
            params byte[] followingTags)
        {
            var paramsTags = new List<byte> {applicationTag, operationTag};
            paramsTags.AddRange(followingTags);

            var payload = GetPayload(data, paramsTags.ToArray());

            return new ApduCommand(ApduFormat.Short, ApduCommand.HidGlobalSpecificApduCommandHeader, payload);
        }

        protected IEnumerable<byte> GetPayload(IEnumerable<byte> data, params byte[] followingTags)
        {
            using (var payloadMemoryStream = new MemoryStream())
            {
                using (var payloadWriter = new BinaryWriter(payloadMemoryStream))
                {
                    payloadWriter.Write(data.Reverse().ToArray());
                    foreach (var tag in followingTags.Reverse())
                    {
                        payloadWriter.WriteTlvEncodedData(tag);
                    }
                }
                return payloadMemoryStream.ToArray().Reverse();
            }
        }
    }


    #endregion

    #region Card Type Command

    public class SetKeyboardWedgeCardTypeCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ConfigurationType configurationType, CardType cardType)
        {
            return GenerateApdu(new[] {(byte) cardType}, (byte) ApplicationTag.ReaderInformation,
                (byte) OperationTag.Set,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwCardType);
        }
    }

    public class GetKeyboardWedgeCardTypeCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public CardType CardType { get; private set; }

        public GetKeyboardWedgeCardTypeCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            CardType = (CardType) reader.ReadByte();
        }

        protected override bool ValidateResponse()
        {
            return Enum.IsDefined(typeof(CardType), CardType);
        }
    }

    public class GetKeyboardWedgeCardTypeCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeCardTypeCommandResponse(response);
        }

        public IApduCommand GetApdu(ConfigurationType configurationType)
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwCardType);
        }
    }

    #endregion

    #region Output Format Command

    public class SetKeyboardWedgeOutputFormatCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ConfigurationType configurationType, OutputFormat outputFormat)
        {
            return GenerateApdu(new[] {(byte) outputFormat}, (byte) ApplicationTag.ReaderInformation,
                (byte) OperationTag.Set, (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwOutputFormat);
        }
    }

    public class GetKeyboardWedgeOutputFormatCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public OutputFormat OutputFormat { get; private set; }

        public GetKeyboardWedgeOutputFormatCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            OutputFormat = (OutputFormat) reader.ReadByte();
        }

        protected override bool ValidateResponse()
        {
            return Enum.IsDefined(typeof(OutputFormat), OutputFormat);
        }
    }

    public class GetKeyboardWedgeOutputFormatCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeOutputFormatCommandResponse(response);
        }

        public IApduCommand GetApdu(ConfigurationType configurationType)
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwOutputFormat);
        }
    }

    #endregion

    #region Output Flags Command

    public class SetKeyboardWedgeOutputFlagsCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ConfigurationType configurationType, OutputType outputType, DataOrder bitOrder,
            DataOrder byteOrder)
        {
            var outputFalgs = (byte) ((int) outputType | (int) bitOrder << 1 | (int) byteOrder << 2);

            return GenerateApdu(new[] {outputFalgs}, (byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Set,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwFlags);
        }
    }

    public class GetKeyboardWedgeOutputFlagsCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        private const byte OutputTypeFlag = 0x01;
        private const byte BitOrderFlag = 0x02;
        private const byte ByteOrderFlag = 0x04;
        public OutputType OutputType { get; private set; }
        public DataOrder BitOrder { get; private set; }
        public DataOrder ByteOrder { get; private set; }

        public GetKeyboardWedgeOutputFlagsCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            var outputFalgs = reader.ReadByte();

            OutputType = (OutputType) (outputFalgs & OutputTypeFlag);
            BitOrder = (outputFalgs & BitOrderFlag) != 0 ? DataOrder.Reversed : DataOrder.Normal;
            ByteOrder = (outputFalgs & ByteOrderFlag) != 0 ? DataOrder.Reversed : DataOrder.Normal;
        }

        protected override bool ValidateResponse()
        {
            return Enum.IsDefined(typeof(OutputType), OutputType)
                   && Enum.IsDefined(typeof(DataOrder), BitOrder)
                   && Enum.IsDefined(typeof(DataOrder), ByteOrder);
        }
    }

    public class GetKeyboardWedgeOutputFlagsCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeOutputFlagsCommandResponse(response);
        }

        public IApduCommand GetApdu(ConfigurationType configurationType)
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwFlags);
        }
    }

    #endregion

    #region Range Start Command

    public class SetKeyboardWedgeRangeStartCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ConfigurationType configurationType, byte rangeStart)
        {
            return GenerateApdu(new[] {rangeStart}, (byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Set,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwRangeStart);
        }
    }

    public class GetKeyboardWedgeRangeStartCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public byte RangeStart { get; private set; }

        public GetKeyboardWedgeRangeStartCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            RangeStart = reader.ReadByte();
        }
    }

    public class GetKeyboardWedgeRangeStartCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeRangeStartCommandResponse(response);
        }

        public IApduCommand GetApdu(ConfigurationType configurationType)
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwRangeStart);
        }
    }

    #endregion

    #region Range Length Command

    public class SetKeyboardWedgeRangeLengthCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ConfigurationType configurationType, byte rangeLength)
        {
            return GenerateApdu(new[] {rangeLength}, (byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Set,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwRangeLength);
        }
    }

    public class GetKeyboardWedgeRangeLengthCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public byte RangeLength { get; private set; }

        public GetKeyboardWedgeRangeLengthCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            RangeLength = reader.ReadByte();
        }
    }

    public class GetKeyboardWedgeRangeLengthCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeRangeLengthCommandResponse(response);
        }

        public IApduCommand GetApdu(ConfigurationType configurationType)
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwRangeLength);
        }
    }

    #endregion

    #region Post Strokes Start Command

    public class SetKeyboardWedgePostStrokesStartCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ConfigurationType configurationType, byte postStrokesStart)
        {
            return GenerateApdu(new[] {postStrokesStart}, (byte) ApplicationTag.ReaderInformation,
                (byte) OperationTag.Set, (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwPostStrokesStart);
        }
    }

    public class GetKeyboardWedgePostStrokesStartCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public byte PostStrokesStart { get; private set; }

        public GetKeyboardWedgePostStrokesStartCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            PostStrokesStart = reader.ReadByte();
        }
    }

    public class GetKeyboardWedgePostStrokesStartCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgePostStrokesStartCommandResponse(response);
        }

        public IApduCommand GetApdu(ConfigurationType configurationType)
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwPostStrokesStart);
        }
    }

    #endregion

    #region Pre Post Strokes Command

    public class SetKeyboardWedgePrePostStrokesCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ConfigurationType configurationType, byte[] prePostStrokes)
        {
            ValidateInput(ref prePostStrokes);

            return GenerateApdu(prePostStrokes, (byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Set,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration, (byte) configurationType,
                (byte) KbWedgeConfigCommand.KbwPrePostStrokes);
        }

        private void ValidateInput(ref byte[] prePostStrokes)
        {
            if (prePostStrokes != null)
            {
                // the command requires prePostStrokes to be exactly 32 bytes long 
                Array.Resize(ref prePostStrokes, 32);
            }
            else
            {
                throw new ArgumentNullException(nameof(prePostStrokes));
            }
        }
    }

    public class GetKeyboardWedgePrePostStrokesCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public byte[] PrePostStrokes { get; private set; }

        public GetKeyboardWedgePrePostStrokesCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadLength();
            reader.ReadByte();
            int lengthOfData = reader.ReadLength();
            PrePostStrokes = reader.ReadBytes(lengthOfData);
        }
    }

    public class GetKeyboardWedgePrePostStrokesCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgePrePostStrokesCommandResponse(response);
        }

        public IApduCommand GetApdu(ConfigurationType configurationType)
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                (byte) configurationType, (byte) KbWedgeConfigCommand.KbwPrePostStrokes);
        }
    }

    #endregion

    #region Keyboard Layout Command

    public class SetKeyboardWedgeKeyboardLayoutCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(KeyboardLayout keyboardLayout)
        {
            return GenerateApdu(new[] {(byte) keyboardLayout}, (byte) ApplicationTag.ReaderInformation,
                (byte) OperationTag.Set, (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                KeyboardCountryDefinitionTag, (byte) KbWedgeConfigCommand.KbwKeyboardLayout);
        }
    }

    public class GetKeyboardWedgeKeyboardLayoutCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public KeyboardLayout KeyboardLayout { get; private set; }

        public GetKeyboardWedgeKeyboardLayoutCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            KeyboardLayout = (KeyboardLayout) reader.ReadByte();
        }

        protected override bool ValidateResponse()
        {
            return Enum.IsDefined(typeof(KeyboardLayout), KeyboardLayout);
        }
    }

    public class GetKeyboardWedgeKeyboardLayoutCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeKeyboardLayoutCommandResponse(response);
        }

        public IApduCommand GetApdu()
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                KeyboardCountryDefinitionTag, (byte) KbWedgeConfigCommand.KbwKeyboardLayout);
        }
    }

    #endregion

    #region Extended Character Support Command

    public class SetKeyboardWedgeSupportForExtendedCharsCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(ExtendedCharacterSupport extendedCharacterSupport)
        {
            return GenerateApdu(new[] {(byte) extendedCharacterSupport}, (byte) ApplicationTag.ReaderInformation,
                (byte) OperationTag.Set, (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                ExtendedCharacterSupportTag, (byte) KbWedgeConfigCommand.KbwExtendedCharSupport);
        }
    }

    public class GetKeyboardWedgeSupportForExtendedCharsCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public ExtendedCharacterSupport ExtendedCharacterSupport { get; private set; }

        public GetKeyboardWedgeSupportForExtendedCharsCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            ExtendedCharacterSupport = (ExtendedCharacterSupport) reader.ReadByte();
        }

        protected override bool ValidateResponse()
        {
            return Enum.IsDefined(typeof(ExtendedCharacterSupport), ExtendedCharacterSupport);
        }
    }

    public class GetKeyboardWedgeSupportForExtendedCharsCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeSupportForExtendedCharsCommandResponse(response);
        }

        public IApduCommand GetApdu()
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                ExtendedCharacterSupportTag, (byte) KbWedgeConfigCommand.KbwExtendedCharSupport);
        }
    }

    #endregion

    #region Character Differences Command

    public class SetKeyboardWedgeCharactersDiffCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(CharacterDiff[] characterDiffs)
        {
            return GenerateApdu(GenerateDiffArray(characterDiffs), (byte) ApplicationTag.ReaderInformation,
                (byte) OperationTag.Set,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration, KeyboardCountryDefinitionTag,
                (byte) KbWedgeConfigCommand.KbwCharactersDiff);
        }

        private byte[] GenerateDiffArray(CharacterDiff[] characterDiffs)
        {
            if (characterDiffs == null) throw new ArgumentNullException(nameof(characterDiffs));

            // array can contain up to 63 items. As a result of this operation shorter array will be filled with zeroes
            Array.Resize(ref characterDiffs, 63);

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    foreach (var item in characterDiffs)
                    {
                        writer.Write(item);
                    }

                    // write empty character diff at the end 
                    WriteTerminatingCharacterDiff(writer);
                }

                return stream.ToArray();
            }
        }

        private void WriteTerminatingCharacterDiff(BinaryWriter writer)
        {
            writer.Write(new CharacterDiff());
        }
    }

    public class GetKeyboardWedgeCharactersDiffCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public CharacterDiff[] CharacterDifferences { get; private set; }

        public GetKeyboardWedgeCharactersDiffCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadLength();
            reader.ReadByte();
            reader.ReadLength();

            CharacterDifferences = ReadCharacterDiffs(reader).ToArray();
        }

        private IEnumerable<CharacterDiff> ReadCharacterDiffs(BinaryReader reader)
        {
            while (reader.BaseStream.Length - reader.BaseStream.Position >= 3)
            {
                yield return reader.ReadCharacterDiff();
            }
        }
    }

    public class GetKeyboardWedgeCharactersDiffCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response)
        {
            return new GetKeyboardWedgeCharactersDiffCommandResponse(response);
        }

        public IApduCommand GetApdu()
        {
            return GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
                (byte) ConfigurationSectionTag.CommonContactlessConfiguration,
                KeyboardCountryDefinitionTag, (byte) KbWedgeConfigCommand.KbwCharactersDiff);
        }
    }

    #endregion

    #region Configuration Card Support Command

    public class GetConfigurationCardSupportEnabledCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public FeatureSupport FeatureSupport { get; private set; }

        public GetConfigurationCardSupportEnabledCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            FeatureSupport = (FeatureSupport) reader.ReadByte();
        }

        protected override bool ValidateResponse()
        {
            return Enum.IsDefined(typeof(FeatureSupport), FeatureSupport);
        }
    }

    public class GetConfigurationCardSupportEnabledCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu() => GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
            (byte) ConfigurationSectionTag.CommonContactlessConfiguration, 0xA0, 0x91);

        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response) =>
            new GetConfigurationCardSupportEnabledCommandResponse(response);
    }

    public class SetConfigurationCardSupportEnabledCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(FeatureSupport featureSupport) =>
            GenerateApdu(new[] {(byte) featureSupport}, (byte) ApplicationTag.ReaderInformation,
                (byte) OperationTag.Set, (byte) ConfigurationSectionTag.CommonContactlessConfiguration, 0xA0,
                0x91);
    }

    #endregion

    #region Led Idle State Command

    public class GetLedIdleStateCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public LedIdleState LedIdleState { get; private set; }

        public GetLedIdleStateCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
            reader.ReadBytes(3);
            LedIdleState = (LedIdleState) reader.ReadByte();
        }

        protected override bool ValidateResponse()
        {
            return Enum.IsDefined(typeof(LedIdleState), LedIdleState);
        }
    }

    public class GetLedIdleStateCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu() => GenerateApdu((byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Get,
            (byte) ConfigurationSectionTag.CommonHardwareConfiguration, 0xA0, 0x81);

        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response) =>
            new GetLedIdleStateCommandResponse(response);
    }

    public class SetLedIdleStateCommand : KeyboardWedgeConfigCommand
    {
        public IApduCommand GetApdu(LedIdleState ledIdleState) =>
            GenerateApdu(new[] {(byte) ledIdleState}, (byte) ApplicationTag.ReaderInformation, (byte) OperationTag.Set,
                (byte) ConfigurationSectionTag.CommonHardwareConfiguration, 0xA0, 0x81);
    }

    #endregion

    #region Configuration Card Initialization Command

    public class ConfigurationCardInitializationCommandResponse : KeyboardWedgeConfigCommandResponse
    {
        public ConfigurationCardInitializationCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }

        protected override void ReadBody(BinaryReader reader)
        {
        }

        protected override void ReadHeader(BinaryReader reader)
        {
        }

        protected override void ReadSwCodes(BinaryReader reader)
        {
            base.ReadSwCodes(reader);

            if (Sw1 == 0x90 && Sw2 == 0x00)
            {
                IsSuccessfulResponse = true;
            }
            else
            {
                IsSuccessfulResponse = false;
            }
        }
    };

    public class ConfigurationCardInitializationCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response) =>
            new ConfigurationCardInitializationCommandResponse(response);

        public IApduCommand GetApdu(byte filesCount) =>
            GenerateApdu(new []{ filesCount },(byte) ApplicationTag.DeviceSpecific, (byte) OperationTag.Set, 0xA0, 0x80);
    };

    #endregion

    #region Configuration Card Write Data Command

    public class ConfigurationCardWriteDataCommandResponse : ConfigurationCardInitializationCommandResponse
    {
        public ConfigurationCardWriteDataCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }
    }

    public class ConfigurationCardWriteDataCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response) =>
            new ConfigurationCardWriteDataCommandResponse(response);

        public IApduCommand GetApdu(byte fileNumber, ushort offset, IEnumerable<byte> data)
        {
            var extendedDataBytes = new List<byte>();
            var offsetBytes = BinaryHelper.ConvertToBytes(offset, BinaryHelper.ByteOrder.MsbFirst);

            extendedDataBytes.AddRange(new byte[] {0x82, 0x01, fileNumber});
            extendedDataBytes.AddRange(new byte[] {0x83, 0x02, offsetBytes[0], offsetBytes[1]});
            extendedDataBytes.AddRange(GetPayload(data, 0x84));
            return GenerateApdu(extendedDataBytes, (byte) ApplicationTag.DeviceSpecific, (byte) OperationTag.Set, 0xA1);
        }
    };

    #endregion

    #region Configuration Card Security Key Update Command

    public class ConfigurationCardSecurityKeyUpdateCommandResponse : ConfigurationCardInitializationCommandResponse
    {
        public ConfigurationCardSecurityKeyUpdateCommandResponse(IEnumerable<byte> response) : base(response)
        {
        }
    }

    public class ConfigurationCardSecurityKeyUpdateCommand : KeyboardWedgeConfigCommand
    {
        public override KeyboardWedgeConfigCommandResponse TranslateResponse(IEnumerable<byte> response) =>
            new ConfigurationCardSecurityKeyUpdateCommandResponse(response);

        public IApduCommand GetApdu([MaxLength(0x10)] [MinLength(0x10)] ICollection<byte> securityKeyBytes) =>
            GenerateApdu(securityKeyBytes, (byte) ApplicationTag.DeviceSpecific, (byte) OperationTag.Set, 0xA5, 0x80);
    };

    #endregion

};
