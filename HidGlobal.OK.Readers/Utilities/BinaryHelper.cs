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

namespace HidGlobal.OK.Readers.Utilities
{
    public static class BinaryHelper
    {
        public static byte[] ConvertOctetStringToBytes(string hex)
        {
            if (hex == null)
            {
                throw new NullReferenceException($"String {nameof(hex)} can not be null.");
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
            if (data == null)
            {
                throw new NullReferenceException($"Array {nameof(data)} can not be null.");
            }

            return data.Select(x => x.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
        }

        public static int ReadLength(this BinaryReader reader)
        {
            if (reader == null)
            {
                throw new NullReferenceException("reader can't be null");
            }

            // todo: KW: implement scenario when length exceepds 128
            return reader.ReadByte();
        }

        public static string ReadOctetString(this BinaryReader reader)
        {
            if (reader == null)
            {
                throw new NullReferenceException("reader can't be null");
            }

            int length = ReadLength(reader);
            if (length == 0)
            {
                return null;
            }
            else
            {
                if (length > reader.BaseStream.Length - reader.BaseStream.Position)
                    throw new EndOfStreamException();

                var bytes = reader.ReadBytes(length);
                return bytes.Select(x => x.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
            }
        }

        public static bool IsValidHexString(string input)
        {
            if (input == null)
            {
                throw new NullReferenceException("input can't be null");
            }

            return System.Text.RegularExpressions.Regex.IsMatch(input, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        
    }
}