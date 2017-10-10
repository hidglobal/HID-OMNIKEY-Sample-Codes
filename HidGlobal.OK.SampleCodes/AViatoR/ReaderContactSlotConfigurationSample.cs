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
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.Components;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    static class ReaderContactSlotConfigurationSample
    {
        private static void PrintData(string title, string command, string response, string data)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"<-- {command}\n--> {response}\n{title}: {data}");
        }
        private static void PrintData(string title, string command, string response, string[] data)
        {
            if (data == null || data.Length <= 0)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"<-- {command}\n--> {response}\n{title}: n/a");
            }
            else if (data.Length == 1)
                PrintData(title, command, response, data[0]);
            else
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"<-- {command}\n--> {response}\n{title}:\n\t{data.ToList().Aggregate((i, j) => i + "\n\t" + j)}");
            }
        }
        private static IReader Connect(string readerName)
        {
            var reader = new Reader(Program.WinscardContext.Handle, readerName);

            var readerState = Program.WinscardContext.GetReaderState(reader.PcscReaderName);
            if (readerState.AtrLength > 0)
                reader.Connect(ReaderSharingMode.Shared, Protocol.Any);
            else
                reader.ConnectDirect();

            return reader;
        }
        public static void ReadContactSlotConfiguration(string readerName)
        {
            var contactSlot = new Readers.AViatoR.Components.ContactSlotConfiguration();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command;
            string response;

            // Read contact slot enable
            command = contactSlot.ContactSlotEnable.GetApdu;
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Contact Slot", command, response, contactSlot.ContactSlotEnable.TranslateGetResponse(response));

            // Read operating mode
            command = contactSlot.OperatingMode.GetApdu;
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Operating Mode", command, response, contactSlot.OperatingMode.TranslateGetResponse(response).ToString());

            // Read contact slot enable
            command = contactSlot.VoltageSequence.GetApdu;
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            List<VoltageSequenceFlags> voltageSequenceList = contactSlot.VoltageSequence.TranslateGetResponse(response);
            if (voltageSequenceList.Count == 0)
            {
                PrintData("Voltage Sequence", command, response, "Device driver decides.");
            }
            else
            {
                PrintData("Voltage Sequence", command, response, voltageSequenceList.Select(item => item.ToString()).ToArray());
            }

            // close connection
            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetContactSlotEnable(string readerName)
        {
            var contactSlot = new Readers.AViatoR.Components.ContactSlotConfiguration();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command;
            string response;
            
            //enable
            command = contactSlot.ContactSlotEnable.SetApdu(true);
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Contact Slot", command, response, "Enable");

            //disable
            // command = contactSlot.ContactSlotEnable.SetApdu(false);
            // response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Contact Slot", command, response, "Disable");

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetOperatingMode(string readerName)
        {
            var contactSlot = new Readers.AViatoR.Components.ContactSlotConfiguration();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command;
            string response;

            // Set ISO7816 mode
            command = contactSlot.OperatingMode.SetApdu(OperatingModeFlags.Iso7816);
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Operating Mode", command, response, "ISO 7816 mode");

            // Set EMVco mode
            // command = contactSlot.OperatingMode.SetApdu(OperatingModeFlags.EMVCo);
            // response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Set Operating Mode", command, response, "EMVco mode");

            reader.Disconnect(CardDisposition.Unpower);
        }

        public static void SetVoltageSequence(string readerName)
        {
            var contactSlot = new Readers.AViatoR.Components.ContactSlotConfiguration();

            IReader reader = Connect(readerName);

            if (!reader.IsConnected)
                return;

            string command;
            string response;

            // Device Driver decides
            command = contactSlot.VoltageSequence.SetAutomaticSequenceApdu();
            response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            PrintData("Set Voltage Sequence", command, response, "Device driver decides");

            // High Mid Low
            // command = contactSlot.VoltageSequence.SetApdu(VoltageSequenceFlags.High, VoltageSequenceFlags.Mid, VoltageSequenceFlags.Low);
            // response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Voltage Sequence", command, response, "High -> Mid -> Low");

            // Low Mid High
            // command = contactSlot.VoltageSequence.SetApdu(VoltageSequenceFlags.Low, VoltageSequenceFlags.Mid, VoltageSequenceFlags.High);
            // response = reader.ConnectionMode != ReaderSharingMode.Direct ? reader.Transmit(command) : reader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
            // PrintData("Voltage Sequence", command, response, "Low -> Mid -> High");

            reader.Disconnect(CardDisposition.Unpower);
        }

    }
}
