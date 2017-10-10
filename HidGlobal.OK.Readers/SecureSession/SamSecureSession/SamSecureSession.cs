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
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.Utilities;
using System.Security.Cryptography;

namespace HidGlobal.OK.Readers.SecureSession.SamSecureSession
{
    public enum SamSessionKeyNumber
    {
        EndUser      = 0x80,
        EndUserAdmin = 0x81,
        OemUser      = 0x82,
        OemUserAdmin = 0x83,
        HidUser      = 0x84,
        HidUserAdmin = 0x85
    };

    public class SamSecureSession : ISecureChannel
    {
        /// <summary>
        /// True if Secure Session is Established.
        /// </summary>
        public bool IsSessionActive { get; private set; }
        private string _sessionEncKey;
        private string _sessionMacKey1;
        private string _sessionMacKey2;
        private string _rMac;
        private string _cMac;
        private IReader _reader;
        public SamSecureSession(IReader reader)
        {
            IsSessionActive = false;
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _reader = reader;
        }

        public void Establish(byte[] masterKey, byte keyNumber)
        {
            string key = BinaryHelper.ConvertBytesToOctetString(masterKey);
            Establish(key, keyNumber);
        }
        public void Establish(string masterKey, byte keyNumber)
        {
            Connect();
            string initData = InitializeAuthentication(keyNumber);
            string serverUid = initData.Substring(0, 16);
            string serverNonce = initData.Substring(16, 16);
            string serverCryptogram = initData.Substring(32, 32);
            string clientNonce = initData.Substring(64, 16);
            ContinueAuthentiacation(masterKey, clientNonce, serverUid, serverNonce, serverCryptogram);
        }
        public void Terminate()
        {
            if (!IsSessionActive)
                return;
                
            string wrapedTerminateCommand = Wrap("FF70076B04A102A200");
            string command = $"FF70076B{wrapedTerminateCommand.Length / 2:X2}" + wrapedTerminateCommand + "00";
            string response = _reader.Transmit(command);
            _reader.Disconnect();

            ClearFields();
            if (response == "9D0290009000")
                return;

            throw new Exception($"Server response to terminate session request mismatch, \nExpected: 9D0290009000, Actual: {response}");
        }
        public string SendCommand(string command)
        {
            string wrapedData = Wrap(command);
            command = $"FF70076B{wrapedData.Length / 2:X2}" + wrapedData + "00";
            string response =_reader.Transmit(command);
            if (!response.StartsWith("9D"))
            {
                ClearFields();
                throw new Exception($"Invalid Response Tag value. Expected: 0x9D, Actual: 0x{response.Substring(0, 2)}");
            }
            string dataToUnWrap = response.Remove(0, 4);
            if (!response.EndsWith("9000"))
            {
                ClearFields();
                throw new Exception($"Invalid Response SW value. Expected: 0x9000, Actual: 0x{response.Substring(response.Length - 4, 4)}");
            }
            dataToUnWrap = dataToUnWrap.Remove(dataToUnWrap.Length - 4, 4);
            return UnWrap(dataToUnWrap);
        }
        public byte[] SendCommand(byte[] command)
        {
            string response = SendCommand(BinaryHelper.ConvertBytesToOctetString(command));
            return BinaryHelper.ConvertOctetStringToBytes(response);
        }
        public void Dispose()
        {
            Terminate();
            _reader = null;
        }
        private string Wrap(string data)
        {
            data = Pad(data);
            string iv = GetComplement(_rMac);
            string encryptedData = Encrypt(_sessionEncKey, data, CipherMode.CBC, PaddingMode.None, iv);
            _cMac = ComputeMac(encryptedData, _rMac);
            return encryptedData + _cMac;
        }
        private string UnWrap(string data)
        {
            if (data.Length % 32 != 0)
            {
                ClearFields();
                throw new ArgumentException("Invalid data length", nameof(data));
            }

            string encryptedData = data.Substring(0, data.Length - 32);
            string dataRMac = data.Substring(data.Length - 32);

            _rMac = ComputeMac(encryptedData, _cMac);

            if (_rMac != dataRMac)
            {
                ClearFields();
                throw new Exception("Invalid MAC recived.");
            }
            string iv = GetComplement(_cMac);
            string decryptedData = Decrypt(_sessionEncKey, encryptedData, CipherMode.CBC, PaddingMode.None, iv);
            return UnPad(decryptedData);
        }
        private string Pad(string data)
        {
            string padding = string.Empty;
            data += "80";
            int length = 32 - data.Length % 32;
            if (length != 0 && length != 32)
                padding = BinaryHelper.ConvertBytesToOctetString(new byte[length / 2]);
            return data + padding;
        }
        private string UnPad(string data)
        {
            int index = data.Length - 2;
            while (data.Substring(index, 2) == "00")
            {
                index -= 2;
            }
            if (data.Substring(index, 2) == "80")
                return data.Remove(index);

            ClearFields();
            throw new Exception("Invalid padding!");
        }
        private string InitializeAuthentication(byte keyNumber, byte versionSecCh = 0x00)
        {
            string clientNonce;
            using (var randomGenerator = new RNGCryptoServiceProvider())
            {
                var tempNonce = new byte[8];
                randomGenerator.GetBytes(tempNonce);
                clientNonce = BinaryHelper.ConvertBytesToOctetString(tempNonce);
            }

            string initAuthCommand = "FF70076B14" + $"A112A0108001{versionSecCh:X2}" + $"8101{keyNumber:X2}" + $"8208{clientNonce}00";
            string response = _reader.Transmit(initAuthCommand);

            if (!(response.StartsWith("9D20") && response.EndsWith("9000")))
            {
                ClearFields();
                throw new Exception($"Server answer to Initialize Authentication command incorrect, answer: {response}");
            }

            return response.Substring(4, 64) + clientNonce;
        }
        private void ContinueAuthentiacation(string masterKey, string clientNonce, string serverUid, string serverNonce, string serverCryptogram)
        {
            string secureChannelBaseKey = GetSecureChannelBaseKey(masterKey, serverUid);

            GetSessionKeys(secureChannelBaseKey, serverNonce);
            
            // Encrypt and check Server Cryptogram
            string expectedServerCryptogram = Encrypt(_sessionEncKey, clientNonce + serverNonce);

            if (!serverCryptogram.Equals(expectedServerCryptogram))
            {
                ClearFields();
                throw new Exception($"Server Cryptogram mismatch, \nExpected: {expectedServerCryptogram}, Actual: {serverCryptogram}");
            }
                
            // Encrypt client Cryptogram
            string clientCryptogram = Encrypt(_sessionEncKey, serverNonce + clientNonce);
            string clientCryptogramCmac = ComputeMac(clientCryptogram);

            string continueAuthCommand = "FF70076B28" + $"A126A1248010{clientCryptogram}" + $"8110{clientCryptogramCmac}00";
            string response = _reader.Transmit(continueAuthCommand);

            if (!(response.StartsWith("9D10") && response.EndsWith("9000")))
            {
                ClearFields();
                throw new Exception($"Server answer to Continue Authentication command incorrect, answer: {response}");
            }

            // check response mac
            string serverRmac = response.Substring(4, 32);
            string rMacData = "80" + BinaryHelper.ConvertBytesToOctetString(new byte[15]);
            string expectedServerRmac = ComputeMac(rMacData, clientCryptogramCmac);
            if (!serverRmac.Equals(expectedServerRmac))
            {
                ClearFields();
                throw new Exception($"Server RMAC mismatch, \nExpected: {expectedServerRmac}, Actual: {serverRmac}");
            }

            _rMac = serverRmac;
            IsSessionActive = true;
        }
        private string ComputeMac(string data, string iv = "00000000000000000000000000000000")
        {
            // TODO Maybe Padding
            string iv2 = string.Empty;
            if (data.Length > 32)
            {
                string resEncryption = Encrypt(_sessionMacKey1, data.Substring(0, data.Length - 32), CipherMode.CBC, PaddingMode.None, iv);
                if (resEncryption.Length > 0)
                {
                    iv2 = resEncryption.Substring(resEncryption.Length - 32, 32);
                }
            }
            else
            {
                iv2 = iv;
            }
            return Encrypt(_sessionMacKey2, data.Substring(data.Length - 32, 32), CipherMode.CBC, PaddingMode.None, iv2);
        }
        private void GetSessionKeys(string secureChannelBaseKey, string serverNonce)
        {
            string keyInitData = serverNonce.Substring(0, 4) + "000000000000000000000000";
            _sessionMacKey1 = Encrypt(secureChannelBaseKey, "0101" + keyInitData);
            _sessionMacKey2 = Encrypt(secureChannelBaseKey, "0102" + keyInitData);
            _sessionEncKey = Encrypt(secureChannelBaseKey, "0182" + keyInitData);
        }
        private string GetSecureChannelBaseKey(string masterKey, string serverUid)
        {
            string complementServerUid = GetComplement(serverUid);
            return Encrypt(masterKey, serverUid + complementServerUid);
        }
        private string Encrypt(string key, string data, CipherMode cMode = CipherMode.ECB, PaddingMode pMode = PaddingMode.None, string iv = "00000000000000000000000000000000")
        {
            byte[] derivedData;
            using (var aes = new AesCryptoServiceProvider { Mode = cMode, Padding = pMode })
            using (var ict = aes.CreateEncryptor(BinaryHelper.ConvertOctetStringToBytes(key), BinaryHelper.ConvertOctetStringToBytes(iv)))
            {
                byte[] dataBytes = BinaryHelper.ConvertOctetStringToBytes(data);
                derivedData = ict.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            }
            return BinaryHelper.ConvertBytesToOctetString(derivedData);
        }
        private string Decrypt(string key, string data, CipherMode cMode = CipherMode.ECB, PaddingMode pMode = PaddingMode.None, string iv = "00000000000000000000000000000000")
        {
            byte[] derivedData;
            using (var aes = new AesCryptoServiceProvider { Mode = cMode, Padding = pMode })
            using (var ict = aes.CreateDecryptor(BinaryHelper.ConvertOctetStringToBytes(key), BinaryHelper.ConvertOctetStringToBytes(iv)))
            {
                byte[] dataBytes = BinaryHelper.ConvertOctetStringToBytes(data);
                derivedData = ict.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            }
            return BinaryHelper.ConvertBytesToOctetString(derivedData);
        }
        private string GetComplement(string data)
        {
            byte[] result = GetComplement(BinaryHelper.ConvertOctetStringToBytes(data));
            return BinaryHelper.ConvertBytesToOctetString(result);
        }
        private byte[] GetComplement(byte[] data)
        {
            var listBytes = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                listBytes[i] = (byte)~data[i];
            }
            return listBytes;
        }
        private void ClearFields()
        {
            _sessionEncKey = string.Empty;
            _sessionMacKey1 = string.Empty;
            _sessionMacKey2 = string.Empty;
            _rMac = string.Empty;
            _cMac = string.Empty;
            IsSessionActive = false;
        }
        private void Connect()
        {
            if (_reader.IsConnected)
                _reader.Disconnect(CardDisposition.Reset);

            _reader.Connect(ReaderSharingMode.Exclusive, Protocol.Any);

            if (_reader.CurrentErrorStatus == ErrorCodes.SCARD_S_SUCCESS)
                return;

            ClearFields();
            throw new Exception($"Failed to connect with: {_reader.PcscReaderName}, mode: {_reader.ConnectionMode}, error: {_reader.CurrentErrorStatus}");
        }
    }
}
