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
using HidGlobal.OK.SampleCodes.Utilities;
using System.Linq;

namespace HidGlobal.OK.SampleCodes
{
    static class Program
    {
        public static Readers.Components.ContextHandler WinscardContext = new Readers.Components.ContextHandler();

        private static List<string> _activeReaders = new List<string>();
        private static Menu _mainMenu;

        private static readonly List<string> _supportedReaders = new List<string>(new[]
        {
            "HID Global OMNIKEY 5022 Smart Card Reader",
            "HID Global OMNIKEY 5023 Smart Card Reader",
            "HID Global OMNIKEY 5122 Smartcard Reader",
            "HID Global OMNIKEY 5122CL Smartcard Reader",
            "HID Global OMNIKEY 5422 Smartcard Reader",
            "HID Global OMNIKEY 5422CL Smartcard Reader",
        });

        private static int Main(string[] args)
        {
            WinscardContext.Establish(Readers.Components.Scope.System);

            using (WinscardContext)
            {
                // Create instance of main menu with reader selection
                CreateMenu();
                UpdateMenu();
                _mainMenu.RunMenu();
            }
            
            return 0;
        }

        /// <summary>
        /// Seeks for supported readers and add them to the dictionary.
        /// </summary>
        private static void RefreshReaderList()
        {
            _activeReaders.Clear();

            var readerNames = WinscardContext.ListReaders();

            if (readerNames != null)
            {
                _activeReaders = readerNames.Where(readerName => _supportedReaders.Any(readerName.Contains)).ToList();
            }
            else
            {
                ConsoleWriter.Instance.PrintMessage("None of supported readers has been found..");
                ConsoleWriter.Instance.WaitKeyPress();
            }
        }

        private static void CreateMenu()
        {
            _mainMenu = new Menu("HID OMNIKEY Smart Card Readers' Sample Codes Application Menu", false)
            {
                new MenuEntry("Refresh reader list", new Action(UpdateMenu))
            };
        }

        private static void UpdateMenu()
        {
            const int menuOffset = 2;
            // Clears menu options.
            _mainMenu.RemoveRange(menuOffset, _mainMenu.Count - menuOffset);
            RefreshReaderList();
            foreach (var readerName in _activeReaders)
            {
                _mainMenu.Add(new MenuEntry(readerName, new Action(() => { RunReaderMenu(readerName); })));
            }
        }

        private static void RunReaderMenu(string readerName)
        {
            if (readerName.Contains("5022"))
            {
                var sample = new AViatoR.Ok5022Samples(readerName);
                sample.Menu.RunMenu();
            }
            else if (readerName.Contains("5023"))
            {
                var sample = new AViatoR.Ok5023Samples(readerName);
                sample.Menu.RunMenu();
            }
            else if (readerName.Contains("5122"))
            {
                var sample = new AViatoR.Ok5122Samples(readerName);
                sample.Menu.RunMenu();
            }
            else if(readerName.Contains("5422"))
            {
                var sample = new AViatoR.Ok5422Samples(readerName);
                sample.Menu.RunMenu();
            }
        }
        
    }
}
