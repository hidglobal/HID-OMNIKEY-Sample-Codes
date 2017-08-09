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
using System.Linq;

namespace HidGlobal.OK.Readers.SecureSession
{
    class Counter
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

        /// <summary>
        /// Length of Secure Session Counter in bytes.
        /// </summary>
        const int _sscLength = 0x10;
        /// <summary>
        /// Secure Session Counter.
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// Secure Session Counter Constructor.
        /// </summary>
        /// <param name="hostNonce"> 16 byte array randomized by the host.</param>
        /// <param name="readerNonce"> 16 byte array randomized by the reader.</param>
        public Counter(byte[] hostNonce, byte[] readerNonce)
        {
            if (hostNonce.Length != _sscLength)
                throw new ArgumentException("Only allowable length of array is 16 bytes.", nameof(hostNonce));
            if (readerNonce.Length != _sscLength)
                throw new ArgumentException("Only allowable length of array is 16 bytes.", nameof(readerNonce));

            Value = ByteArrayToOctetString(hostNonce.Take(_sscLength / 2).Concat(readerNonce.Take(_sscLength / 2)).ToArray());
        }
        public Counter(string hostNonce, string readerNonce)
        {
            var tempHostNonce = OctetStringToByteArray(hostNonce);
            var tempReaderNonce = OctetStringToByteArray(readerNonce);

            if (tempHostNonce.Length != _sscLength)
                throw new ArgumentException("Only allowable length of array is 16 bytes.", nameof(hostNonce));
            if (tempReaderNonce.Length != _sscLength)
                throw new ArgumentException("Only allowable length of array is 16 bytes.", nameof(readerNonce));

            Value = ByteArrayToOctetString(tempHostNonce.Take(_sscLength / 2).Concat(tempReaderNonce.Take(_sscLength / 2)).ToArray());
        }
        /// <summary>
        /// Increments value of the counter.
        /// </summary>
        public void Increment()
        {
            byte[] temp = OctetStringToByteArray(Value);

            for (var i = temp.Length-1; i >= 0; i--)
            {
                temp[i] += 1;
                if (temp[i] != 0)
                    break;
            }

            Value = ByteArrayToOctetString(temp);
        }

    }
}