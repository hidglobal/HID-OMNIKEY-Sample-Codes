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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.SecureSession;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    static class ContactlessCardCommunicationSample
    {
        private static void PrintData(string title, string command, string response, string information)
        {
            Console.WriteLine(title);
            Console.WriteLine($"<-- {command}\n--> {response}\n{information}");
            Console.WriteLine("-----------------------------------");
        }
        private static IReader ConnectWithCard(string readerName)
        {
            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (readerState.AtrLength > 0)
            {
                reader.Connect(ReaderSharingMode.Shared, Protocol.Any);
                return reader;
            }
            throw new Exception("No Smart Card Available.");
        }
        private static IReader Connect(string readerName)
        {
            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (readerState.AtrLength > 0)
                reader.Connect(ReaderSharingMode.Shared, Protocol.Any);
            else
                reader.ConnectDirect();

            return reader;
        }
        public static void SeosExample(string readerName)
        {
            try
            {
                var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
                IReader reader = ConnectWithCard(readerName);
                if (!reader.IsConnected)
                    return;

                // Get Seos card UID, Le = 00 -> get all available data
                string command = contactlessCommands.GetData.GetApdu(GetDataCommand.Type.Default);
                string response = reader.Transmit(command);
                PrintData("Get Seos UID", command, response, $"UID: {response.Substring(0, response.Length - 4)}");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void ReadMifareClassic1k4kExample(string readerName)
        {
            try
            {
                var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
                IReader reader = ConnectWithCard(readerName);

                if (!reader.IsConnected)
                    return;

                string command;
                string response;

                // Get Card UID
                command = contactlessCommands.GetData.GetApdu(GetDataCommand.Type.Default);
                response = reader.Transmit(command);
                PrintData("Get Card UID", command, response, $"UID: {response.Substring(0, response.Length - 4)}");

                // Read Block 4 with authentication using correct key (correct key need to be loaded to used keyslot)
                byte blockNumber = 0x04;
                byte blockSize = 0x10;
                byte keySlotNumber = 0x00;
                var keyType = GeneralAuthenticateCommand.MifareKeyType.MifareKeyA;

                command = contactlessCommands.GeneralAuthenticate.GetMifareApdu(blockNumber, keyType, keySlotNumber);
                response = reader.Transmit(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate Block 0x{blockNumber:X2} ", command, response,
                    $"Key type: {keyType}, Key from slot {keySlotNumber:D}, {result}");

                command = contactlessCommands.ReadBinary.GetMifareReadApdu(blockNumber, blockSize);
                response = reader.Transmit(command);
                PrintData($"Read Block 0x{blockNumber:X2}", command, response,
                    $"Data: {response.Substring(0, response.Length - 4)}");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void WriteMifareClassic1k4kExample(string readerName)
        {
            try
            {
                var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
                IReader reader = ConnectWithCard(readerName);

                if (!reader.IsConnected)
                    return;

                string command;
                string response;

                // Write data to Block 4 with authentication using correct key (correct key need to be loaded to used keyslot)
                byte keySlotNumber = 0x00;
                var keyType = GeneralAuthenticateCommand.MifareKeyType.MifareKeyA;
                byte blockNumber = 0x04;
                string data = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

                command = contactlessCommands.GeneralAuthenticate.GetMifareApdu(blockNumber, keyType, keySlotNumber);
                response = reader.Transmit(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate Block 0x{blockNumber:X2} ", command, response,
                    $"Key type: {keyType}, Key from slot {keySlotNumber:D}, {result}");

                command = contactlessCommands.UpdateBinary.GetApdu(UpdateBinaryCommand.Type.Default, blockNumber, data);
                response = reader.Transmit(command);
                PrintData($"Write Block 0x{blockNumber:X2} with data: {data}", command, response, "");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void IncrementMifareClassic1k4kExample(string readerName)
        {
            try
            {
                var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
                IReader reader = ConnectWithCard(readerName);

                if (!reader.IsConnected)
                    return;

                string command;
                string response;

                // Change data in Block 4 with authentication using correct key (key need to be loaded to used keyslot)
                byte keySlotNumber = 0x00;
                var keyType = GeneralAuthenticateCommand.MifareKeyType.MifareKeyA;
                byte blockNumber = 0x04;
                byte blockSize = 0x10;

                // Authenticate block 4 with key loaded to reader
                command = contactlessCommands.GeneralAuthenticate.GetMifareApdu(blockNumber, keyType, keySlotNumber);
                response = reader.Transmit(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate Block 0x{blockNumber:X2} ", command, response,
                    $"Key type: {keyType}, Key from slot {keySlotNumber:D}, {result}");

                // Update block 4 with write operation in value block format:
                // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                int value = 1234567;

                string bigEndianValue = BitConverter.IsLittleEndian
                    ? BitConverter.ToString(BitConverter.GetBytes(value)).Replace("-", "")
                    : value.ToString("X8");
                string bigEndianInvertedValue = BitConverter.IsLittleEndian
                    ? BitConverter.ToString(BitConverter.GetBytes(~value)).Replace("-", "")
                    : (~value).ToString("X8");

                string data = bigEndianValue + bigEndianInvertedValue + bigEndianValue + $"{blockNumber:X2}" +
                              ((byte) ~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                              ((byte) ~blockNumber).ToString("X2");

                command = contactlessCommands.UpdateBinary.GetApdu(UpdateBinaryCommand.Type.Default, blockNumber, data);
                response = reader.Transmit(command);
                PrintData($"Write Block 0x{blockNumber:X2} with data: {data}", command, response, "");

                // Read current value
                command = contactlessCommands.ReadBinary.GetMifareReadApdu(blockNumber, blockSize);
                response = reader.Transmit(command);
                PrintData($"Read Block 0x{blockNumber:X2}", command, response,
                    $"Data: {response.Substring(0, response.Length - 4)}");

                // Increment value in block 4 by 1
                int incrementValue = 1;
                string incrementData = BitConverter.IsLittleEndian
                    ? BitConverter.ToString(BitConverter.GetBytes(incrementValue)).Replace("-", "")
                    : incrementValue.ToString("X8");
                command = contactlessCommands.Increment.GetApdu(blockNumber, incrementData);
                response = reader.Transmit(command);
                PrintData($"Increment by 1 the value in block 0x{blockNumber:X2}", command, response, "");

                // Read value after incrementation
                command = contactlessCommands.ReadBinary.GetMifareReadApdu(blockNumber, blockSize);
                response = reader.Transmit(command);
                PrintData($"Read Block 0x{blockNumber:X2}", command, response,
                    $"Data: {response.Substring(0, response.Length - 4)}");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void DecrementMifareClassic1k4kExample(string readerName)
        {
            try
            {
                var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
                IReader reader = ConnectWithCard(readerName);

                if (!reader.IsConnected)
                    return;

                string command;
                string response;

                byte keySlotNumber = 0x00;
                var keyType = GeneralAuthenticateCommand.MifareKeyType.MifareKeyA;
                byte blockNumber = 0x04;
                byte blockSize = 0x10;

                // Authenticate block 4 with key loaded to reader
                command = contactlessCommands.GeneralAuthenticate.GetMifareApdu(blockNumber, keyType, keySlotNumber);
                response = reader.Transmit(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate Block 0x{blockNumber:X2} ", command, response,
                    $"Key type: {keyType}, Key from slot {keySlotNumber:D}, {result}");

                // Update block 4 with write operation in value block format:
                // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                int value = 1234567;

                string bigEndianValue = BitConverter.IsLittleEndian
                    ? BitConverter.ToString(BitConverter.GetBytes(value)).Replace("-", "")
                    : value.ToString("X8");
                string bigEndianInvertedValue = BitConverter.IsLittleEndian
                    ? BitConverter.ToString(BitConverter.GetBytes(~value)).Replace("-", "")
                    : (~value).ToString("X8");

                string data = bigEndianValue + bigEndianInvertedValue + bigEndianValue + $"{blockNumber:X2}" +
                              ((byte) ~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                              ((byte) ~blockNumber).ToString("X2");

                command = contactlessCommands.UpdateBinary.GetApdu(UpdateBinaryCommand.Type.Default, blockNumber, data);
                response = reader.Transmit(command);
                PrintData($"Write Block 0x{blockNumber:X2} with data: {data}", command, response, "");

                // Read current value
                command = contactlessCommands.ReadBinary.GetMifareReadApdu(blockNumber, blockSize);
                response = reader.Transmit(command);
                PrintData($"Read Block 0x{blockNumber:X2}", command, response,
                    $"Data: {response.Substring(0, response.Length - 4)}");

                // Decrement value in block 4 by 16
                int decrementValue = 16;
                string decrementData = BitConverter.IsLittleEndian
                    ? BitConverter.ToString(BitConverter.GetBytes(decrementValue)).Replace("-", "")
                    : decrementValue.ToString("X8");
                command = contactlessCommands.Decrement.GetApdu(blockNumber, decrementData);
                response = reader.Transmit(command);
                PrintData($"Decrement by 16 the value in block 0x{blockNumber:X2}", command, response, "");

                // Read value after decrementation
                command = contactlessCommands.ReadBinary.GetMifareReadApdu(blockNumber, blockSize);
                response = reader.Transmit(command);
                PrintData($"Read Block 0x{blockNumber:X2}", command, response,
                    $"Data: {response.Substring(0, response.Length - 4)}");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void LoadMifareKeyExample(string readerName)
        {
            try
            {
                var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;
                // Load 6 byte Mifare key "FFFFFFFFFFFF" to keyslot 5
                byte keySlotNumber = 0;
                string key = "FFFFFFFFFFFF";
                string command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                string response = reader.ConnectionMode != ReaderSharingMode.Direct
                    ? reader.Transmit(command)
                    : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData("Load Mifare Key", command, response, $"Slot {keySlotNumber}, Key {key} : {result}");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void LoadMifareKeyWithSecureSessionExample(string readerName)
        {
            var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
            // Seure session needs to be established with Admin keys to use load key command
            const SessionAccessKeyType loadKeyAccess = SessionAccessKeyType.UserAdminCipherKey;
            // Admin access encryption key, following key needs to be replaced with real one
            const string encKey = "FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF";
            // Admin access mac key
            const string macKey = "FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF";

            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);
            // check card presence
            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            bool isCardPresent = readerState.AtrLength > 0;

            using (var secureChannel = new Readers.SecureSession.SecureChannel(reader))
            {
                // Establish secure session
                bool isEstablished =
                    secureChannel.EstablishSecureSession(encKey, macKey, loadKeyAccess, !isCardPresent);
                if (!isEstablished)
                {
                    Console.WriteLine("Failed to establish secure session, check if keys used to establish session are correct.");
                    return;
                }
                    

                // Load 6 byte Mifare key "FFFFFFFFFFFF" to keyslot 5
                byte keySlotNumber = 5;
                string key = "FFFFFFFFFFFF";
                string command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                
                // encrypt, send and decrypt the response
                string response = secureChannel.SendCommand(command);

                string result = response == "9000" ? "Success" : "Error";
                PrintData("Load Mifare Key within secure session", command, response,
                    $"Slot {keySlotNumber}, Key {key} : {result}");
                // terminate session   
                secureChannel.TerminateSecureSession();
            }
        }
        public static void MifareDesfireEV1Example(string readerName)
        {
            var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
            IReader reader = ConnectWithCard(readerName);
            if (!reader.IsConnected)
                return;

            // Get card UID, Le = 00 -> get all available data
            string command = contactlessCommands.GetData.GetApdu(GetDataCommand.Type.Default);
            string response = reader.Transmit(command);
            PrintData("Get Card UID", command, response, $"UID: {response.Substring(0, response.Length - 4)}");

            // Get Historical bytes 
            command = contactlessCommands.GetData.GetApdu(GetDataCommand.Type.Specific);
            response = reader.Transmit(command);
            PrintData("Get Card Historical Bytes", command, response,
                $"Data: {response.Substring(0, response.Length - 4)}");

            // Create application with AID = 02 00 00
            string aid = "020000";
            command = "90CA000005" + aid + "0F01" + "00";
            response = reader.Transmit(command);
            PrintData($"Create application with AID = {aid}", command, response, string.Empty);

            // Select created application
            command = "905A000003" + aid + "00";
            response = reader.Transmit(command);
            PrintData($"Select application with AID = {aid}", command, response, string.Empty);

            // Create std value file numberd 1 with size of 16 bytes
            byte fileNumber = 0x01;
            byte fileLength = 0x10;
            command = "90CD000007" + $"{fileNumber:X2}00EEEE{fileLength:X2}0000" + "00";
            response = reader.Transmit(command);
            PrintData($"Create standard value file {fileLength} bytes length", command, response, string.Empty);

            // Write to the created file
            byte nBytes = 0x01;
            string data = "FF";
            command = "903D000008" + $"{fileNumber:X2}000000{nBytes:X2}0000" + data + "00";
            response = reader.Transmit(command);
            PrintData($"Write {nBytes} byte of data to created file", command, response, string.Empty);

            // Read n bytes of file starting at byte 0
            nBytes = 0x10;
            string dataField = $"{fileNumber:X2}000000{nBytes:X2}0000";
            command = "90BD000007" + dataField + "00";
            response = reader.Transmit(command);
            PrintData($"Read {nBytes} bytes of data from file number {fileNumber}", command, response, string.Empty);

            // Write another 8 bytes to file 1
            data = "FEFEFEFEFEFEFEFE";
            nBytes = 0x08;
            dataField = $"{fileNumber:X2}010000{nBytes:X2}0000" + data;
            command = $"903D00000F{dataField}00";
            response = reader.Transmit(command);
            PrintData($"Write {nBytes} bytes of data to file", command, response, string.Empty);

            // Read n bytes of file x starting at byte 0
            nBytes = 0x0F;
            dataField = $"{fileNumber:X2}000000{nBytes:X2}0000";
            command = "90BD000007" + dataField + "00";
            response = reader.Transmit(command);
            PrintData($"Read {nBytes} bytes of data from file number {fileNumber}", command, response, string.Empty);

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void Iso15693CardExample(string readerName)
        {
            try
            {
                var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();
                IReader reader = ConnectWithCard(readerName);
                if (!reader.IsConnected)
                    return;

                // Get card CSN, Le = 00 -> get all available data
                string command = contactlessCommands.GetData.GetApdu(GetDataCommand.Type.Default);
                string response = reader.Transmit(command);
                PrintData("Get ISO-15693 card CSN", command, response,
                    $"CSN: {response.Substring(0, response.Length - 4)}");

                // Read one block
                byte addressMsb = 0x00;
                byte addressLsb = 0x00;
                byte expectedLength = 0x00; // 00 -> get all available data
                command = $"FFB0{addressMsb:X2}{addressLsb:X2}{expectedLength:X2}";
                response = reader.Transmit(command);
                PrintData($"Read 1 block of data from address 0x{addressMsb:X2}{addressLsb:X2}", command, response,
                    $"Data: {response.Substring(0, response.Length - 4)}");

                // Update one block
                byte dataLength = 0x04;
                string data = "AAAAAAAA";
                addressMsb = 0x00;
                addressLsb = 0x01;
                command = $"FFD6{addressMsb:X2}{addressLsb:X2}{dataLength:X2}" + data;
                response = reader.Transmit(command);
                PrintData($"Update 1 block of data from address 0x{addressMsb:X2}{addressLsb:X2}", command, response,
                    string.Empty);

                // Read one block
                addressMsb = 0x00;
                addressLsb = 0x01;
                expectedLength = 0x00; // 00 -> get all available data
                command = $"FFB0{addressMsb:X2}{addressLsb:X2}{expectedLength:X2}";
                response = reader.Transmit(command);
                PrintData($"Read 1 block of data from address 0x{addressMsb:X2}{addressLsb:X2}", command, response,
                    $"Data: {response.Substring(0, response.Length - 4)}");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void SecureSessionLoadKeyExample(string readerName)
        {
            var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();

            // Secure session needs to be established with Admin keys to use load key command
            const SessionAccessKeyType loadKeyAccess = SessionAccessKeyType.UserAdminCipherKey;

            // Admin access encryption key, following key needs to be replaced with real one
            const string encKey = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

            // Admin access mac key
            const string macKey = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);
            // check card presence
            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            bool isCardPresent = readerState.AtrLength > 0;

            using (var secureChannel = new Readers.SecureSession.SecureChannel(reader))
            {
                // Establish secure session
                bool isEstablished =
                    secureChannel.EstablishSecureSession(encKey, macKey, loadKeyAccess, !isCardPresent);
                if (!isEstablished)
                {
                    Console.WriteLine("Failed to establish secure session, check if keys used to establish session are correct.");
                    return;
                }

                // Load 6 byte Mifare key "FFFFFFFFFFFF" to keyslot 5
                byte keySlotNumber = 5;
                string key = "FFFFFFFFFFFF";
                string command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                string response = secureChannel.SendCommand(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData("Load Mifare Key within secure session", command, response,
                    $"Slot {keySlotNumber}, Key {key} : {result}");

                // Load 8 byte iclass key "FFFFFFFFFFFFFFFF" to vlolatile keyslot 65
                keySlotNumber = 65;
                key = "FFFFFFFFFFFFFFFF";
                command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                response = secureChannel.SendCommand(command);
                result = response == "9000" ? "Success" : "Error";
                PrintData("Load iClass key to volatile keyslot", command, response,
                    $"Slot {keySlotNumber}, Key {key} : {result}");

                // Load Secure Session Read/Write Cipher key
                keySlotNumber = 70;
                key = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                response = secureChannel.SendCommand(command);
                result = response == "9000" ? "Success" : "Error";
                PrintData("Load Secure Session Read/Write Cipher key", command, response,
                    $"Slot {keySlotNumber}, Key {key} : {result}");

                // Load Secure Session Read/Write MAC key
                keySlotNumber = 71;
                key = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB";
                command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                response = secureChannel.SendCommand(command);
                result = response == "9000" ? "Success" : "Error";
                PrintData("Load Secure Session Read/Write MAC key", command, response,
                    $"Slot {keySlotNumber}, Key {key} : {result}");

                // Load Secure Session Readonly Cipher key
                keySlotNumber = 72;
                key = "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC";
                command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                response = secureChannel.SendCommand(command);
                result = response == "9000" ? "Success" : "Error";
                PrintData("Load Secure Session Readonly Cipher key", command, response,
                    $"Slot {keySlotNumber}, Key {key} : {result}");

                // Load Secure Session Readonly MAC key
                keySlotNumber = 73;
                key = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";
                command = contactlessCommands.LoadKey.GetApdu(keySlotNumber, key);
                response = secureChannel.SendCommand(command);
                result = response == "9000" ? "Success" : "Error";
                PrintData("Load Secure Session Readonly MAC key", command, response,
                    $"Slot {keySlotNumber}, Key {key} : {result}");

                // terminate session   
                secureChannel.TerminateSecureSession();
            }
        }
        public static void ReadBinaryiClass16kExample(string readerName)
        {
            var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();

            const SessionAccessKeyType readOnlyAccess = SessionAccessKeyType.ReadOnlyCipherKey;

            // readonly access encryption key, following key needs to be replaced with real one
            const string encKey = "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC";

            // readonly access mac key, following key needs to be replaced with real one
            const string macKey = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";

            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            // check card presence
            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (!(readerState.AtrLength > 0))
            {
                Console.WriteLine("No Smart Card Available.");
                return;
            }

            using (var secureChannel = new Readers.SecureSession.SecureChannel(reader))
            {
                // Establish secure session
                bool isEstablished = secureChannel.EstablishSecureSession(encKey, macKey, readOnlyAccess);
                if (!isEstablished)
                {
                    Console.WriteLine("Failed to establish secure session, check if keys used to establish session are correct.");
                    return;
                }
                // Authenticate with KC key for page 0 with implicit select book 0 page 0
                // KC key for book 0 page 0 is stored in reader key slot 33 (0x21)
                bool isImplicitSelect = true;
                var book = BookNumber.Book0;
                var page = PageNumber.Page0;
                var keyType = GeneralAuthenticateCommand.IClassKeyType.PicoPassCreditKeyKC;
                byte keySlotNumber = 0x21;
                
                string command = contactlessCommands.GeneralAuthenticate.GetiClassApdu(book, page, isImplicitSelect, keyType, keySlotNumber);
                string response = secureChannel.SendCommand(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate {book}, {page}", command, response, $"Key type: {keyType}, key from slot {keySlotNumber:D}, {result}");
               
                // Read block number 20
                byte blockNumber = 20;

                command = contactlessCommands.ReadBinary.GetiClassReadWithoutSelectApdu(blockNumber);
                response = secureChannel.SendCommand(command);
                PrintData($"Read Block 0x{blockNumber:X2}", command, response, $"Data: {response.Substring(0, response.Length - 4)}");

                // terminate session   
                secureChannel.TerminateSecureSession();
            }
        }
        public static void UpdateBinaryiClass16kExample(string readerName)
        {
            var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();

            const SessionAccessKeyType readWriteAccess = SessionAccessKeyType.ReadWriteCipherKey;

            // read/write access encryption key, following key needs to be replaced with real one
            const string encKey = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

            // read/write access mac key, following key needs to be replaced with real one
            const string macKey = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB";

            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            // check card presence
            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (!(readerState.AtrLength > 0))
            {
                Console.WriteLine("No Smart Card Available.");
                return;
            }

            using (var secureChannel = new Readers.SecureSession.SecureChannel(reader))
            {
                // Establish secure session
                bool isEstablished = secureChannel.EstablishSecureSession(encKey, macKey, readWriteAccess);
                if (!isEstablished)
                {
                    Console.WriteLine("Failed to establish secure session, check if keys used to establish session are correct.");
                    return;
                }
                // Authenticate with KC key for page 0 with implicit select book 0 page 0
                // KC key for book 0 page 0 is stored in reader key slot 33 (0x21)
                bool isImplicitSelect = true;
                var book = BookNumber.Book0;
                var page = PageNumber.Page0;
                var keyType = GeneralAuthenticateCommand.IClassKeyType.PicoPassCreditKeyKC;
                byte keySlotNumber = 0x21;

                string command = contactlessCommands.GeneralAuthenticate.GetiClassApdu(book, page, isImplicitSelect, keyType, keySlotNumber);
                string response = secureChannel.SendCommand(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate {book}, {page}", command, response, $"Key type: {keyType}, key from slot {keySlotNumber:D}, {result}");

                // Update block number 20
                byte blockNumber = 20;
                string data = "BACDEF0122345678";

                command = contactlessCommands.UpdateBinary.GetApdu(UpdateBinaryCommand.Type.Default, blockNumber, data);
                response = secureChannel.SendCommand(command);
                result = response == "9000" ? "Success" : "Error";
                PrintData($"Write 8 byte long data to  {book}, {page}, block: {blockNumber}", command, response, result);

                // terminate session   
                secureChannel.TerminateSecureSession();
            }
        }
        public static void ReadBinaryiClass2ksExample(string readerName)
        {
            var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();

            const SessionAccessKeyType readOnlyAccess = SessionAccessKeyType.ReadOnlyCipherKey;

            // readonly access encryption key, following key needs to be replaced with real one
            const string encKey = "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC";

            // readonly access mac key, following key needs to be replaced with real one
            const string macKey = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";

            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            // check card presence
            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (!(readerState.AtrLength > 0))
            {
                Console.WriteLine("No Smart Card Available.");
                return;
            }

            using (var secureChannel = new Readers.SecureSession.SecureChannel(reader))
            {
                // Establish secure session
                bool isEstablished = secureChannel.EstablishSecureSession(encKey, macKey, readOnlyAccess);
                if (!isEstablished)
                {
                    Console.WriteLine("Failed to establish secure session, check if keys used to establish session are correct.");
                    return;
                }

                // Authenticate with KC key for page 0 WITHOUT implicit select book 0 page 0
                // KC key for book 0 page 0 is stored in reader key slot 33 (0x21)
                bool isImplicitSelect = false;
                var book = BookNumber.Book0;
                var page = PageNumber.Page0;
                var keyType = GeneralAuthenticateCommand.IClassKeyType.PicoPassCreditKeyKC;
                byte keySlotNumber = 0x21;

                string command = contactlessCommands.GeneralAuthenticate.GetiClassApdu(book, page, isImplicitSelect, keyType, keySlotNumber);
                string response = secureChannel.SendCommand(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate {book}, {page}", command, response, $"Key type: {keyType}, key from slot {keySlotNumber:D}, {result}");

                // Read block number 20
                byte blockNumber = 20;

                command = contactlessCommands.ReadBinary.GetiClassReadWithoutSelectApdu(blockNumber);
                response = secureChannel.SendCommand(command);
                PrintData($"Read Block 0x{blockNumber:X2}", command, response, $"Data: {response.Substring(0, response.Length - 4)}");

                // terminate session   
                secureChannel.TerminateSecureSession();
            }
        }
        public static void UpdateBinaryiClass2ksExample(string readerName)
        {
            var contactlessCommands = new Readers.AViatoR.Components.ContactlessCardCommunication();

            const SessionAccessKeyType readWriteAccess = SessionAccessKeyType.ReadWriteCipherKey;

            // read/write access encryption key, following key needs to be replaced with real one
            const string encKey = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

            // read/write access mac key, following key needs to be replaced with real one
            const string macKey = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB";

            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            // check card presence
            ReaderState readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (!(readerState.AtrLength > 0))
            {
                Console.WriteLine("No Smart Card Available.");
                return;
            }

            using (var secureChannel = new Readers.SecureSession.SecureChannel(reader))
            {
                // Establish secure session
                bool isEstablished = secureChannel.EstablishSecureSession(encKey, macKey, readWriteAccess);
                if (!isEstablished)
                {
                    Console.WriteLine("Failed to establish secure session, check if keys used to establish session are correct.");
                    return;
                }
                // Authenticate with KC key for page 0 WITHOUT implicit select book 0 page 0
                // KC key for book 0 page 0 is stored in reader key slot 33 (0x21)
                bool isImplicitSelect = false;
                var book = BookNumber.Book0;
                var page = PageNumber.Page0;
                var keyType = GeneralAuthenticateCommand.IClassKeyType.PicoPassCreditKeyKC;
                byte keySlotNumber = 0x21;

                string command = contactlessCommands.GeneralAuthenticate.GetiClassApdu(book, page, isImplicitSelect, keyType, keySlotNumber);
                string response = secureChannel.SendCommand(command);
                string result = response == "9000" ? "Success" : "Error";
                PrintData($"Authenticate {book}, {page}", command, response, $"Key type: {keyType}, key from slot {keySlotNumber:D}, {result}");

                // Update block number 20
                byte blockNumber = 20;
                string data = "BACDEF0122345678";

                command = contactlessCommands.UpdateBinary.GetApdu(UpdateBinaryCommand.Type.Default, blockNumber, data);
                response = secureChannel.SendCommand(command);
                result = response == "9000" ? "Success" : "Error";
                PrintData($"Write 8 byte long data to  {book}, {page}, block: {blockNumber}", command, response, result);

                // terminate session   
                secureChannel.TerminateSecureSession();
            }
        }
    }
}
