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
using System.Linq;
using HidGlobal.OK.Readers.Components;

namespace HidGlobal.OK.Readers.AViatoR.Components
{
    public class ContactlessCardCommunication
    {
        public LoadKeyCommand LoadKey => new LoadKeyCommand();

        public GetDataCommand GetData => new GetDataCommand(); 

        public ReadBinaryCommand ReadBinary => new ReadBinaryCommand();

        public UpdateBinaryCommand UpdateBinary => new UpdateBinaryCommand(); 

        public GeneralAuthenticateCommand GeneralAuthenticate => new GeneralAuthenticateCommand(); 

        public IncrementCommand Increment=> new IncrementCommand(); 

        public DecrementCommand Decrement => new DecrementCommand();
    }

    public class ContactlessCardCommunicationV2
    {
        public LoadKeyCommand LoadKey => new LoadKeyCommand();

        public GetDataCommand GetData => new GetDataCommand();

        public ReadBinaryCommand ReadBinary => new ReadBinaryCommand();

        public UpdateBinaryCommand UpdateBinary => new UpdateBinaryCommand();

        public GeneralAuthenticateCommand GeneralAuthenticate => new GeneralAuthenticateCommand();

        public IncrementDecrementCommand IncrementDecrement => new IncrementDecrementCommand();
    }

    public class LoadKeyCommand
    {
        // ToDo Currently only plain transmission is supported
        public enum Transmission
        {
            Plain,
        }
        public enum Persistence
        {
            Volatile,
            Persistent,
        }
        public enum KeyType
        {
            CardKey,
            ReaderKey,
        }
        public enum KeyLength : byte
        {
            _6Bytes = 0x06,
            _8Bytes = 0x08,
            _16Bytes = 0x10,
            _24Bytes = 0x18,
            _32Bytes = 0x20,
        }
        public string GetApdu(byte keySlot, KeyType keyType, Persistence persistence, Transmission transmissionType, KeyLength keyLength, string key)
        {
            key = key.Replace(" ", "").Replace("-", "");

            byte keyStructure = 0x00;

            switch (keyType)
            {
                case KeyType.CardKey:
                    break;
                case KeyType.ReaderKey:
                    keyStructure |= (1 << 7);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null);
            }

            switch (transmissionType)
            {
                case Transmission.Plain:
                    break;
                // ToDo add more cases for transmission of encrypted key content if any reader support this feature
                default:
                    throw new ArgumentOutOfRangeException(nameof(transmissionType), transmissionType, null);
            }

            switch (persistence)
            {
                case Persistence.Volatile:
                    break;
                case Persistence.Persistent:
                    keyStructure |= (1 << 5);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(persistence), persistence, null);
            }

            switch (keyLength)
            {
                case KeyLength._6Bytes:
                case KeyLength._8Bytes:
                case KeyLength._16Bytes:
                case KeyLength._24Bytes:
                case KeyLength._32Bytes:
                    if ((int) keyLength == Utilities.BinaryHelper.ConvertOctetStringToBytes(key).Length)
                    {
                        break;
                    }
                    throw new ArgumentException("Key length incorrect", nameof(key));
                default:
                    throw new ArgumentOutOfRangeException(nameof(keyLength), keyLength, null);
            }
            return "FF82" + keyStructure.ToString("X2") + keySlot.ToString("X2") + ((byte)keyLength).ToString("X2") + key;
        }
    }

    public class GetDataCommand
    {
        /// <summary>
        /// Specifies the type of command to be used,
        /// </summary>
        public enum Type : byte
        {
            /// <summary>
            /// GetData command returns:
            /// ISO/IEC 14443 Type A            4, 7 or 10-byte UID; 
            /// ISO/IEC 14443 Type B            4-byte PUPI;
            /// ISO/IEC 15693                   8-byte UID;
            /// FeliCa                          8-byte IDm;
            /// iCLASS 14443 Type B / 15693     8-byte CSN;
            /// </summary>
            Default = 0x00,
            /// <summary>
            /// Works only for ISO/IEC 14443 Type A cards, number of historical bytes returned is limited to 15.
            /// </summary>
            Specific = 0x01,
        };

        /// <summary>
        /// This command is used to retrieve certain specific information relating to the card itself such as card
        /// serial number, rather than data on the card itself.
        /// </summary>
        public string GetApdu(Type comandType, byte expectedLength = 0x00)
        {
            return "FFCA" + ((byte) comandType).ToString("X2") + "00" + expectedLength.ToString("X2");
        }
    }

    public class GeneralAuthenticateCommand
    {
        public enum MifareKeyType : byte
        {
            MifareKeyA = 0x60,
            MifareKeyB = 0x61
        };
        public enum iClassKeyType : byte
        {
            PicoPassDebitKeyKD = 0x00,
            PicoPassCreditKeyKC = 0x01
        };
        public enum ImplicitSelection
        {
            On,
            Off
        };
        /// <summary>
        /// This command allows the user to authenticate a credential. Before using this command the correct
        /// keys must have been loaded to the relevant key slot. 
        /// </summary>
        /// <param name="address"> The Block number counted from 0 to [19 (MINI), 63 (1K), 127 (2K), 255 (4k)].</param>
        /// <returns></returns>
        public string GetMifareApdu(byte address, MifareKeyType keyType, byte keySlot)
        {
            return "FF8600000501"+ "00" + address.ToString("X2") + ((byte)keyType).ToString("X2") + keySlot.ToString("X2");
        }
        /// <summary>
        /// This command allows the user to authenticate a credential. Before using this command the correct keys must have been loaded to the relevant key slot. 
        /// For iCLASS keys these keys are preloaded onto the reader, so the application must just select the correct key number for the area they are attempting to access.
        /// Note: The reader will not allow the user to authenticate the HID area of a card outside of a secure session.
        /// </summary>
        public string GetiClassApdu(BookNumber bookNumber, PageNumber pageNumber, ImplicitSelection implicitSelection, iClassKeyType keyType, byte keySlot)
        {
            int address = 0x0000;

            address |= (int)pageNumber;
            address |= bookNumber != BookNumber.Book0 ? (1 << 8) : 0;
            address |= implicitSelection == ImplicitSelection.On ? (1 << 9) : 0;

            return "FF8600000501" + address.ToString("X4") + ((byte)keyType).ToString("X2") + keySlot.ToString("X2");
        }
    }
    
    public class ReadBinaryCommand
    {
        public enum ReadOption : byte { WithoutSelect = 0x00, WithSelect = (1 << 4), WithDesDecrypted = (1 << 6), WithTripleDesDecrypted = (1 << 7) | (1 << 6) };
        /// <summary>
        /// Returns full apdu command to read binary data form iClass card.
        /// </summary>
        /// <param name="option"> Read binary pcsc command parametr.</param>
        /// <param name="blockNumber"></param>
        /// <param name="expectedLength"></param>
        /// <param name="bookNumber"> Used only if option set to <see cref="ReadOption.WithSelect"/></param>
        /// <param name="pageNumber"> Used only if option set to <see cref="ReadOption.WithSelect"/></param>
        /// <returns></returns>
        public string GetiClassReadApdu(ReadOption option, byte blockNumber, byte expectedLength, BookNumber bookNumber = BookNumber.Book0, PageNumber pageNumber = PageNumber.Page0)
        {
            if (option != ReadOption.WithSelect)
                return "FFB0" + ((byte) option).ToString("X2") + blockNumber.ToString("X2") +
                       expectedLength.ToString("X2");

            byte address = 0x00;

            address |= (byte)option;
            address |= bookNumber != BookNumber.Book0 ? (byte)(1 << 3) : (byte)0;
            address |= (byte)pageNumber;

            return "FFB0" + address.ToString("X2") + blockNumber.ToString("X2") + expectedLength.ToString("X2");
        }

        public string GetMifareReadApdu(byte blockNumber, byte expectedLength)
        {
            return "FFB0" + "00" + blockNumber.ToString("X2") + expectedLength.ToString("X2");
        }

        public string GetApdu(byte msb, byte lsb, byte expectedLength)
        {
            return "FFB0" + msb.ToString("X2") + lsb.ToString("X2") + expectedLength.ToString("X2");
        }
    }
    
    public class UpdateBinaryCommand
    {
        public enum Type
        {
            /// <summary>
            /// Update plain data.
            /// </summary>
            Plain = 0,
            /// <summary>
            /// Iclass card only, update DES encrypted.
            /// </summary>
            DES = 1 << 6,
            /// <summary>
            /// Iclass card only, update 3DES encrypted.
            /// </summary>
            TripleDES = (1 << 6) | (1 << 7),
        };

        public string GetApdu(Type commandType, byte blockNumber, string data)
        {
            data = data.Replace(" ", "").Replace("-", "");

            return "FFD6" + ((byte)commandType).ToString("X2") + blockNumber.ToString("X2") + (data.Length / 2).ToString("X2") + data;
        }
    }
    
    public class IncrementCommand
    {
        public string GetApdu(byte blockNumber, int incrementValue)
        {
            var data = BitConverter.GetBytes(incrementValue);
            if (!BitConverter.IsLittleEndian)
            {
                data = data.Reverse().ToArray();
            }
            string value = BitConverter.ToString(data).Replace("-", "");
            return "FFD400" + blockNumber.ToString("X2") + "04" + value;
        }
    }

    public class DecrementCommand
    {
        public string GetApdu(byte blockNumber, int incrementValue)
        {
            var data = BitConverter.GetBytes(incrementValue);
            if (!BitConverter.IsLittleEndian)
            {
                data = data.Reverse().ToArray();
            }
            string value = BitConverter.ToString(data).Replace("-", "");
            return "FFD800" + blockNumber.ToString("X2") + "04" + value;
        }
    }

    public class IncrementDecrementCommand
    {
        public enum OperationType : byte
        {
            Increment,
            Decrement
        };
        public string GetApdu(OperationType operation, byte blockNumber, int value)
        {
            var operationTag = string.Empty;
            switch (operation)
            {
                case OperationType.Increment:
                    operationTag = "A0";
                    break;
                case OperationType.Decrement:
                    operationTag = "A1";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
            var data = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                data = data.Reverse().ToArray();
            }
            string valueData = BitConverter.ToString(data).Replace("-", "");
            return "FFC200030B" + operationTag + "098001" + blockNumber.ToString("X2") + "8104" + valueData;
        }
    }
}
