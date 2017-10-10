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
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    public class ReaderEepromSample
    {
        private static void PrintCommand(string name, string input, string output)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
        }
        public class WriteEeprom
        {
            private void WriteEepromCommand(IReader reader, string comment, ushort offset, string dataToWrite)
            {
                var eepromCommands = new Readers.AViatoR.Components.ReaderEeprom();

                string input = eepromCommands.WriteCommand(offset, dataToWrite);
                string output = ReaderHelper.SendCommand(reader, input);

                PrintCommand(comment, input, output);
            }
            void ExecuteExample(IReader reader)
            {
                WriteEepromCommand(reader, "Write 1 byte of FF with offset address 0x0001", 0x0001, "FF");
                WriteEepromCommand(reader, "Write 16 bytes of FF with offset address 0x0001", 0x0001, "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
                WriteEepromCommand(reader, "Write 128 bytes of FF with offset address 0x0001", 0x0001,
                    "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                    "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                    "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                    "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            }
            public void Run(string readerName)
            {
                using (var reader = new Reader(Program.WinscardContext.Handle, readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader);

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

        public class ReadEeprom
        {
            private void ReadEepromCommand(IReader reader, string comment, ushort offset, byte dataLength)
            {
                var eepromCommands = new Readers.AViatoR.Components.ReaderEeprom();

                string input = eepromCommands.ReadCommand(offset, dataLength);
                string output = ReaderHelper.SendCommand(reader, input);

                PrintCommand(comment, input, output);
            }
            private void ExecuteExample(IReader reader)
            {
                ReadEepromCommand(reader, "Read 1 byte with offset address 0x0000", 0x0000, 0x01);
                ReadEepromCommand(reader, "Read 16 bytes with offset address 0x00F0", 0x00F0, 0x10);
                ReadEepromCommand(reader, "Read 128 bytes with offset address 0x0100", 0x0100, 0x80);
            }
            public void Run(string readerName)
            {
                using (var reader = new Reader(Program.WinscardContext.Handle, readerName))
                {
                    try
                    {
                        ConsoleWriter.Instance.PrintSplitter();
                        ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                        ReaderHelper.ConnectToReader(reader);

                        ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");

                        ExecuteExample(reader);

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
}
