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
using System.IO;
using System.Linq;
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.AViatoR.Components
{
    public abstract class SEPCommand
    {
        public class SEPCommandResponse
        {
            public enum ErrorCode : ushort
            {
                CorruptedCommand      = 0x0000,
                Success               = 0x9000,
                Warning               = 0x6282,
                NoResponseFromMedia   = 0x6400,
                UnknownError          = 0x6f00,
                WrongAPDULength       = 0x6700,
                BlockNotAuthenticated = 0x6982,
                IllegalBlockNumber    = 0x6a82,
                WrongLe               = 0x6c00,
            }

            public ErrorCode ReturnCode { get; set; }
        }

        private readonly byte[] _commonHeader = new byte[] { 0xff, 0x70, 0x07, 0x6b };
        private readonly byte _commonLe = 0x00;

        protected const byte SeProcessorCommandTag = 0xa0;
        protected const byte ProcessCardAPITag = 0xa5;
        protected const byte CardAPIDESFireTag = 0xa2;
        protected const byte ApplicationNumberTag = 0x80;
        protected const byte KeyReferenceTag = 0x81;
        protected const byte FileNumberTag = 0x80;
        protected const byte FileNumberValLength = 0x01;
        protected const byte OffsetTag = 0x81;
        protected const byte DataTag = 0x82;
        protected const byte DataValLenght = 0x01;
        protected const byte ModeTag = 0x83;
        protected const byte ModeValLength = 0x01;

        public enum CommunicationMode : byte
        {
            Plain = 0x00,
            MAC = 0x01,
            Encrypt = 0x03,
        }

        public enum CommitFlag
        {
            NoCommit = 0x00,
            Commit = 0x01,
        }

        protected abstract byte BodyLength { get; }

        public abstract SEPCommandResponse TranslateResponse(string response);

        private byte TotalLength => (byte)(BodyLength + 6);

        protected byte GetLengthToEnd(BinaryWriter writer)
        {
            return (byte)(TotalLength - writer.BaseStream.Position - 2);
        }

        protected void WriteSuffix(BinaryWriter writer)
        {
            writer.Write(_commonHeader);
            writer.Write(BodyLength);
        }

        protected void WritePostfix(BinaryWriter writer)
        {
            writer.Write(_commonLe);
        }

        protected void ReadErrorCode(byte[] response, SEPCommandResponse commandResponse)
        {
            if (commandResponse == null) return;

            if (response != null && response.Length >= 2)
            {
                ushort statusWord = BitConverter.ToUInt16(response.Reverse().ToArray(), 0);
                commandResponse.ReturnCode = GetErrorCode(statusWord);
            }
            else
            {
                commandResponse.ReturnCode = SEPCommandResponse.ErrorCode.CorruptedCommand;
            }
        }

        private SEPCommandResponse.ErrorCode GetErrorCode(ushort statusWord)
        {
            switch (statusWord)
            {
                case 0x9000:
                    return SEPCommandResponse.ErrorCode.Success;
                case 0x6200:
                    return SEPCommandResponse.ErrorCode.Warning;
                case 0x6400:
                    return SEPCommandResponse.ErrorCode.NoResponseFromMedia;
                case 0x6f00:
                    return SEPCommandResponse.ErrorCode.UnknownError;
                case 0x6700:
                    return SEPCommandResponse.ErrorCode.WrongAPDULength;
                case 0x6900:
                    return SEPCommandResponse.ErrorCode.BlockNotAuthenticated;
                case 0x6a00:
                    return SEPCommandResponse.ErrorCode.IllegalBlockNumber;
                case 0x6c00:
                    return SEPCommandResponse.ErrorCode.WrongLe;
                default:
                    return SEPCommandResponse.ErrorCode.CorruptedCommand;
            }
        }
    }

    public abstract class SEPCommandCommonBase : SEPCommand
    {
        public override SEPCommandResponse TranslateResponse(string response)
        {
            if (response == null)
            {
                throw new NullReferenceException("response can't be null");
            }
            if (!BinaryHelper.IsValidHexString(response))
            {
                throw new ArgumentException("response isn't valid hex string");
            }

            byte[] data = BinaryHelper.ConvertOctetStringToBytes(response);
            SEPCommandResponse commandResponse = new SEPCommandResponse();
            ReadErrorCode(data, commandResponse);
            return commandResponse;
        }
    }

    public class SEPLoadKey : SEPCommandCommonBase
    {
        private const byte CardAPILoadKeyTag = 0xa5;
        private const byte IsPersistentValLength = 0x80;
        private const byte LengthOfIsPersistentValue = 0x01;
        private const byte KeyValueTag = 0x82;

        private byte[] _keyReference;
        private byte[] _keyValue;

        protected override byte BodyLength => (byte)(13 + _keyReference.Length + _keyValue.Length);

        public enum Persistence : byte
        {
            Volatile   = 0x00,
            Persistent = 0x01,
        }

        public string GetApdu(Persistence persistence, string keyReference, string keyValue)
        {
            ValidateInput(keyReference, keyValue);

            _keyReference = BinaryHelper.ConvertOctetStringToBytes(keyReference);
            _keyValue = BinaryHelper.ConvertOctetStringToBytes(keyValue);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPILoadKeyTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(IsPersistentValLength);
                    writer.Write(LengthOfIsPersistentValue);
                    writer.Write((byte)persistence);
                    writer.Write(KeyReferenceTag);
                    writer.Write((byte)_keyReference.Length);
                    writer.Write(_keyReference);
                    writer.Write(KeyValueTag);
                    writer.Write((byte)_keyValue.Length);
                    writer.Write(_keyValue);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        private void ValidateInput(string keyReference, string keyValue)
        {
            if (keyReference == null)
            {
                throw new NullReferenceException("keyReference can't be null");
            }
            if (keyValue == null)
            {
                throw new NullReferenceException("keyValue can't be null");
            }

            if(!BinaryHelper.IsValidHexString(keyReference))
            {
                throw new ArgumentException("keyReference isn't valid hex string");
            }
            if (!BinaryHelper.IsValidHexString(keyValue))
            {
                throw new ArgumentException("keyValue isn't valid hex string");
            }

            if (keyReference.Length != 6)
            {
                throw new ArgumentException("keyReference needs to be exactly 3 bytes long");
            }
        }
    }

    public class SEPDESFireAuthNative : SEPCommandCommonBase
    {
        private const byte DESFireAuthNativeTag = 0xa1;
        private const byte KeyNumberTag = 0x80;
        private const byte KeyNumberValLength = 0x01;

        private byte[] _keyReference;

        protected override byte BodyLength => (byte)(13 + _keyReference.Length);

        public string GetApdu(byte keyNumber, string keyReference)
        {
            ValidateInput(keyReference);

            _keyReference = BinaryHelper.ConvertOctetStringToBytes(keyReference);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPIDESFireTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(DESFireAuthNativeTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(KeyNumberTag);
                    writer.Write(KeyNumberValLength);
                    writer.Write(keyNumber);
                    writer.Write(KeyReferenceTag);
                    writer.Write((byte)_keyReference.Length);
                    writer.Write(_keyReference);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        private void ValidateInput(string keyReference)
        {
            if (keyReference == null)
            {
                throw new NullReferenceException("keyReference can't be null");
            }

            if (!BinaryHelper.IsValidHexString(keyReference))
            {
                throw new ArgumentException("keyReference isn't valid hex string");
            }

            if (keyReference.Length != 6)
            {
                throw new ArgumentException("keyReference needs to be exactly 3 bytes long");
            }
        }
    }

    public class SEPDESFireFormatCard : SEPCommandCommonBase
    {
        private const byte DESFireFormatTag = 0x93;
        private const byte DESFireFormatValLength = 0x00;

        protected override byte BodyLength => 0x08;

        public string GetApdu()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPIDESFireTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(DESFireFormatTag);
                    writer.Write(DESFireFormatValLength);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }
    }

    public class SEPDESFireCreateApplication : SEPCommandCommonBase
    {
        private const byte DESFireCreateApplicationTag = 0xa6;
        private const byte MasterKeySettingsTag = 0x81;
        private const byte MasterKeySettingsValLength = 0x01;
        private const byte NumberOfKeysTag = 0x82;
        private const byte NumberOfKeysValLength = 0x01;

        private byte[] _applicationNumber;

        protected override byte BodyLength => (byte) (16 + _applicationNumber.Length);

        public string GetApdu(string applicationNumber, byte masterKeySettings, byte numberOfKeys)
        {
            ValidateInput(applicationNumber);

            _applicationNumber = BinaryHelper.ConvertOctetStringToBytes(applicationNumber);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPIDESFireTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(DESFireCreateApplicationTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ApplicationNumberTag);
                    writer.Write((byte)_applicationNumber.Length);
                    writer.Write(_applicationNumber);
                    writer.Write(MasterKeySettingsTag);
                    writer.Write(MasterKeySettingsValLength);
                    writer.Write(masterKeySettings);
                    writer.Write(NumberOfKeysTag);
                    writer.Write(NumberOfKeysValLength);
                    writer.Write(numberOfKeys);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        private void ValidateInput(string applicationNumber)
        {
            if (applicationNumber == null)
            {
                throw new NullReferenceException("applicationNumber can't be null");
            }

            if (!BinaryHelper.IsValidHexString(applicationNumber))
            {
                throw new ArgumentException("applicationNumber isn't valid hex string");
            }

            if (applicationNumber.Length != 6)
            {
                throw new ArgumentException("applicationNumber needs to be exactly 3 bytes long");
            }
        }
    }

    public class SEPDESFireSelectApplication : SEPCommandCommonBase
    {
        private const byte DESFireSelectApplicationTag = 0xa0;

        private byte[] _applicationNumber;

        protected override byte BodyLength => (byte)(10 + _applicationNumber.Length);

        public string GetApdu(string applicationNumber)
        {
            ValidateInput(applicationNumber);

            _applicationNumber = BinaryHelper.ConvertOctetStringToBytes(applicationNumber);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPIDESFireTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(DESFireSelectApplicationTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ApplicationNumberTag);
                    writer.Write((byte)_applicationNumber.Length);
                    writer.Write(_applicationNumber);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        private void ValidateInput(string applicationNumber)
        {
            if (applicationNumber == null)
            {
                throw new NullReferenceException("applicationNumber can't be null");
            }

            if (!BinaryHelper.IsValidHexString(applicationNumber))
            {
                throw new ArgumentException("applicationNumber isn't valid hex string");
            }

            if (applicationNumber.Length != 6)
            {
                throw new ArgumentException("applicationNumber needs to be exactly 3 bytes long");
            }
        }
    }

    public class SEPDESFireCreateStandardDataFile : SEPCommandCommonBase
    {
        private const byte DESFireCreateStandardDataFileTag = 0xa8;
        private const byte CommunicationSettingsTag = 0x82;
        private const byte CommunicationSettingsValLength = 0x01;
        private const byte AccessRightsTag = 0x83;
        private const byte FileSizeTag = 0x84;

        private byte[] _accessRights;
        private byte[] _fileSize;

        protected override byte BodyLength => (byte)(18 + _accessRights.Length + _fileSize.Length);

        public string GetApdu(byte fileNumber, CommunicationMode communicationMode, string accessRights,
            string fileSize)
        {
            ValidateInput(accessRights, fileSize);

            _accessRights = BinaryHelper.ConvertOctetStringToBytes(accessRights);
            _fileSize = BinaryHelper.ConvertOctetStringToBytes(fileSize);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPIDESFireTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(DESFireCreateStandardDataFileTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(FileNumberTag);
                    writer.Write(FileNumberValLength);
                    writer.Write(fileNumber);
                    writer.Write(CommunicationSettingsTag);
                    writer.Write(CommunicationSettingsValLength);
                    writer.Write((byte)communicationMode);
                    writer.Write(AccessRightsTag);
                    writer.Write((byte)_accessRights.Length);
                    writer.Write(_accessRights);
                    writer.Write(FileSizeTag);
                    writer.Write((byte)_fileSize.Length);
                    writer.Write(_fileSize);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        private void ValidateInput(string accessRights, string fileSize)
        {
            if (accessRights == null)
            {
                throw new NullReferenceException("accessRights can't be null");
            }
            if (fileSize == null)
            {
                throw new NullReferenceException("fileSize can't be null");
            }

            if (!BinaryHelper.IsValidHexString(accessRights))
            {
                throw new ArgumentException("accessRights isn't valid hex string");
            }
            if (!BinaryHelper.IsValidHexString(fileSize))
            {
                throw new ArgumentException("fileSize isn't valid hex string");
            }

            if (fileSize.Length != 6)
            {
                throw new ArgumentException("fileSize needs to be exactly 3 bytes long");
            }
        }
    }

    public class SEPDESFireWriteData : SEPCommandCommonBase
    {
        private const byte DESFireWriteDataTag = 0xa4;
        private const byte CommitTag = 0x84;
        private const byte CommitValLength = 0x01;

        private byte[] _offset;
        private byte[] _dataToBeWritten;

        protected override byte BodyLength => (byte)(21 + _offset.Length + _dataToBeWritten.Length);

        public string GetApdu(byte fileNumber, string offset, string dataToBeWritten, CommunicationMode communicationMode,
            CommitFlag commitFlag)
        {
            ValidateInput(offset, dataToBeWritten);

            _offset = BinaryHelper.ConvertOctetStringToBytes(offset);
            _dataToBeWritten = BinaryHelper.ConvertOctetStringToBytes(dataToBeWritten);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPIDESFireTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(DESFireWriteDataTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(FileNumberTag);
                    writer.Write(FileNumberValLength);
                    writer.Write(fileNumber);
                    writer.Write(OffsetTag);
                    writer.Write((byte)_offset.Length);
                    writer.Write(_offset);
                    writer.Write(DataTag);
                    writer.Write((byte)_dataToBeWritten.Length);
                    writer.Write(_dataToBeWritten);
                    writer.Write(ModeTag);
                    writer.Write(ModeValLength);
                    writer.Write((byte)communicationMode);
                    writer.Write(CommitTag);
                    writer.Write(CommitValLength);
                    writer.Write((byte)commitFlag);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        private void ValidateInput(string offset, string dataToBeWritten)
        {
            if (offset == null)
            {
                throw new NullReferenceException("offset can't be null");
            }
            if (dataToBeWritten == null)
            {
                throw new NullReferenceException("dataToBeWritten can't be null");
            }

            if (!BinaryHelper.IsValidHexString(offset))
            {
                throw new ArgumentException("offset isn't valid hex string");
            }
            if (!BinaryHelper.IsValidHexString(dataToBeWritten))
            {
                throw new ArgumentException("dataToBeWritten isn't valid hex string");
            }

            if (2 > offset.Length || offset.Length > 4)
            {
                throw new ArgumentException("offset needs to be 1 or 2 bytes long");
            }
        }
    }

    public class SEPDESFireReadData : SEPCommandCommonBase
    {
        public class SEPDESFireReadDataResponse : SEPCommandResponse
        {
            public byte[] DataRaw { get; private set; }
            public string DataHexString { get; private set; }

            public SEPDESFireReadDataResponse(byte[] data)
            {
                DataRaw = data;
                DataHexString = data != null ? BinaryHelper.ConvertBytesToOctetString(data) : null;
            }
        }

        private const byte DESFireReadDataTag = 0xa3;

        private byte[] _offset;

        protected override byte BodyLength => (byte)(19 + _offset.Length);

        public string GetApdu(byte fileNumber, string offset, byte numberOfBytesToBeRead,
            CommunicationMode communicationMode)
        {
            ValidateInput(offset);

            _offset = BinaryHelper.ConvertOctetStringToBytes(offset);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ProcessCardAPITag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(CardAPIDESFireTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(DESFireReadDataTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(FileNumberTag);
                    writer.Write(FileNumberValLength);
                    writer.Write(fileNumber);
                    writer.Write(OffsetTag);
                    writer.Write((byte)_offset.Length);
                    writer.Write(_offset);
                    writer.Write(DataTag);
                    writer.Write(DataValLenght);
                    writer.Write(numberOfBytesToBeRead);
                    writer.Write(ModeTag);
                    writer.Write(ModeValLength);
                    writer.Write((byte)communicationMode);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        public override SEPCommandResponse TranslateResponse(string response)
        {
            if (response == null)
            {
                throw new NullReferenceException("response can't be null");
            }
            if (!BinaryHelper.IsValidHexString(response))
            {
                throw new ArgumentException("response isn't valid hex string");
            }

            SEPDESFireReadDataResponse commandResponse;
            byte[] data = BinaryHelper.ConvertOctetStringToBytes(response);

            if (data != null && data.Length > 2)
            {
                commandResponse = new SEPDESFireReadDataResponse(data.Take(data.Length - 2).ToArray()); // read only body - last two bytes contain sw codes
            }
            else
            {
                commandResponse = new SEPDESFireReadDataResponse(null);
            }

            ReadErrorCode(data, commandResponse);

            return commandResponse;
        }

        private void ValidateInput(string offset)
        {
            if (offset == null)
            {
                throw new NullReferenceException("offset can't be null");
            }

            if (!BinaryHelper.IsValidHexString(offset))
            {
                throw new ArgumentException("offset isn't valid hex string");
            }

            if (2 > offset.Length || offset.Length > 4)
            {
                throw new ArgumentException("offset needs to be 1 or 2 bytes long");
            }
        }
    }

    public class SEPReadPACSData : SEPCommandCommonBase
    {
        public class SEPReadPACSDataResponse : SEPCommandResponse
        {
            public enum MediaEdgeType : byte
            {
                Unknown     = 0x00,
                DESFire     = 0x01,
                MIFARE      = 0x02,
                PicoPass    = 0x03,
                ISO14443AL4 = 0x04,
                MIFAREPlus  = 0x06,
                Seos        = 0x07,
            }

            private const byte ElementDataTag = 0x80;
            private const byte SecureObjectOIDTag = 0x81;
            private const byte MediaTypeTag = 0x82;

            public string ContentElementData { get; private set; }
            public string SecureObjectOID { get; private set; }

            public MediaEdgeType MediaType { get; private set; }

            public SEPReadPACSDataResponse(byte[] data)
            {       
                ReadCommand(data);
            }

            private void ReadCommand(byte[] data)
            {
                if (data != null)
                {
                    using (var stream = new MemoryStream(data)) 
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            reader.ReadByte(); // tag
                            reader.ReadByte(); // length to end of command

                            while (reader.BaseStream.Position < reader.BaseStream.Length)
                            {
                                ReadValue(reader, reader.ReadByte());
                            }
                        }
                    }
                }
            }

            private void ReadValue(BinaryReader reader, byte tag)
            {
                switch (tag)
                {
                    case ElementDataTag:
                        ReadElementData(reader);
                        break;
                    case SecureObjectOIDTag:
                        ReadSecureObjectOID(reader);
                        break;
                    case MediaTypeTag:
                        ReadMediaType(reader);
                        break;
                }
            }

            private void ReadElementData(BinaryReader reader)
            {
                ContentElementData = reader.ReadOctetString();
            }

            private void ReadSecureObjectOID(BinaryReader reader)
            {
                SecureObjectOID = reader.ReadOctetString();
            }

            private void ReadMediaType(BinaryReader reader)
            {
                reader.ReadByte(); // length
                MediaType = (MediaEdgeType)reader.ReadByte();
            }
        }

        private const byte GetContentElement2Tag = 0xbe;
        private const byte ContentElementTag = 0x80;
        private const byte ContentElementValLength = 0x01;
        private const byte PhysicalAccessBits = 0x04;

        protected override byte BodyLength => 0x07;

        public string GetApdu()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteSuffix(writer);

                    writer.Write(SeProcessorCommandTag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(GetContentElement2Tag);
                    writer.Write(GetLengthToEnd(writer));
                    writer.Write(ContentElementTag);
                    writer.Write(ContentElementValLength);
                    writer.Write(PhysicalAccessBits);

                    WritePostfix(writer);
                }

                data = stream.ToArray();
            }

            return BinaryHelper.ConvertBytesToOctetString(data);
        }

        public override SEPCommandResponse TranslateResponse(string response)
        {
            if (response == null)
            {
                throw new NullReferenceException("response can't be null");
            }
            if (!BinaryHelper.IsValidHexString(response))
            {
                throw new ArgumentException("response isn't valid hex string");
            }

            SEPReadPACSDataResponse commandResponse;
            byte[] data = BinaryHelper.ConvertOctetStringToBytes(response);

            if (data != null && data.Length > 2)
            {
                commandResponse = new SEPReadPACSDataResponse(data.Take(data.Length - 2).ToArray()); // read only body - last two bytes contain sw codes
            }
            else
            {
                commandResponse = new SEPReadPACSDataResponse(null);
            }

            ReadErrorCode(data, commandResponse);

            return commandResponse;
        }
    }
}