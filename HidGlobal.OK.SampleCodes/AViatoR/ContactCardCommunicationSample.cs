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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.Components;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    static class ContactCardCommunicationSample
    {
        private static void PrintData(string title, string command, string response, string data)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"<-- {command}\n--> {response}\n{title}: {data}");
        }
        /// <summary>
        /// Establishes connection between given reader and a smart card and returns object implementing IReader interface 
        /// capable of interaction with a card, throws an exception if no card available.
        /// </summary>
        /// <param name="readerName">Reader name seen by smart card resource manager.</param>
        /// <returns></returns>
        private static IReader Connect(string readerName)
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
            throw new Exception("No Smart Card Available in contact slot.");
        }
        public static void ReadMainMemory2WbpExample(string readerName)
        {
            try
            {
                var twoWireBusProtocol = new Readers.AViatoR.Components.Synchronus2WBP();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;

                // address of first byte to be read
                byte address = 0x20;
                byte notUsed = 0x00;
                string cardMemory = string.Empty;
                for (int i = 32; i < 256; i++)
                {
                    string command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.ReadMainMemory, address, notUsed);
                    string response = reader.Transmit(command);
                    if (response.StartsWith("9D01") && response.EndsWith("9000"))
                    {
                        string data = response.Substring(4, 2);
                        PrintData($"Read Main Memory, Address 0x{address:X2}", command, response, $"Value 0x{data}");
                        cardMemory += data;
                    }
                    else
                    {
                        PrintData($"Read Main Memory, Address 0x{address:X2}", command, response, "Error Response");
                    }
                    address++;
                }
                Console.WriteLine($"\nMain Memory starting from address 0x20 to address 0xFF:\n{cardMemory}\n");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void ReadProtectionMemory2WbpExample(string readerName)
        {
            try
            {
                var twoWireBusProtocol = new Readers.AViatoR.Components.Synchronus2WBP();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;

                byte notUsed = 0x00;

                string command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.ReadProtectionMemory, notUsed, notUsed);
                string response = reader.Transmit(command);
                if (response.StartsWith("9D04") && response.EndsWith("9000"))
                {
                    string data = response.Substring(4, 8);
                    PrintData("Read Protection Memory", command, response, $"Value 0x{data}");
                }
                else
                {
                    PrintData("Read Protection Memory", command, response, "Error Response");
                }
                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void UpdateMainMemory2WbpExample(string readerName)
        {
            try
            {
                var twoWireBusProtocol = new Readers.AViatoR.Components.Synchronus2WBP();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;
                // Following code is commented to prevent usage of incorrect Pin code, which can lead to blocking the synchronus card if done three times in row
                // Verification with correct pin code is necessary to write any data into card memory
                // Be advice not to use VerifyUser2Wbp command if correct pin code is not known 
                
                /*
                string pin = "FFFFFF";

                byte firstPinByte = byte.Parse(pin.Substring(0, 2), NumberStyles.HexNumber);
                byte secondPinByte = byte.Parse(pin.Substring(2, 2), NumberStyles.HexNumber);
                byte thirdPinByte = byte.Parse(pin.Substring(4, 2), NumberStyles.HexNumber);

                VerifyUser2Wbp(reader, firstPinByte, secondPinByte, thirdPinByte);
                //*/

                byte address = 0x20;
                byte data = 0x01;
                
                string command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.UpdateMainMemory, address, data);
                string response = reader.Transmit(command);
                PrintData($"Write Main Memory, address: 0x{address:X2}, data: 0x{data:X2}", command, response, response.Equals("9D009000") ? "Success" : "Error Response");

                address = 0x20;
                data = 0xF0;
                command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.UpdateMainMemory, address, data);
                response = reader.Transmit(command);
                PrintData($"Write Main Memory, address: 0x{address:X2}, data: 0x{data:X2}", command, response, response.Equals("9D009000") ? "Success" : "Error Response");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void VerifyUser2Wbp(IReader reader, byte firstPinByte, byte secondPinByte, byte thirdPinByte)
        {
            var twoWireBusProtocol = new Readers.AViatoR.Components.Synchronus2WBP();

            byte notUsed = 0x00;
            byte newErrorCounter;
            Console.WriteLine("User Verification");

            // Read Error Counter
            string command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.ReadSecurityMemory, notUsed, notUsed);
            string response = reader.Transmit(command);
            string currentErrorCounter = response.Substring(4, 2);
            PrintData("Read Error Counter", command, response, $"0x{currentErrorCounter}");
            
            // decrement counter
            switch (currentErrorCounter)
            {
                case "07":
                    newErrorCounter = 0x06;
                    break;
                case "06":
                    newErrorCounter = 0x04;
                    break;
                case "04":
                    newErrorCounter = 0x00;
                    break;
                default:
                    Console.WriteLine("Returned error counter is not correct or card is blocked");
                    return;
            }
            command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.UpdateSecurityMemory, 0x00, newErrorCounter);
            response = reader.Transmit(command);
            PrintData("Write new Error Counter", command, response, $"0x{newErrorCounter:X2}");
            
            // Compare verification data - first part
            command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.CompareVerificationData, 0x01, firstPinByte);
            response = reader.Transmit(command);
            PrintData("Compare verification data - first part", command, response, $"{firstPinByte:X2}");
            
            // Compare verification data - second part
            command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.CompareVerificationData, 0x02, secondPinByte);
            response = reader.Transmit(command);
            PrintData("Compare verification data - second part", command, response, $"{secondPinByte:X2}");
            
            // Compare verification data - third part
            command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.CompareVerificationData, 0x03, thirdPinByte);
            response = reader.Transmit(command);
            PrintData("Compare verification data - third part", command, response, $"{thirdPinByte:X2}");
            
            // Reset Error Counter
            command = twoWireBusProtocol.GetApdu(Synchronus2WBP.ControlByte.UpdateSecurityMemory, 0x00, 0xFF);
            response = reader.Transmit(command);
            PrintData("Reset Error Counter", command, response, "");
        }
        private static void VerifyUser3Wbp(IReader reader, byte firstPinByte, byte secondPinByte)
        {
            var threeWireBusProtocol = new Readers.AViatoR.Components.Synchronus3WBP();

            byte newErrorCounter;
            Console.WriteLine("User Verification");

            // Read Error Counter
            string command = threeWireBusProtocol.ReadErrorCounterApdu();
            string response = reader.Transmit(command);
            string currentErrorCounter = response.Substring(4, 2);
            PrintData("Read Error Counter", command, response, $"0x{currentErrorCounter}");
            
            // decrement counter
            switch (currentErrorCounter)
            {
                case "7F":
                    newErrorCounter = 0x7E;
                    break;
                case "7E":
                    newErrorCounter = 0x7C;
                    break;
                case "7C":
                    newErrorCounter = 0x78;
                    break;
                case "78":
                    newErrorCounter = 0x70;
                    break;
                case "70":
                    newErrorCounter = 0x60;
                    break;
                case "60":
                    newErrorCounter = 0x40;
                    break;
                case "40":
                    newErrorCounter = 0x00;
                    break;
                default:
                    Console.WriteLine("Returned error counter is not correct or card is blocked");
                    return;
            }
            command = threeWireBusProtocol.WriteErrorCounterApdu(newErrorCounter);
            response = reader.Transmit(command);
            PrintData("Write new Error Counter", command, response, $"0x{newErrorCounter:X2}");

            // Verify pin first byte
            command = threeWireBusProtocol.VerifyFirstPinByte(firstPinByte);
            response = reader.Transmit(command);
            PrintData("Verify first pin byte", command, response, $"{firstPinByte:X2}");

            // Verify pin second byte
            command = threeWireBusProtocol.VerifySecondPinByte(secondPinByte);
            response = reader.Transmit(command);
            PrintData("Verify second pin byte", command, response, $"{secondPinByte:X2}");

            // Reset Error Counter
            command = threeWireBusProtocol.ResetErrorCounterApdu();
            response = reader.Transmit(command);
            PrintData("Reset Error Counter", command, response, "");
        }
        public static void ReadMainMemory3WbpExample(string readerName)
        {
            try
            {
                var threeWireBusProtocol = new Readers.AViatoR.Components.Synchronus3WBP();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;

                // address of first byte to be read
                ushort address = 0x00;
                byte notUsed = 0x00;
                string cardMemory = string.Empty;

                // read data from addresses 0x0000 - 0x03FF
                for (int i = 0x00; i < 0x0400; i++)
                {
                    string command = threeWireBusProtocol.GetApdu(Synchronus3WBP.ControlByte.Read9BitsDataWithProtectBit, address, notUsed);
                    string response = reader.Transmit(command);

                    if (response.StartsWith("9D02") && response.EndsWith("9000"))
                    {
                        string data = response.Substring(4, 2);
                        string protectionByte = response.Substring(6, 2);
                        string bitSet = protectionByte != "00" ? "not set" : "set";
                        PrintData($"Read Main Memory, Address 0x{address:X4}", command, response,
                            $"Value 0x{data}, {protectionByte} -> protection bit {bitSet}");
                        cardMemory += data;
                    }
                    else
                    {
                        PrintData($"Read Main Memory, Address 0x{address:X4}", command, response, "Error Response");
                    }
                    address++;
                }
                Console.WriteLine($"\nMain Memory starting from address 0x0000 to address 0x03FF:\n{cardMemory}\n");
                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void UpdateMainMemory3WbpExample(string readerName)
        {
            try
            {
                var threeWireBusProtocol = new Readers.AViatoR.Components.Synchronus3WBP();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;
                // Following code is commented to prevent usage of incorrect Pin code, which can lead to blocking the synchronus card if done several times in row
                // Verification with correct pin code is necessary to write any data into card memory
                // Be advice not to use VerifyUser2Wbp command if correct pin code is not known 
                /*
                string pin = "FFFF";

                byte firstPinByte = byte.Parse(pin.Substring(0, 2), NumberStyles.HexNumber);
                byte secondPinByte = byte.Parse(pin.Substring(2, 2), NumberStyles.HexNumber);

                VerifyUser3Wbp(reader, firstPinByte, secondPinByte);
                
                //*/

                ushort address = 0x01FF;
                byte data = 0xAB;
                
                string command = threeWireBusProtocol.GetApdu(Synchronus3WBP.ControlByte.WriteAndEraseWithoutProtectBit, address, data);
                string response = reader.Transmit(command);
                PrintData($"Write Main Memory, address: 0x{address:X4}, data: 0x{data:X2}", command, response, response.Equals("9D009000") ? "Success" : "Error Response");

                address = 0x01FF;
                data = 0xCC;
                command = threeWireBusProtocol.GetApdu(Synchronus3WBP.ControlByte.WriteAndEraseWithoutProtectBit, address, data);
                response = reader.Transmit(command);
                PrintData($"Write Main Memory, address: 0x{address:X4}, data: 0x{data:X2}", command, response, response.Equals("9D009000") ? "Success" : "Error Response");


                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void ReadI2CExampleAt24C16(string readerName)
        {
            try
            {
                var i2c = new Readers.AViatoR.Components.SynchronusI2C();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;

                // Read 1 byte from address 0x0000 from AT24C16 card
                ushort address = 0x0000;
                byte bytesToRead = 0x01;

                string command = i2c.GetReadCommandApdu(SynchronusI2C.MemorySize._2048, address, bytesToRead);
                string response = reader.Transmit(command);
                PrintData($"I2C Read {bytesToRead} byte from address 0x{address:X6}", command, response, "");

                // Read 10 bytes from address 0x0010 from AT24C16 card
                address = 0x0010;
                bytesToRead = 0x0A;
                command = i2c.GetReadCommandApdu(SynchronusI2C.MemorySize._2048, address, bytesToRead);
                response = reader.Transmit(command);
                PrintData($"I2C Read {bytesToRead} bytes starting from address 0x{address:X6}", command, response, "");

                // Read 32 bytes from address 0x0400 from AT24C16 card
                address = 0x0400;
                bytesToRead = 0x20;
                command = i2c.GetReadCommandApdu(SynchronusI2C.MemorySize._2048, address, bytesToRead);
                response = reader.Transmit(command);
                PrintData($"I2C Read {bytesToRead} bytes starting from address 0x{address:X6}", command, response, "");
                
                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void WriteI2CExampleAt24C16(string readerName)
        {
            try
            {
                var i2c = new Readers.AViatoR.Components.SynchronusI2C();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;

                // Write 1 byte to address 0x0000, AT24C16 card
                ushort address = 0x0000;
                byte numberOfBytesToWrite = 0x01;
                string data = "01";

                string command = i2c.GetWriteCommandApdu(SynchronusI2C.MemorySize._2048, address, numberOfBytesToWrite, data);
                string response = reader.Transmit(command);
                PrintData($"I2C Write {numberOfBytesToWrite} byte to address 0x{address:X6}", command, response, "");

                // Write 10 bytes to address 0x0010, AT24C16 card
                address = 0x0010;
                numberOfBytesToWrite = 0x0A;
                data = "FFFFFFFFFFFFFFFFFFFF";
                command = i2c.GetWriteCommandApdu(SynchronusI2C.MemorySize._2048, address, numberOfBytesToWrite, data);
                response = reader.Transmit(command);
                PrintData($"I2C Write {numberOfBytesToWrite} bytes starting from address 0x{address:X6}", command, response, "");

                // Write 32 bytes to address 0x0400, AT24C16 card
                // AT24C16 card has page size of 16 bytes so write operation need to be done twice
                address = 0x0400;
                numberOfBytesToWrite = 0x10;
                data = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                command = i2c.GetWriteCommandApdu(SynchronusI2C.MemorySize._2048, address, numberOfBytesToWrite, data);
                response = reader.Transmit(command);
                PrintData($"I2C Write {numberOfBytesToWrite} bytes starting from address 0x{address:X6}", command, response, "");
                // second part of write operation
                address += 16;
                numberOfBytesToWrite = 0x10;
                data = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                command = i2c.GetWriteCommandApdu(SynchronusI2C.MemorySize._2048, address, numberOfBytesToWrite, data);
                response = reader.Transmit(command);
                PrintData($"I2C Write {numberOfBytesToWrite} bytes starting from address 0x{address:X6}", command, response, "");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void ReadI2CExampleAt24C128(string readerName)
        {
            try
            {
                var i2c = new Readers.AViatoR.Components.SynchronusI2C();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;

                // Read 1 byte from address 0x0000 from AT24C128 card
                ushort address = 0x0000;
                byte bytesToRead = 0x01;

                string command = i2c.GetReadCommandApdu(SynchronusI2C.MemorySize._16384, address, bytesToRead);
                string response = reader.Transmit(command);
                PrintData($"I2C Read {bytesToRead} byte from address 0x{address:X6}", command, response, "");

                // Read 10 bytes from address 0x0010 from AT24C128 card
                address = 0x0010;
                bytesToRead = 0x0A;
                command = i2c.GetReadCommandApdu(SynchronusI2C.MemorySize._16384, address, bytesToRead);
                response = reader.Transmit(command);
                PrintData($"I2C Read {bytesToRead} bytes starting from address 0x{address:X6}", command, response, "");

                // Read 32 bytes from address 0x0400 from AT24C128 card
                address = 0x0400;
                bytesToRead = 0x20;
                command = i2c.GetReadCommandApdu(SynchronusI2C.MemorySize._16384, address, bytesToRead);
                response = reader.Transmit(command);
                PrintData($"I2C Read {bytesToRead} bytes starting from address 0x{address:X6}", command, response, "");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void WriteI2CExampleAt24C128(string readerName)
        {
            try
            {
                var i2c = new Readers.AViatoR.Components.SynchronusI2C();
                IReader reader = Connect(readerName);

                if (!reader.IsConnected)
                    return;

                // Write 1 byte to address 0x0000, AT24C128 card
                ushort address = 0x0000;
                byte numberOfBytesToWrite = 0x01;
                string data = "FF";

                string command = i2c.GetWriteCommandApdu(SynchronusI2C.MemorySize._16384, address, numberOfBytesToWrite, data);
                string response = reader.Transmit(command);
                PrintData($"I2C Write {numberOfBytesToWrite} byte to address 0x{address:X6}", command, response, "");

                // Write 10 bytes to address 0x0010, AT24C128 card
                address = 0x0010;
                numberOfBytesToWrite = 0x0A;
                data = "FFFFFFFFFFFFFFFFFFFF";
                command = i2c.GetWriteCommandApdu(SynchronusI2C.MemorySize._16384, address, numberOfBytesToWrite, data);
                response = reader.Transmit(command);
                PrintData($"I2C Write {numberOfBytesToWrite} bytes starting from address 0x{address:X6}", command, response, "");

                // Write 32 bytes to address 0x0400, AT24C128 card
                address = 0x0400;
                numberOfBytesToWrite = 0x20;
                data = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                command = i2c.GetWriteCommandApdu(SynchronusI2C.MemorySize._16384, address, numberOfBytesToWrite, data);
                response = reader.Transmit(command);
                PrintData($"I2C Write {numberOfBytesToWrite} bytes starting from address 0x{address:X6}", command, response, "");

                reader.Disconnect(CardDisposition.Unpower);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
