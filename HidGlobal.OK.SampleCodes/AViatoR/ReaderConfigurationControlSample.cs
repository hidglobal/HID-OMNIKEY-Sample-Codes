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
    public class ReaderConfigurationControlSample
    {
        private static void PrintCommand(string name, string input, string output)
        {
            ConsoleWriter.Instance.PrintSplitter();
            ConsoleWriter.Instance.PrintCommand(name, input, output);
        }

        public class RestoreFactoryDefaults
        {
            private void RestoreFactoryDefaultsCommand(IReader reader)
            {
                var resotoreFactoryDefaults = new Readers.AViatoR.Components.ResotoreFactoryDefaults();

                ConsoleWriter.Instance.PrintMessage("Restore Factory Defaults");

                string input = resotoreFactoryDefaults.GetApdu;
                string output = ReaderHelper.SendCommand(reader, input);

                PrintCommand(string.Empty, input, output);
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

                        RestoreFactoryDefaultsCommand(reader);

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
        public class RebootDevice
        {
            private void RebootDeviceCommand(IReader reader)
            {
                var rebootDevice = new Readers.AViatoR.Components.RebootDevice();

                ConsoleWriter.Instance.PrintMessage("Reboot Device");

                string input = rebootDevice.GetApdu;
                string output = ReaderHelper.SendCommand(reader, input);

                PrintCommand(string.Empty, input, output);
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

                        RebootDeviceCommand(reader);

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
        public class ApplySettings
        {
            private void ApplySettingsCommand(IReader reader)
            {
                var applySettings = new Readers.AViatoR.Components.ApplySettings();

                ConsoleWriter.Instance.PrintMessage("Apply Settings");

                string input = applySettings.GetApdu;
                string output = ReaderHelper.SendCommand(reader, input);

                PrintCommand(string.Empty, input, output);
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

                        ApplySettingsCommand(reader);

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