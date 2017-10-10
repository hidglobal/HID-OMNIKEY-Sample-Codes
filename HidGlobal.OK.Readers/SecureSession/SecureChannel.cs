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
using System;
using System.Linq;
using System.Security.Cryptography;
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.SecureSession
{
    public class SecureChannel : ISecureChannel
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        enum SessionStatus { NotEstablished, GetChallengePhase, MutualAuthenticationPhase, Established };

        /// <summary>
        /// Length of encryption key in bytes.
        /// </summary>
        const int _keyLength = 0x10;
        /// <summary>
        /// Length of mac key in bytes.
        /// </summary>
        const int _macLength = 0x10;
        /// <summary>
        /// Length of nonce in bytes.
        /// </summary>
        const int _nonceLength = 0x10;
        /// <summary>
        /// Current status of secure channel.
        /// </summary>
        SessionStatus _sessionStatus;
        public bool IsSessionActive { get { return _sessionStatus == SessionStatus.Established;  } }
        /// <summary>
        /// Reader 16-bytes response to Get Challange request.
        /// </summary>
        string _readerNonce;
        /// <summary>
        /// 16-bytes key randomized by the reader.
        /// </summary>
        string _readerKey;
        /// <summary>
        /// 16-bytes key randomized by the client.
        /// </summary>
        string _hostKey;
        /// <summary>
        /// 16-bytes random number from the client.
        /// </summary>
        string _hostNonce;
        /// <summary>
        /// SSC is a 16 bytes counter which must be incremented after every packet (request and response).
        /// </summary>
        Counter _counter;
        /// <summary>
        /// Active Session Key.
        /// </summary>
        string _sessionEncryptionKey;
        /// <summary>
        /// Active Session Mac.
        /// </summary>
        string _sessionMacKey;
        /// <summary>
        /// Key slot used to establish secure session.
        /// </summary>
        byte _keySlot;
        /// <summary>
        /// Reader object
        /// </summary>
        IReader _reader;

        public SecureChannel(IReader reader)
        {
            _sessionStatus          = SessionStatus.NotEstablished;
            _reader                 = reader;
        }
        /// <summary>
        /// Retuns true if bitwise AND of two arrays is not equal to 0.
        /// </summary>
        /// <param name="arrayA"> Array of bytes.</param>
        /// <param name="arrayB"> Array of bytes.</param>
        /// <returns></returns>
        bool BitWiseAndForArrayIsNotZero(byte[] arrayA, byte[] arrayB)
        {
            var arrayLength = arrayA.Length >= arrayB.Length ? arrayA.Length : arrayB.Length;
            // Pads the arrays with 0x00 on the left side to appropriate length
            if (arrayA.Length != arrayLength)
                arrayA = BinaryHelper.ConvertOctetStringToBytes(BitConverter.ToString(arrayA)
                    .Replace("-", "")
                    .PadLeft((arrayLength - arrayA.Length) * 2, '0'));

            if (arrayB.Length != arrayLength)
                arrayB = BinaryHelper.ConvertOctetStringToBytes(BitConverter.ToString(arrayB)
                    .Replace("-", "")
                    .PadLeft((arrayLength - arrayB.Length) * 2, '0'));

            for (var i = 0; i < arrayLength; i++)
            {
                if (0x00 != (byte)(arrayA[i] & arrayB[i]))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Calculate bitwise AND for two arrays.
        /// </summary>
        /// <param name="arrayA"> Array of bytes.</param>
        /// <param name="arrayB"> Array of bytes.</param>
        /// <returns> Array of bytes with result of AND operation on appropriate bytes of both arrays.</returns>
        byte[] BitWiseAndForArray(byte[] arrayA, byte[] arrayB)
        {
            var arrayLength = arrayA.Length >= arrayB.Length ? arrayA.Length : arrayB.Length;
            var result = new byte[arrayLength];
            // Pads the arrays with 0x00 on the left side to appropriate length
            if (arrayA.Length != arrayLength)
                arrayA = BinaryHelper.ConvertOctetStringToBytes(BitConverter.ToString(arrayA)
                    .Replace("-", "")
                    .PadLeft((arrayLength - arrayA.Length) * 2, '0'));

            if (arrayB.Length != arrayLength)
                arrayB = BinaryHelper.ConvertOctetStringToBytes(BitConverter.ToString(arrayB)
                    .Replace("-", "")
                    .PadLeft((arrayLength - arrayB.Length) * 2, '0'));

            for (var i = 0; i < arrayLength; i++)
                result[i] = (byte)(arrayA[i] & arrayB[i]);

            return result;
        }
        /// <summary>
        /// Rolls the array by one bit.
        /// </summary>
        /// <param name="parameter"> Byte array to be rolled. </param>
        /// <returns></returns>
        byte[] Rol(byte[] parameter)
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
        byte[] Double(byte[] parameter)
        {
            byte[] result = null;
            byte[] temp = null;
            var mask1 = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            var mask2 = new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            temp = Rol(parameter);
            result = BitWiseAndForArray(temp, mask1);

            if (BitWiseAndForArrayIsNotZero(parameter, mask2))
                result[result.Length - 1] = (byte)(result[result.Length - 1] ^ 0x87);
            return result;
        }
        byte[] XorEnd(byte[] m, byte[] t)
        {
            try
            {
                if (m.Length >= 16)
                {
                    var end = new byte[16];
                    var result = new byte[m.Length];
                    // coppy beginning
                    Array.ConstrainedCopy(m, 0, result, 0, m.Length - end.Length);
                    // copy ending
                    Array.ConstrainedCopy(m, m.Length - end.Length, end, 0, end.Length);
                    // xor ending
                    end = XorArray(end, t);
                    // copy xored end to the result
                    Array.ConstrainedCopy(end, 0, result, result.Length - end.Length, end.Length);
                    return result;
                }
                var temp = new byte[16];
                Array.ConstrainedCopy(m, 0, temp, 0, m.Length);
                temp[m.Length] = 0x80;
                for (var i = m.Length + 1; i < 16; i++)
                    temp[i] = 0x00;

                return XorArray(Double(t), temp);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                throw new Exception("XorEnd exception", error);
            }
        }
        byte[] XorArray(byte[] first, byte[] second)
        {
            if (first.Length == second.Length)
            {
                var result = new byte[first.Length];
                for (var i = 0; i < first.Length; i++)
                    result[i] = (byte)(first[i] ^ second[i]);
                return result;
            }
            else
                throw new Exception("XorArray: Array sizes are not equal.");
        }
        byte[] AesSivCtr(byte[] key, byte[] mac, byte[] data)
        {
            var mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF };
            var iv = BitWiseAndForArray(mac, mask);
            byte[] result;

            using (var am = new AesCounterMode(iv))
            using (var ict = am.CreateEncryptor(key))
                result = ict.TransformFinalBlock(data, 0, data.Length);

            return result;
        }
        byte[] AesSivMac(byte[] macKey, byte[] header, byte[] data)
        {
            var cmzerod = Double(CMac.GetHashTag(macKey, new byte[16]));
            var h1Mac = CMac.GetHashTag(macKey, header);
            var m1 = XorArray(h1Mac, cmzerod);
            var t = XorEnd(data, m1);
            var iv = CMac.GetHashTag(macKey, t);
            return iv;
        }
        byte[] AesSp800108(byte[] data, byte[] macKey)
        {
            byte[] key1 = CMac.GetHashTag(macKey, data.Concat<byte>(new byte[] { 0x01, 0x01, 0x00 }).ToArray());
            byte[] key2 = CMac.GetHashTag(macKey, data.Concat<byte>(new byte[] { 0x02, 0x01, 0x00 }).ToArray());
            return key1.Concat(key2).ToArray();
        }
        bool Connect()
        {
            if (_reader.IsConnected != false)
                _reader.Disconnect(CardDisposition.Unpower);

            _reader.Connect(Components.ReaderSharingMode.Exclusive, Components.Protocol.Any);

            if (_reader.IsConnected != true)
                return false;
            else
                return true;
        }
        
        public void Establish(string key, byte keySlot)
        {
            _keySlot = keySlot;
            if (key.Length != (4 * _macLength))
            {
                log.Error($"{nameof(key)} length invalid, expected 32 bytes key");
                return;
            }

            _sessionEncryptionKey = key.Substring(0, 2 * _keyLength);
            _sessionMacKey = key.Substring(2 * _keyLength);

            if (!Connect())
            {
                log.Error($"Unable to connect to reader: {_reader.PcscReaderName}");
                return;
            }

            GetChallange();
            var response = HostAuthentication();
            ReaderAuthentication(response);
        }
        
        public void Establish(byte[] key, byte keySlot)
        {
            Establish(BinaryHelper.ConvertBytesToOctetString(key), keySlot);
        }

        void GetChallange()
        {
            _sessionStatus = SessionStatus.GetChallengePhase;

            string getChallangeApdu = "FF7200" + _keySlot.ToString("X2") + "00";

            var response = _reader.Transmit(getChallangeApdu);

            if (response.Substring(response.Length - 4) != "9000")
            {
                log.Error($"Establish secure session failed at {_sessionStatus}\nSend: {getChallangeApdu}\nRecived apdu: {response}");
                _sessionStatus = SessionStatus.NotEstablished;
                return;
            }
            _readerNonce = response.Substring(0, _nonceLength * 2);
            
        }

        string HostAuthentication()
        {
            if (_sessionStatus != SessionStatus.GetChallengePhase)
            {
                _sessionStatus = SessionStatus.NotEstablished;
                return null;
            }

            using (var randomGenerator = RNGCryptoServiceProvider.Create())
            {
                var hostKey = new byte[_keyLength];
                var hostNonce = new byte[_nonceLength];

                randomGenerator.GetBytes(hostKey);
                randomGenerator.GetBytes(hostNonce);

                _hostKey = BinaryHelper.ConvertBytesToOctetString(hostKey);
                _hostNonce = BinaryHelper.ConvertBytesToOctetString(hostNonce);
            }

            // encrypy data
            byte[] plain = BinaryHelper.ConvertOctetStringToBytes(_hostNonce + _readerNonce + _hostKey);
            var mac = AesSivMac(BinaryHelper.ConvertOctetStringToBytes(_sessionMacKey), new byte[] { _keySlot }, plain);
            var enc = AesSivCtr(BinaryHelper.ConvertOctetStringToBytes(_sessionEncryptionKey), mac, plain);

            string mutualAuthenticationApdu = "FF72010040" + BinaryHelper.ConvertBytesToOctetString(enc.Concat(mac).ToArray());
            var response = _reader.Transmit(mutualAuthenticationApdu);

            if (response.Substring(response.Length - 4) != "9000")
            {
                log.Error($"Establish secure session failed at HostAuthenticationPhase \nSend: {mutualAuthenticationApdu}\nRecived apdu: {response}");
                _sessionStatus = SessionStatus.NotEstablished;
                return null;
            }
            _sessionStatus = SessionStatus.MutualAuthenticationPhase;
            return response;
        }

        void ReaderAuthentication(string hostAuthResponse)
        {
            if (_sessionStatus != SessionStatus.MutualAuthenticationPhase)
            {
                _sessionStatus = SessionStatus.NotEstablished;
                return;
            }
            
            var data = BinaryHelper.ConvertOctetStringToBytes(hostAuthResponse.Substring(0, hostAuthResponse.Length - 4));
            var mac2 = data.Skip(_nonceLength + _nonceLength + _keyLength).Take(_macLength).ToArray();

            var plain2 = AesSivCtr(BinaryHelper.ConvertOctetStringToBytes(_sessionEncryptionKey), mac2, data.Take(_nonceLength + _nonceLength + _keyLength).ToArray());
            var mac3 = AesSivMac(BinaryHelper.ConvertOctetStringToBytes(_sessionMacKey), new byte[] { _keySlot }, plain2);

            if (!Enumerable.SequenceEqual(mac2, mac3))
            {
                log.Error("Mac mismatch. Session Terminated.");
                _sessionStatus = SessionStatus.NotEstablished;
                return;
            }
            if (!Enumerable.SequenceEqual(plain2.Take(_nonceLength).ToArray(), BinaryHelper.ConvertOctetStringToBytes(_readerNonce)))
            {
                log.Error("Reader nonce mismatch. Session Terminated.");
                _sessionStatus = SessionStatus.NotEstablished;
                return;
            }
            if (!Enumerable.SequenceEqual(plain2.Skip(_nonceLength).Take(_nonceLength).ToArray(), BinaryHelper.ConvertOctetStringToBytes(_hostNonce)))
            {
                log.Error("Host nonce mismatch. Session Terminated.");
                _sessionStatus = SessionStatus.NotEstablished;
                return;
            }

            _readerKey = BinaryHelper.ConvertBytesToOctetString(plain2.Skip(_nonceLength + _nonceLength)
                .Take(_keyLength)
                .ToArray());

            var sessionkeys = AesSp800108(BinaryHelper.ConvertOctetStringToBytes(_hostKey + _readerKey),
                BinaryHelper.ConvertOctetStringToBytes(_sessionMacKey));

            _sessionEncryptionKey = BinaryHelper.ConvertBytesToOctetString(sessionkeys.Take(_keyLength).ToArray());
            _sessionMacKey = BinaryHelper.ConvertBytesToOctetString(sessionkeys.Skip(_keyLength).Take(_keyLength).ToArray());

            _counter = new Counter(_hostNonce, _readerNonce);

            _sessionStatus = SessionStatus.Established;
        }
        /// <summary>
        /// Terminates established secure channel
        /// </summary>
        public void Terminate()
        {
            if (_sessionStatus == SessionStatus.Established)
                _reader.Transmit("FF72030000");

            if (_reader.IsConnected)
                _reader.Disconnect(CardDisposition.Unpower);

            _sessionStatus          = SessionStatus.NotEstablished;
            _sessionEncryptionKey   = null;
            _sessionMacKey          = null;
            _hostKey                = null;
            _hostNonce              = null;
            _readerKey              = null;
            _readerNonce            = null;
            _counter                = null;
        }
        /// <summary>
        /// Encrypts and sends given apdu command, returns decrypted response.
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        public string SendCommand(string apdu)
        {
            if (_sessionStatus != SessionStatus.Established)
            {
                log.Error("Attempt to Send Command via secure session, while session is not established");
                Terminate();
                return null;
            }
            // Encrypt data
            _counter.Increment();

            byte[] mac = AesSivMac(BinaryHelper.ConvertOctetStringToBytes(_sessionMacKey), BinaryHelper.ConvertOctetStringToBytes(_counter.Value), BinaryHelper.ConvertOctetStringToBytes(apdu));
            byte[] enc = AesSivCtr(BinaryHelper.ConvertOctetStringToBytes(_sessionEncryptionKey), mac, BinaryHelper.ConvertOctetStringToBytes(apdu));
            byte[] data = enc.Concat(mac).ToArray();

            var response = _reader.Transmit("FF720200" + data.Length.ToString("X2") + BinaryHelper.ConvertBytesToOctetString(data));
            
            // Decrypt response
            _counter.Increment();

            if (response.Substring(response.Length - 4) != "9000")
            {
                log.Error($"Error {response.Substring(response.Length - 4)}\nSession Terminated.");
                _sessionStatus = SessionStatus.NotEstablished;
                Terminate();
                return response;
            }

            byte[] cryptogram = BinaryHelper.ConvertOctetStringToBytes(response.Substring(0, response.Length - 4));

            byte[] dataEnc = cryptogram.Take(cryptogram.Length - _macLength).ToArray();
            byte[] dataMac = cryptogram.Skip(cryptogram.Length - _macLength).Take(_macLength).ToArray();

            byte[] plain = AesSivCtr(BinaryHelper.ConvertOctetStringToBytes(_sessionEncryptionKey), dataMac, dataEnc);
            byte[] dataMac2 = AesSivMac(BinaryHelper.ConvertOctetStringToBytes(_sessionMacKey), BinaryHelper.ConvertOctetStringToBytes(_counter.Value), plain);
            if (!Enumerable.SequenceEqual(dataMac, dataMac2))
            {
                log.Error("Mac mismatch in decrypted response.\nSession Terminated.");
                _sessionStatus = SessionStatus.NotEstablished;
                Terminate();
                return null;
            }
            return BinaryHelper.ConvertBytesToOctetString(plain);
        }
        /// <summary>
        /// Encrypts and sends given apdu command, returns decrypted response.
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        public byte[] SendCommand(byte[] apdu)
        {
            return BinaryHelper.ConvertOctetStringToBytes(SendCommand(BinaryHelper.ConvertBytesToOctetString(apdu)));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (IsSessionActive)
                        Terminate();

                    if (_reader.IsConnected)
                        _reader.Disconnect(CardDisposition.Reset);

                    _reader                 = null;
                }


                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
