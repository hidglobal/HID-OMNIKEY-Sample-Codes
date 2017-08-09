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
using System.Security.Cryptography;

namespace HidGlobal.OK.Readers.SecureSession
{
    class CMac
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static byte[] OctetStringToByteArray(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                log.Error("OctetStringToByteArray function parameter is null or whitespace.");
                return null;
            }
            // Remove delimeters
            hex = hex.Replace(" ", "");
            hex = hex.Replace("-", "");

            if (hex.Length % 2 != 0)
                hex = hex.Insert(0, "0");
            try
            {
                return Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
            }
            catch (Exception error)
            {
                log.Error(null, error);
                return null;
            }
        }
        private static string ByteArrayToOctetString(byte[] bytes)
        {
            if (bytes == null)
            {
                log.Error("ByteArrayToOctetString function parameter is null.");
                return null;
            }
            try
            {
                return bytes.Select(x => x.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
            }
            catch (Exception error)
            {
                log.Error(null, error);
                return null;
            }
        }

        static byte[] AesEncrypt(byte[] key, byte[] iv, byte[] data)
        {
            byte[] result;

            using (var memoryStream = new MemoryStream())
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }

        static byte[] Rol(byte[] parameter)
        {
            var result = new byte[parameter.Length];
            byte carry = 0;
            for (var i = parameter.Length - 1; i >= 0; i--)
            {
                var parameter2 = (ushort)(parameter[i] << 1);
                result[i] = (byte)((parameter2 & 0xFF) + carry);
                carry = (byte)((parameter2 & 0xFF00) >> 8);
            }
            return result;
        }

        public static string GetHashTag(string macKey, string dataToMac)
        {
            var mac = OctetStringToByteArray(macKey);
            var data = OctetStringToByteArray(dataToMac);
            byte[] hash = GetHashTag(mac, data);
            return ByteArrayToOctetString(hash);
        }

        public static byte[] GetHashTag(byte[] macKey, byte[] dataToMac)
        {
            var key = new byte[macKey.Length];
            var data = new byte[dataToMac.Length];

            Array.ConstrainedCopy(macKey, 0, key, 0, macKey.Length);
            Array.ConstrainedCopy(dataToMac, 0, data, 0, dataToMac.Length);

            // SubKey generation
            // AES-128 with key K is applied to an all-zero input block.
            var L = AesEncrypt(key, new byte[16], new byte[16]);
            // K1 is derived through the following operation:
            // If the most significant bit of L is equal to 0, K1 is the left-shift of L by 1 bit.
            // Otherwise, K1 is the exclusive-OR of const_Rb and the left-shift of L by 1 bit.
            var firstSubkey = Rol(L); 
            if ((L[0] & 0x80) == 0x80)
                firstSubkey[15] ^= 0x87;
            // K2 is derived through the following operation:
            // If the most significant bit of K1 is equal to 0, K2 is the left-shift of K1 by 1 bit.
            // Otherwise, K2 is the exclusive-OR of const_Rb and the left-shift of K1 by 1 bit.
            var secondSubkey = Rol(firstSubkey); 
            if ((firstSubkey[0] & 0x80) == 0x80)
                secondSubkey[15] ^= 0x87;
            // MAC computing
            // If the size of the input message block is equal to a positive multiple of the block size (namely, 128 bits),
            // the last block shall be exclusive-OR'ed with K1 before processing
            // Otherwise, the last block shall be padded with 10^i and exclusive-OR'ed with K2
            if (((data.Length != 0) && (data.Length % 16 == 0)) == true)
            {
                for (var j = 0; j < firstSubkey.Length; j++)
                    data[data.Length - 16 + j] ^= firstSubkey[j];
            }
            else
            {
                var padding = new byte[16 - data.Length % 16];
                padding[0] = 0x80;
                data = data.Concat<byte>(padding.AsEnumerable()).ToArray();
                for (var j = 0; j < secondSubkey.Length; j++)
                    data[data.Length - 16 + j] ^= secondSubkey[j];
            }
            // The result of the previous process will be the input of the last encryption.
            var encResult = AesEncrypt(key, new byte[16], data);
            var hashValue = new byte[16];
            Array.Copy(encResult, encResult.Length - hashValue.Length, hashValue, 0, hashValue.Length);
            return hashValue;
        }
    }
}
