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
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.SampleCodes.Utilities;
using System;
using System.Linq;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    class ExampleWithMifareClassic
    {
        public class LoadKeyExample
        {
            private void LoadKeyCommand(IReader reader, string description, byte keySlot, LoadKeyCommand.KeyType keyType, LoadKeyCommand.Persistence persistence, LoadKeyCommand.Transmission transmission, LoadKeyCommand.KeyLength keyLength, string key)
            {
                var loadKeyCommand = new Readers.AViatoR.Components.LoadKeyCommand();

                string input = loadKeyCommand.GetApdu(keySlot, keyType, persistence, transmission, keyLength, key);
                string output = ReaderHelper.SendCommand(reader, input);
                ConsoleWriter.Instance.PrintCommand(description + key, input, output);
            }
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to { reader.PcscReaderName}");

                    ReaderHelper.ConnectToReader(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    LoadKeyCommand(reader, "Load Mifare Key: ", 0x02,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                        Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                        Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, "FFFFFFFFFFFF");

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class ReadBinaryMifareClassic1kExample
        {
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    ReaderHelper.ReadBinaryMifareCommand(reader, "Read Binary block nr ", 0x04, 0x00);

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x05,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    ReaderHelper.ReadBinaryMifareCommand(reader, "Read Binary block nr ", 0x05, 0x00);

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x06,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    ReaderHelper.ReadBinaryMifareCommand(reader, "Read Binary block nr ", 0x06, 0x00);

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x07,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    ReaderHelper.ReadBinaryMifareCommand(reader, "Read Binary block nr ", 0x07, 0x00);
                    
                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class UpdateBinaryMifareClassic1kExample
        {
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    ReaderHelper.UpdateBinaryCommand(reader, "Update Binary block nr ", UpdateBinaryCommand.Type.Plain, 0x04, "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x05,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    ReaderHelper.UpdateBinaryCommand(reader, "Update Binary block nr ", UpdateBinaryCommand.Type.Plain, 0x05, "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x06,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    ReaderHelper.UpdateBinaryCommand(reader, "Update Binary block nr ", UpdateBinaryCommand.Type.Plain, 0x06, "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
                    
                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class IncrementMifareClassic1kExample
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte) ~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte) ~blockNumber).ToString("X2");
            }
            void SendIncrementCommand(IReader reader, string description, int value, byte blockNumber)
            {
                var incrementCommand = new IncrementCommand();
                string input = incrementCommand.GetApdu(blockNumber, value);
                string output = ReaderHelper.SendCommand(reader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendIncrementCommand(reader, "Increment value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class DecrementMifareClassic1kExample
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2");
            }
            void SendDecrementCommand(IReader reader, string description, int value, byte blockNumber)
            {
                var decrementCommand = new DecrementCommand();
                string input = decrementCommand.GetApdu(blockNumber, value);
                string output = ReaderHelper.SendCommand(reader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendDecrementCommand(reader, "Decrement value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class IncrementMifareClassic1kForOK5023Example
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2");
            }
            void SendIncrementCommand(IReader reader, string description, int value, byte blockNumber)
            {
                var incrementCommand = new IncrementDecrementCommand();
                string input = incrementCommand.GetApdu(IncrementDecrementCommand.OperationType.Increment, blockNumber, value);
                string output = ReaderHelper.SendCommand(reader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendIncrementCommand(reader, "Increment value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class DecrementMifareClassic1kForOK5023Example
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2");
            }
            void SendDecrementCommand(IReader reader, string description, int value, byte blockNumber)
            {
                var decrementCommand = new IncrementDecrementCommand();
                string input = decrementCommand.GetApdu(IncrementDecrementCommand.OperationType.Decrement, blockNumber, value);
                string output = ReaderHelper.SendCommand(reader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new Reader(Program.WinscardContext.Handle, readerName);
                
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);
                    
                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendDecrementCommand(reader, "Decrement value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
    }
}
