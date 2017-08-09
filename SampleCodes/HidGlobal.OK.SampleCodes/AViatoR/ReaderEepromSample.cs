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

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    static class ReaderEepromSample
    {
        private static void PrintData(string title, string command, string response)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"{title}:\n<-- {command}\n--> {response}");
        }
        private static IReader Connect(string readerName)
        {
            if (!Program.WinscardContext.IsValid())
                Program.WinscardContext.Establish(Scope.System);

            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            var readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (readerState.AtrLength > 0)
                reader.Connect(ReaderSharingMode.Shared, Protocol.Any);
            else
                reader.ConnectDirect();

            return reader;
        }
        public static void WriteEeprom(string readerName)
        {
            var eeprom = new Readers.AViatoR.Components.ReaderEeprom();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command;
            string response;


            // write 1 byte of FF
            command = eeprom.WriteCommand(0x0001, "FF");
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Write 1 byte of FF with offset address 0x0001", command, response);

            // write 16 bytes of FF starting from address 0x0001
            command = eeprom.WriteCommand(0x0001, "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Write 16 bytes of FF with offset address 0x0001", command, response);

            // write 128 bytes of FF starting from address 0x0100
            string data = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                          "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
            command = eeprom.WriteCommand(0x0001, data);
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Write 128 bytes of FF with offset address 0x0001", command, response);

            reader.Disconnect(CardDisposition.Unpower);
        }
        public static void ReadEeprom(string readerName)
        {
            var eeprom = new Readers.AViatoR.Components.ReaderEeprom();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command;
            string response;


            // Read 1 byte from eeprom
            command = eeprom.ReadCommand(0x0000, 0x01);
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Read 1 byte with offset address 0x0000", command, response);

            // Read 16 bytes starting from address 0x00F0
            command = eeprom.ReadCommand(0x00F0, 0x10);
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Read 16 bytes with offset address 0x00F0", command, response);

            // Read 128 bytes starting from address 0x0100
            command = eeprom.ReadCommand(0x0100, 0x80);
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Read 128 bytes with offset address 0x0100", command, response);

            reader.Disconnect(CardDisposition.Unpower);
        }
    }
}
