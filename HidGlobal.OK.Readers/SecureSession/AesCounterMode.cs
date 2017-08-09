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
using System.Security.Cryptography;

namespace HidGlobal.OK.Readers.SecureSession
{ 
    public class AesCounterMode : SymmetricAlgorithm
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Length of an array, in bytes, that contains the Counter.
        /// </summary>
        private const int _counterLength = 0x10;
        private readonly byte[] _counter;
        private readonly AesCryptoServiceProvider _aesCrypto;

        /// <summary>
        /// Constructor of Aes Counter Mode object.
        /// </summary>
        /// <param name="counterInitialState"> Initial state of counter, 16 bytes length.</param>
        public AesCounterMode(byte[] counterInitialState)
        {
            if (counterInitialState == null)
                throw new ArgumentNullException(nameof(counterInitialState));
            if (counterInitialState.Length != _counterLength)
                throw new ArgumentException($"Counter size must be the same as block size (actual: {counterInitialState.Length}, expected: {_counterLength}");

            _aesCrypto = new AesCryptoServiceProvider { Mode = CipherMode.ECB, Padding = PaddingMode.None };
            _counter = counterInitialState;          
        }

        /// <summary>
        /// Obsolete in counter mode.
        /// </summary>
        public override void GenerateIV() { }
        /// <summary>
        /// Generates a random key to use for the algorithm.
        /// </summary>
        public override void GenerateKey() { _aesCrypto.GenerateKey(); }

        /// <summary>
        /// Creates decryptor object.
        /// </summary>
        /// <param name="rgbKey"> Key used in decryption.</param>
        /// <param name="rgbIv"> Initialization vector parametr will be omitted in counter mode.</param>
        /// <returns></returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIv)
        {
            return CreateDecryptor(rgbKey);
        }
        /// <summary>
        /// Creates decryptor object.
        /// </summary>
        /// <param name="rgbKey"> Key used in decryption.</param>
        /// <returns></returns>
        public ICryptoTransform CreateDecryptor(byte[] rgbKey)
        {
            return new CounterModeCryptoTransform(_aesCrypto, rgbKey, _counter);
        }

        /// <summary>
        /// Creates encryptor object.
        /// </summary>
        /// <param name="rgbKey"> Key used in encryption.</param>
        /// <param name="rgbIv"> Initialization vector parametr will be omitted in counter mode.</param>
        /// <returns></returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIv)
        {
            return CreateEncryptor(rgbKey);
        }
        /// <summary>
        /// Creates encryptor object.
        /// </summary>
        /// <param name="rgbKey"> Key used in encryption.</param>
        /// <returns></returns>
        public ICryptoTransform CreateEncryptor(byte[] rgbKey)
        {
            return new CounterModeCryptoTransform(_aesCrypto, rgbKey, _counter);
        }

    }

}
