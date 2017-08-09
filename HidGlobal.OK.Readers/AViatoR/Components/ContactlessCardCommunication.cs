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


    public class LoadKeyCommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const byte MifareKeyLastSlot = 31;
        private const byte iClassNonvolatileKeyLastSlot = 63;
        private const byte iClassDESKeySlot = 64;
        private const byte iClassVolatileKeyLastSlot = 66;
        private const byte iClass3DESKeySlot = 67;
        private const byte SecureSessionLastKeySlot = 73;
        private const byte MifareKeySize = 6;
        private const byte iClassNonvolatileKeySize = 8;
        private const byte iClassDESKeySize = 8;
        private const byte iClassVolatileKeySize = 8;
        private const byte iClass3DESKeySize = 16;
        private const byte SecureSessionKeySize = 16;

        public string GetApdu(byte keySlot, string key)
        {
            byte keyStructure = 0x00;
            byte keyLength;
            bool isCardKey;
            bool isStoredInVolatileMemory;
            key = key.Replace(" ", "").Replace("-", "");

            if (keySlot <= MifareKeyLastSlot)
            {
                keyLength = MifareKeySize;
                isCardKey = true;
                isStoredInVolatileMemory = false;
            }
            else if (keySlot <= iClassNonvolatileKeyLastSlot)
            {
                keyLength = iClassNonvolatileKeySize;
                isCardKey = true;
                isStoredInVolatileMemory = false;
            }
            else if (keySlot <= iClassDESKeySlot)
            {
                keyLength = iClassDESKeySize;
                isCardKey = false;
                isStoredInVolatileMemory = false;
            }
            else if (keySlot <= iClassVolatileKeyLastSlot)
            {
                keyLength = iClassVolatileKeySize;
                isCardKey = true;
                isStoredInVolatileMemory = true;
            }
            else if (keySlot <= iClass3DESKeySlot)
            {
                keyLength = iClass3DESKeySize;
                isCardKey = false;
                isStoredInVolatileMemory = false;
            }
            else if (keySlot <= SecureSessionLastKeySlot)
            {
                keyLength = SecureSessionKeySize;
                isCardKey = false;
                isStoredInVolatileMemory = false;
            }
            else
            {
                log.Error($"LoadKey function parameter {nameof(keySlot)} value cannot be higher then {SecureSessionLastKeySlot},\n current value: {keySlot}");
                return null;
            }

            if (!isCardKey)
                keyStructure |= (1 << 7);

            if (!isStoredInVolatileMemory)
                keyStructure |= (1 << 5);

            if (2 * keyLength == key.Length)
                return "FF82" + keyStructure.ToString("X2") + keySlot.ToString("X2") + keyLength.ToString("X2") + key;

            log.Error($"LoadKey function parameter {nameof(key)} length is incorrect.\nExpected: {keyLength} bytes\nActual: {key.Replace(" ", "").Length} bytes");
            return null;
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
        public enum MifareKeyType : byte { MifareKeyA = 0x60, MifareKeyB = 0x61 };
        public enum IClassKeyType : byte { PicoPassDebitKeyKD = 0x00, PicoPassCreditKeyKC = 0x01 };

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
        public string GetiClassApdu(BookNumber bookNumber, PageNumber pageNumber, bool authenticateWithImplicitSelect, IClassKeyType keyType, byte keySlot)
        {
            int address = 0x0000;

            address |= (int)pageNumber;
            address |= bookNumber != BookNumber.Book0 ? (1 << 8) : 0;
            address |= authenticateWithImplicitSelect ? (1 << 9) : 0;

            return "FF8600000501" + address.ToString("X4") + ((byte)keyType).ToString("X2") + keySlot.ToString("X2");
        }
    }
    
    public class ReadBinaryCommand
    {
        public enum ReadOption : byte { WithoutSelect = 0x00, WithSelect = (1 << 4), WithDesDecrypted = (1 << 6), WithTripleDesDecrypted = (1 << 7) | (1 << 6) };

        private string GetiClassReadApdu(ReadOption option, BookNumber bookNumber, PageNumber pageNumber, byte blockNumber, byte expectedLength)
        {
            if (option == ReadOption.WithSelect)
            {
                byte address = 0x00;

                address |= (byte)option;
                address |= bookNumber != BookNumber.Book0 ? (byte)(1 << 3) : (byte)0;
                address |= (byte)pageNumber;

                return "FFB0" + address.ToString("X2") + blockNumber.ToString("X2") + expectedLength.ToString("X2");
            }
            return "FFB0" + ((byte)option).ToString("X2") + blockNumber.ToString("X2") + expectedLength.ToString("X2");
        }

        public string GetMifareReadApdu(byte blockNumber, byte expectedLength)
        {
            return "FFB0" + "00" + blockNumber.ToString("X2") + expectedLength.ToString("X2");
        }

        public string GetiClassReadWithoutSelectApdu(byte blockNumber, byte expectedLength = 0)
        {
            return GetiClassReadApdu(ReadOption.WithoutSelect, (BookNumber)(0xFF), (PageNumber)(0xFF), blockNumber, expectedLength);
        }

        public string GetiClassReadWithSelectApdu(BookNumber book, PageNumber page, byte blockNumber, byte expectedLength = 0)
        {
            return GetiClassReadApdu(ReadOption.WithSelect, book, page, blockNumber, expectedLength);
        }

        public string GetiClassReadWithDesDecryptedApdu(byte blockNumber, byte expectedLength = 0)
        {
            return GetiClassReadApdu(ReadOption.WithDesDecrypted, (BookNumber)(0xFF), (PageNumber)(0xFF), blockNumber, expectedLength);
        }

        public string GetiClassReadWithTripleDesDecryptedApdu(byte blockNumber, byte expectedLength = 0)
        {
            return GetiClassReadApdu(ReadOption.WithTripleDesDecrypted, (BookNumber)(0xFF), (PageNumber)(0xFF), blockNumber, expectedLength);
        }
    }
    
    public class UpdateBinaryCommand
    {
        public enum Type
        {
            /// <summary>
            /// Update plain data.
            /// </summary>
            Default = 0,
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string GetApdu(byte blockNumber, string data)
        {
            data = data.Replace(" ", "").Replace("-", "");
            if (data.Length != 8)
            {
                log.Error($"Parametr {nameof(data)} expected length: 8 characters, current: {data.Length}");
                return null;
            }

            return "FFD400" + blockNumber.ToString("X2") + "04" + data;
        }
    }

    public class DecrementCommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string GetApdu(byte blockNumber, string data)
        {
            data = data.Replace(" ", "").Replace("-", "");
            if (data.Length != 8)
            {
                log.Error($"Parametr {nameof(data)} expected length: 8 characters, current: {data.Length}");
                return null;
            }

            return "FFD800" + blockNumber.ToString("X2") + "04" + data;
        }
    }
}
