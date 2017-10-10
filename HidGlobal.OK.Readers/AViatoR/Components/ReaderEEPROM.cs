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
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.AViatoR.Components
{
    public class ReaderEeprom
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ushort Minoffset => 0x0000;
        public ushort Maxoffset => 0x03FF;

        public string ReadCommand(ushort offset, byte dataLength)
        {
            if (offset > Maxoffset)
            {
                throw new ArgumentOutOfRangeException($"Offset parameter grater then maximum value, current: {offset}, max: {Maxoffset}.");
            }

            if (Maxoffset + 1 >= offset + dataLength)
                return "FF70076B0DA20BA009A7078102" + offset.ToString("X4") + "8201" + dataLength.ToString("X2") + "00";

            throw new ArgumentOutOfRangeException("Attempt to read data out of user eeprom space.");
        }

        public string WriteCommand(ushort offset, string dataToWrite)
        {
            if (dataToWrite == null)
            {
                throw new NullReferenceException($"String {nameof(dataToWrite)} can not be null.");
            }
            if (dataToWrite.Length % 2 != 0)
            {
                throw new ArgumentException($"String {nameof(dataToWrite)} length is not valid, expected even length.");
            }
            if (!BinaryHelper.IsValidHexString(dataToWrite))
            {
                throw new FormatException($"String {nameof(dataToWrite)} length contains illegal characters.");
            }
            if (offset > Maxoffset)
            {
                throw new ArgumentOutOfRangeException($"Offset parameter grater then maximum value, current: {offset}, max: {Maxoffset}.");
            }

            ushort dataLength = (ushort)(dataToWrite.Length / 2);

            if (0x0400 < offset + dataLength)
            {
                throw new ArgumentOutOfRangeException("Attempt to write data out of user eeprom space.");
            }

            const string header = "FF70076B";
            
            // Select data field length encoding
            if (dataLength + 12 < 128)
            {
                var tlvSelection = (dataLength + 12).ToString("X2") + "A2" + (dataLength + 10).ToString("X2") + "A1" + (dataLength + 8).ToString("X2") + "A7" + (dataLength + 6).ToString("X2") +
                                   "8102" + offset.ToString("X4") + "83" + dataLength.ToString("X2") + dataToWrite + "00";
                return header + tlvSelection;
            }

            if (dataLength + 16 < 256)
            {
                var lenFlag = ((1 << 7) | (1 << 0)).ToString("X2");
                var tlvSelection = (dataLength + 16).ToString("X2") + "A2" + lenFlag + (dataLength + 13).ToString("X2") + "A1" + lenFlag + (dataLength + 10).ToString("X2") + "A7" +
                                    lenFlag + (dataLength + 7).ToString("X2") + "8102" + offset.ToString("X4") + "83" + lenFlag + dataLength.ToString("X2") + dataToWrite + "00";
                return header + tlvSelection;
            }

            throw new ArgumentOutOfRangeException($"Single eeprom write apdu datafield cannot have more the 255 bytes of length, current apdu length: {dataLength + 16}");
        }
    }

}
