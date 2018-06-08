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
using System.Security.Cryptography;

namespace HidGlobal.OK.Readers.SecureSession
{
    public class CounterModeCryptoTransform : ICryptoTransform
    {
        private readonly byte[] _counter;
        private readonly ICryptoTransform _counterEncryptor;
        private readonly Queue<byte> _xorMask = new Queue<byte>();
        private readonly SymmetricAlgorithm _symetricAlgorithm;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricAlgorithm"></param>
        /// <param name="key"></param>
        /// <param name="counter"> Initial value of the counter.</param>
        public CounterModeCryptoTransform(SymmetricAlgorithm symmetricAlgorithm, byte[] key, byte[] counter)
        {
            if (symmetricAlgorithm == null)
                throw new ArgumentNullException(nameof(symmetricAlgorithm));

            _symetricAlgorithm = symmetricAlgorithm;

            if (counter == null)
                throw new ArgumentNullException(nameof(counter));
            if (counter.Length != _symetricAlgorithm.BlockSize / 8)
                throw new ArgumentException($"Counter size must be the same as block size (actual: {counter.Length}, expected: {symmetricAlgorithm.BlockSize / 8}");

            _counter = counter;

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _counterEncryptor = symmetricAlgorithm.CreateEncryptor(key, new byte[_symetricAlgorithm.BlockSize / 8]);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] output = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            for (int i = 0; i < inputCount; i++)
            {
                if (NeedMoreXorMaskBytes())
                    EncryptCounter();

                var mask = _xorMask.Dequeue();
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ mask);
            }
            return inputCount;
        }
        /// <summary>
        /// Checks if there are any bytes of encrypted counter left.
        /// </summary>
        /// <returns></returns>
        private bool NeedMoreXorMaskBytes() { return _xorMask.Count == 0; }
        /// <summary>
        /// Encrypts counter and add resulted data to the Xor queue.
        /// </summary>
        private void EncryptCounter()
        {
            var counterBlock = new byte[_symetricAlgorithm.BlockSize / 8];
            _counterEncryptor.TransformBlock(_counter, 0, _counter.Length, counterBlock, 0);
            IncrementCounter();

            foreach (var item in counterBlock) { _xorMask.Enqueue(item); }
        }
        /// <summary>
        /// Increments the counter.
        /// </summary>
        private void IncrementCounter()
        {
            for (var i = _counter.Length - 1; i >= 0; i--)
            {
                _counter[i] += 1;
                if (_counter[i] != 0)
                    break;
            }
        }

        public int InputBlockSize => _symetricAlgorithm.BlockSize / 8;
        public int OutputBlockSize => _symetricAlgorithm.BlockSize / 8;
        public bool CanTransformMultipleBlocks => true;
        public bool CanReuseTransform => false;
        public void Dispose() { }

    }
}
