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
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    public class ExampleWithIso15693
    {
        public class ReadBinaryNXPiCode
        {
            void SendReadBinaryCommand(IReader reader, byte msb, byte lsb, byte expectedLength)
            {
                ConsoleWriter.Instance.PrintMessage($"Read Binary NXP iCode card, address: 0x{msb:X2}{lsb:X2}");

                var readBinary = new ReadBinaryCommand();
                string input = readBinary.GetApdu(msb, lsb, expectedLength);
                var output = reader.Transmit(input);

                ConsoleWriter.Instance.PrintCommand(string.Empty, input, output);
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

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    SendReadBinaryCommand(reader, 0x00, 0x00, 0x00);

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
        public class UpdateBinaryNXPiCode
        {
            void SendUpdateBinaryCommand(IReader reader, UpdateBinaryCommand.Type type, byte blockNumber, string data)
            {
                ConsoleWriter.Instance.PrintMessage($"Update Binary NXP iCode card, block number: 0x{blockNumber:X2}, with data :{data}");

                var updateBinary = new UpdateBinaryCommand();
                string input = updateBinary.GetApdu(type, blockNumber, data);
                var output = reader.Transmit(input);

                ConsoleWriter.Instance.PrintCommand(string.Empty, input, output);
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

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    SendUpdateBinaryCommand(reader, UpdateBinaryCommand.Type.Plain, 0x00, "ABCDEF01");

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
