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
    static class ReaderConfigurationControlSample
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
        public static void RestoreFactoryDefaults(string readerName)
        {
            var configurationControl = new Readers.AViatoR.Components.ReaderConfigurationControl();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = configurationControl.RestoreFactoryDefaults.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Restore Factory Defaults", command, response);
            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void RebootReader(string readerName)
        {
            var configurationControl = new Readers.AViatoR.Components.ReaderConfigurationControl();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = configurationControl.RebootDevice.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Reboot Reader", command, response);
            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void ApplySettings(string readerName)
        {
            var configurationControl = new Readers.AViatoR.Components.ReaderConfigurationControl();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command = configurationControl.ApplySettings.GetApdu;
            string response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Apply Settings", command, response);
            reader.Disconnect(CardDisposition.Unpower);
        }
    }
}