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
using System.Linq;

namespace HidGlobal.OK.SampleCodes.Utilities
{
    public interface IConsoleWriter
    {
        void PrintCommand(string title, string input, string output);
        void PrintCommand(string title, string input, string output, string translatedResponse);
        void PrintCommand(string title, string input, string output, string[] translatedResponse);
        void PrintError(string message);
        void PrintTask(string task);
        void PrintMessage(string message);
        void PrintSplitter();
        void Wait(int milliseconds);
        void WaitKeyPress();
    }

    public class ConsoleWriter : IConsoleWriter
    {
        private const string Splitter = "-----------------------------------";
        private static IConsoleWriter _instance;

        public static IConsoleWriter Instance => _instance ?? (_instance = new ConsoleWriter());

        public void PrintCommand(string title, string input, string output)
        {   
            Console.WriteLine($"Sending command: {title}");
            Console.WriteLine($"<-- {input}");
            Console.WriteLine($"--> {output}");
        }

        public void PrintCommand(string title, string input, string output, string translatedResponse)
        {
            PrintCommand(title, input, output);
            Console.WriteLine($"Response: {translatedResponse}");
        }

        public void PrintCommand(string title, string input, string output, string[] translatedResponse)
        {
            PrintCommand(title, input, output);
            Console.WriteLine("Response:");

            if (translatedResponse != null)
            {
                translatedResponse.ToList().ForEach(x => Console.WriteLine($"\t{x}"));
            }
        }

        public void PrintError(string message)
        {
            Console.WriteLine($"An error has occured: {message}");
        }

        public void PrintTask(string task)
        {
            Console.WriteLine(task + " ...");
        }

        public void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void PrintSplitter()
        {
            Console.WriteLine(Splitter);
        }

        public void Wait(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void WaitKeyPress()
        {
            PrintMessage("Press any key to continue..");
            Console.ReadKey();
        }

        private ConsoleWriter() {}
    }
}
