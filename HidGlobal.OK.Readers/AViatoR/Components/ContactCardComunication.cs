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
using System.Globalization;

namespace HidGlobal.OK.Readers.AViatoR.Components
{
    public class ContactCardCommunication
    {
        public Synchronus2WBP Synchronus2Wbp => new Synchronus2WBP();
        public Synchronus3WBP Synchronus3Wbp => new Synchronus3WBP(); 
        public SynchronusI2C SynchronusI2c => new SynchronusI2C();
    }

    /// <summary>
    /// This commands allow communication with synchronous contact smart card which supports 2 Wire Bus Protocol (2WBP) such as SLE 4432/42.
    /// </summary>
    public class Synchronus2WBP
    {
        public enum ControlByte : byte
        {
            /// <summary>
            /// 2WBP Common Command: Read Main Memory
            /// </summary>
            ReadMainMemory              = (1 << 5) | (1 << 4),
            
            /// <summary>
            /// 2WBP Common Command: Update Main Memory
            /// </summary>
            UpdateMainMemory            = (1 << 5) | (1 << 4) | (1 << 3),

            /// <summary>
            /// 2WBP Common Command: Read Protection Memory
            /// </summary>
            ReadProtectionMemory        = (1 << 5) | (1 << 4) | (1 << 2),

            /// <summary>
            /// 2WBP Common Command: Write Protection Memory
            /// </summary>
            WriteProtectionMemory       = (1 << 5) | (1 << 4) | (1 << 3) | (1 << 2),

            /// <summary>
            /// 2WBP SLE 4442 only Command: Read Security Memory
            /// </summary>
            ReadSecurityMemory          = (1 << 5) | (1 << 4) | (1 << 0),

            /// <summary>
            /// 2WBP SLE 4442 only Command: Update Security Memory
            /// </summary>
            UpdateSecurityMemory        = (1 << 5) | (1 << 4) | (1 << 3) | (1 << 0),

            /// <summary>
            /// 2WBP SLE 4442 only Command: Compare Verification Data
            /// </summary>
            CompareVerificationData     = (1 << 5) | (1 << 4) | (1 << 1) | (1 << 0),
        };

        public string GetApdu(ControlByte control, byte address, byte data)
        {
            return "FF70076B07A605A003" + ((byte)control).ToString("X2") + address.ToString("X2") + data.ToString("X2") + "00";
        }
    }

    /// <summary>
    /// This commands allow communication with synchronous contact smart card which supports 3 Wire Bus Protocol (3WBP) such as SLE 4418/28.
    /// </summary>
    public class Synchronus3WBP
    {
        public enum ControlByte
        {
            WriteProtectBitWithDataComparison       = (1 << 5) | (1 << 4),
            WriteAndEraseWithProtectBit             = (1 << 5) | (1 << 4) | (1 << 0),
            WriteAndEraseWithoutProtectBit          = (1 << 5) | (1 << 4) | (1 << 1) | (1 << 0),
            Read9BitsDataWithProtectBit             = (1 << 3) | (1 << 2),
            Read8BitsDataWithoutProtectBit          = (1 << 3) | (1 << 2) | (1 << 1),
            ReadErrorCounter                        = (1 << 7) | (1 << 6) | (1 << 3) | (1 << 2) | (1 << 1),
            WriteErrorCounter                       = (1 << 7) | (1 << 6) | (1 << 5) | (1 << 4) | (1 << 1),
            ResetErrorCounter                       = (1 << 7) | (1 << 6) | (1 << 5) | (1 << 4) | (1 << 1) | (1 << 0),
            VerifyPinByte                           = (1 << 7) | (1 << 6) | (1 << 3) | (1 << 2) | (1 << 0),
        };

        public string GetApdu(ControlByte control, ushort address, byte data)
        {
            byte fullControlByte = (byte)control;
            switch (control)
            {
                case ControlByte.WriteAndEraseWithProtectBit:
                case ControlByte.WriteAndEraseWithoutProtectBit:
                case ControlByte.WriteProtectBitWithDataComparison:
                case ControlByte.Read9BitsDataWithProtectBit:
                case ControlByte.Read8BitsDataWithoutProtectBit:
                    fullControlByte |= (byte) (((1 << 9 | 1 << 8) & address) >> 2);
                    break;

                case ControlByte.WriteErrorCounter:
                    address = 0xFD;
                    break;

                case ControlByte.ResetErrorCounter:
                    address = 0xFD;
                    data = 0xFF;
                    break;

                case ControlByte.ReadErrorCounter:
                    address = 0xFD;
                    data = 0x00;
                    break;

                case ControlByte.VerifyPinByte:
                    if (!(address == 0x03FE || address == 0x03FF))
                        return null;

                    break;

                default:
                    return null;
            }

            return "FF70076B07A605A103" + fullControlByte.ToString("X2") + ((byte)address).ToString("X2") + data.ToString("X2") + "00";
        }

        public string WriteErrorCounterApdu(byte bitMask)
        {
            return GetApdu(ControlByte.WriteErrorCounter, 0, bitMask);
        }
        public string ResetErrorCounterApdu()
        {
            return GetApdu(ControlByte.ResetErrorCounter, 0, 0);
        }
        public string ReadErrorCounterApdu()
        {
            return GetApdu(ControlByte.ReadErrorCounter, 0, 0);
        }
        public string VerifyFirstPinByte(byte firstPinByte)
        {
            return GetApdu(ControlByte.VerifyPinByte, 0x03FE, firstPinByte);
        }
        public string VerifySecondPinByte(byte secondPinByte)
        {
            return GetApdu(ControlByte.VerifyPinByte, 0x03FF, secondPinByte);
        }

    }
    
    /// <summary>
    /// This commands allow communication with synchronous contact smart card which supports I2C Bus Protocol such as AT24C01/02/04/…/1024.
    /// </summary>
    public class SynchronusI2C
    {
        public enum MemorySize
        {
            _256    = 0x000100,
            _512    = 0x000200,
            _1024   = 0x000400,
            _2048   = 0x000800,
            _4096   = 0x001000,
            _8192   = 0x002000,
            _16384  = 0x004000,
            _32768  = 0x008000,
            _65536  = 0x010000,
            _131072 = 0x020000,
        };
        public string GetReadCommandApdu(MemorySize cardMemorySize, int address, byte numberOfBytesToRead)
        {
            if ((int) cardMemorySize <= address)
                throw new ArgumentOutOfRangeException(nameof(address), address, $"Address out of card memory address range, selected card memory size: {(int) cardMemorySize} bytes");

            var i2CCommand = string.Empty;
            byte numberOfAddressBytes;
            const byte readFlag = 0x01;
            byte deviceAddress;
            byte subAddress1;
            byte subAddress2;

            switch (cardMemorySize)
            {
                case MemorySize._256:
                    numberOfAddressBytes = 2;
                    deviceAddress = (byte) (0xA0 | readFlag);
                    subAddress1 = (byte) (address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._512:
                    numberOfAddressBytes = 2;
                    deviceAddress = (byte) (0xA0 | readFlag | ((address & 0x000100) >> 7));
                    subAddress1 = (byte) (address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._1024:
                    numberOfAddressBytes = 2;
                    deviceAddress = (byte) (0xA0 | readFlag | ((address & 0x000300) >> 7));
                    subAddress1 = (byte) (address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._2048:
                    numberOfAddressBytes = 2;
                    deviceAddress = (byte) (0xA0 | readFlag | ((address & 0x000700) >> 7));
                    subAddress1 = (byte) (address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._4096:
                case MemorySize._8192:
                case MemorySize._16384:
                case MemorySize._32768:
                case MemorySize._65536:
                    numberOfAddressBytes = 3;
                    deviceAddress = (byte) (0xA0 | readFlag);
                    subAddress1 = (byte) ((address & 0xFF00) >> 8);
                    subAddress2 = (byte) (address & 0x00FF);
                    break;
                case MemorySize._131072:
                    numberOfAddressBytes = 3;
                    deviceAddress = (byte) (0xA0 | readFlag | ((address & 0x010000) >> 15));
                    subAddress1 = (byte) ((address & 0xFF00) >> 8);
                    subAddress2 = (byte) (address & 0x00FF);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardMemorySize), cardMemorySize, null);
            }
            i2CCommand = numberOfAddressBytes.ToString("X2") + numberOfBytesToRead.ToString("X2") + deviceAddress.ToString("X2") + subAddress1.ToString("X2") + subAddress2.ToString("X2");
            var len = i2CCommand.Length / 2;
            return "FF70076B" + (len + 4).ToString("X2") + "A6" + (len + 2).ToString("X2") + "A2" + len.ToString("X2") + i2CCommand + "00";
        }
        public string GetWriteCommandApdu(MemorySize cardMemorySize, int address, byte numberOfBytesToWrite, string dataOctetString)
        {
            if ((int)cardMemorySize <= address)
                throw new ArgumentOutOfRangeException(nameof(address), address, $"Address out of card memory address range, selected card memory size: {(int)cardMemorySize} bytes");

            var i2CCommand = string.Empty;
            byte numberOfAddressBytes;
            byte deviceAddress;
            byte subAddress1;
            byte subAddress2;

            switch (cardMemorySize)
            {
                case MemorySize._256:
                    numberOfAddressBytes = 2;
                    deviceAddress = 0xA0;
                    subAddress1 = (byte)(address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._512:
                    numberOfAddressBytes = 2;
                    deviceAddress = (byte)(0xA0 | ((address & 0x000100) >> 7));
                    subAddress1 = (byte)(address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._1024:
                    numberOfAddressBytes = 2;
                    deviceAddress = (byte)(0xA0 | ((address & 0x000300) >> 7));
                    subAddress1 = (byte)(address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._2048:
                    numberOfAddressBytes = 2;
                    deviceAddress = (byte)(0xA0 | ((address & 0x000700) >> 7));
                    subAddress1 = (byte)(address & 0x00FF);
                    subAddress2 = 0x00;
                    break;
                case MemorySize._4096:
                case MemorySize._8192:
                case MemorySize._16384:
                case MemorySize._32768:
                case MemorySize._65536:
                    numberOfAddressBytes = 3;
                    deviceAddress = 0xA0;
                    subAddress1 = (byte)((address & 0xFF00) >> 8);
                    subAddress2 = (byte)(address & 0x00FF);
                    break;
                case MemorySize._131072:
                    numberOfAddressBytes = 3;
                    deviceAddress = (byte)(0xA0 | ((address & 0x010000) >> 15));
                    subAddress1 = (byte)((address & 0xFF00) >> 8);
                    subAddress2 = (byte)(address & 0x00FF);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardMemorySize), cardMemorySize, null);
            }

            if (dataOctetString.Length % 2 != 0)
                throw new ArgumentOutOfRangeException(nameof(dataOctetString), dataOctetString, "String length not even.");

            for (int i = 0; i < dataOctetString.Length / 2; i++)
            {
                byte data;
                if (!byte.TryParse(dataOctetString.Substring(2 * i, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out data))
                    throw new ArgumentOutOfRangeException(nameof(dataOctetString), dataOctetString, "String can contain only hex numbers.");
            }
            
            i2CCommand = numberOfAddressBytes.ToString("X2") + numberOfBytesToWrite.ToString("X2") + deviceAddress.ToString("X2") + subAddress1.ToString("X2") + subAddress2.ToString("X2") + dataOctetString;
            var len = i2CCommand.Length / 2;
            return "FF70076B" + (len + 4).ToString("X2") + "A6" + (len + 2).ToString("X2") + "A2" + len.ToString("X2") + i2CCommand + "00";
        }
        public string GetApdu(string i2CCommand)
        {
            i2CCommand = i2CCommand.Replace(" ", "").Replace("-", "");
            var len = i2CCommand.Length / 2;
            return "FF70076B" + (len + 4).ToString("X2") + "A6" + (len + 2).ToString("X2") + "A2" + len.ToString("X2") +
                   i2CCommand + "00";
        }
    }
}
