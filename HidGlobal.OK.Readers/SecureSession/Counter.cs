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
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.SecureSession
{
    internal class Counter
    {
        /// <summary>
        /// Length of Secure Session Counter in bytes.
        /// </summary>
        private const int SscLength = 0x10;
        /// <summary>
        /// Secure Session Counter.
        /// </summary>
        public byte[] Value { get; private set; }
        /// <summary>
        /// Secure Session Counter Constructor.
        /// </summary>
        /// <param name="hostNonce"> 16 byte array randomized by the host.</param>
        /// <param name="readerNonce"> 16 byte array randomized by the reader.</param>
        public Counter(byte[] hostNonce, byte[] readerNonce)
        {
            if (hostNonce.Length != SscLength)
                throw new ArgumentException("Only allowable length of array is 16 bytes.", nameof(hostNonce));

            if (readerNonce.Length != SscLength)
                throw new ArgumentException("Only allowable length of array is 16 bytes.", nameof(readerNonce));

            Value = hostNonce.Take(SscLength / 2).Concat(readerNonce.Take(SscLength / 2)).ToArray();
        }

        public Counter(string hostNonce, string readerNonce) : this(BinaryHelper.ConvertOctetStringToBytes(hostNonce),
            BinaryHelper.ConvertOctetStringToBytes(readerNonce)) { }
        
        /// <summary>
        /// Increments value of the counter.
        /// </summary>
        public void Increment()
        {
            var temp = Value;

            for (var i = temp.Length-1; i >= 0; i--)
            {
                temp[i] += 1;
                if (temp[i] != 0)
                    break;
            }

            Value = temp;
        }

    }
}