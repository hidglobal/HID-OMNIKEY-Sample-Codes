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
using System.IO;
using System.Linq;
using HidGlobal.OK.Readers.AViatoR.Components;

namespace HidGlobal.OK.Readers.Utilities
{
    public static class BinaryHelper
    {
        public static byte[] ConvertOctetStringToBytes(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException($"String {nameof(hex)} can not be null.");
            }
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException($"String {nameof(hex)} length is not valid for byte conversion.");
            }
            if (!IsValidHexString(hex))
            {
                throw new FormatException($"String {nameof(hex)} length contains illegal characters.\n{nameof(hex)}: \"{hex}\"");
            }

            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string ConvertBytesToOctetString(byte[] data)
        {
            return data.Select(x => x.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
        }

        public static IList<byte> ConvertNullTerminatedByteArrayFromString(System.Text.Encoding characterEncoding, string data)
        {
            if (characterEncoding == null)
                throw new ArgumentNullException(nameof(characterEncoding));

            if (data == null)
                throw new ArgumentNullException(nameof(data));


            var convertedItem = new List<byte>();

            convertedItem.AddRange(characterEncoding.GetBytes(data));

            // terminate with null
            convertedItem.Add(0);

            return convertedItem;
        }
        
        public static IList<byte> ConvertMultiNullTerminatedByteArrayFromStringEnumerable(System.Text.Encoding characterEncoding, IEnumerable<string> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            
            var result = new List<byte>();

            foreach (var element in enumerable)
                result.AddRange(ConvertNullTerminatedByteArrayFromString(characterEncoding, element));
            
            // terminate with null
            result.Add(0);

            return result;
        }

        public static IList<string> ConvertMultiNullTerminatedStringFromBytesToStringArray(System.Text.Encoding characterEncoding, IEnumerable<byte> multiNullTerminatedBytes)
        {
            if (multiNullTerminatedBytes == null)
                throw new ArgumentNullException(nameof(multiNullTerminatedBytes));

            if (characterEncoding == null)
                throw new ArgumentNullException(nameof(characterEncoding));

            var multiNullTerminatedString = characterEncoding.GetString(multiNullTerminatedBytes.ToArray());

            return multiNullTerminatedString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int ReadLength(this BinaryReader reader)
        {
            byte length = reader.ReadByte();

            if ((length & 0x80) != 0)
            {
                // TODO: KW: this implementation assumes that length can be maximally 4 bytes long. Can it be longer (in practice)? Do we need to consider such cases?
                // TODO: Not sure if it read Lc field or tlv encoded value lenght ???

                byte size = (byte)(length & (0xff >> 1));
                byte[] bytes = reader.ReadBytes(size);

                Array.Reverse(bytes);
                Array.Resize(ref bytes, 4);

                return BitConverter.ToInt32(bytes, 0);
            }
            else
            {
                return length;
            }
        }
        
        public enum ByteOrder
        {
            MsbFirst,
            LsbFirst,
        }

        public static void WriteApduLcField(this BinaryWriter writer, int payloadLength, ByteOrder byteOrder = ByteOrder.LsbFirst)
        {
            var lcField = GetApduLcField(payloadLength);

            if (byteOrder == ByteOrder.LsbFirst)
                lcField = lcField.Reverse().ToArray();
            
            writer.Write(lcField);
        }

        private static byte[] GetApduLcField(int payloadLength)
        {
            var lcField = BitConverter.IsLittleEndian
                ? BitConverter.GetBytes(payloadLength).Reverse()
                : BitConverter.GetBytes(payloadLength);

            if (payloadLength > 65535)
            {
                throw new ArgumentOutOfRangeException(
                    $"Payload size is greater then max value supported by extended APDU, payload size:{payloadLength}, max size 65535");
            }
            else if (payloadLength > 255)
            {
                // Extended APDU -> 0 - 65535 bytes long payload -> three bytes long Lc field, first byte 0x00 length encoded on the last to bytes
                lcField = lcField.Skip(1); // Take last 3 bytes
            }
            else if (payloadLength > 0)
            {
                // Short APDU -> 0 - 255 bytes long payload -> one byte long Lc field,
                lcField = lcField.Skip(3); // Take last byte
            }
            else
            {
                // Lc field absent in a APDU structure
                lcField = lcField.Skip(4);
            }
            return lcField.ToArray();
        }

        public static string ReadOctetString(this BinaryReader reader)
        {
            int length = ReadLength(reader);
            if (length == 0)
            {
                return null;
            }
            else
            {
                var bytes = reader.ReadBytes(length);
                return bytes.Select(x => x.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
            }
        }

        public static bool IsValidHexString(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static byte[] GetTlvLength(long valueLength, ByteOrder byteOrder = ByteOrder.LsbFirst)
        {
            const byte extendedLengthTag = 0x80;
            var lengthField = new List<byte>();

            var lengthData = BitConverter.IsLittleEndian
                ? BitConverter.GetBytes(valueLength).Reverse()
                : BitConverter.GetBytes(valueLength);

            if (valueLength > 127)
            {
                var lengthBytes = lengthData.SkipWhile(b => b == 0).ToArray();
                lengthField.Add((byte)(lengthBytes.Length | extendedLengthTag));
                lengthField.AddRange(lengthBytes);
            }
            else if (valueLength > 0)
            {
                lengthField.Add((byte)valueLength);
            }
            else
            {
                lengthField.Add(0x00);
            }
            if (byteOrder == ByteOrder.LsbFirst)
                lengthField.Reverse();

            return lengthField.ToArray();
        }

        /// <summary>
        /// Write TLV encoded data, assuming that memory stream held by binary writer contains value bytes.
        /// </summary>
        public static void WriteTlvEncodedData(this BinaryWriter writer, byte tag, ByteOrder byteOrder = ByteOrder.LsbFirst)
        {
            if (byteOrder == ByteOrder.MsbFirst)
            {
                var length = writer.BaseStream.Length;
                writer.Write(tag);
                writer.Write(GetTlvLength(length, ByteOrder.MsbFirst));
            }
            else
            {
                writer.Write(GetTlvLength(writer.BaseStream.Length));
                writer.Write(tag);
            }
        }

        public static void Write(this BinaryWriter writer, CharacterDiff characterDiff)
        {
            writer.Write(characterDiff.Character);
            writer.Write(characterDiff.Modifiers);
            writer.Write(characterDiff.KeyVal);
        }

        public static CharacterDiff ReadCharacterDiff(this BinaryReader reader)
        {
            return new CharacterDiff()
            {
                Character = reader.ReadByte(),
                Modifiers = reader.ReadByte(),
                KeyVal = reader.ReadByte(),
            };
        }

        public static byte[] ConvertToBytes(ushort value, ByteOrder endianness)
        {
            var isDesiredOutputLsb = endianness == ByteOrder.LsbFirst;

            var output = isDesiredOutputLsb == BitConverter.IsLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();

            return output;
        }

        /// <summary>
        /// Convert configuration apdu set request as octet string to IApduCommand object, assuming that the request is build as short apdu request.
        /// </summary>
        public static IApduCommand ConvertOctetStringToIApduInterface(string apdu)
        {
            var header = HidGlobal.OK.Readers.Utilities.BinaryHelper.ConvertOctetStringToBytes(apdu.Substring(0, 8));
            var payload =
                HidGlobal.OK.Readers.Utilities.BinaryHelper.ConvertOctetStringToBytes(apdu.Substring(10,
                    apdu.Length - 12));

            return new ApduCommand(ApduFormat.Short, header, payload);
        }

    }
}