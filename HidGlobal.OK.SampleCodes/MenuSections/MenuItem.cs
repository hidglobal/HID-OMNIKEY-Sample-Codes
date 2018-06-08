/*****************************************************************************************
    (c) 2017-2018 HID Global Corporation/ASSA ABLOY AB.  All rights reserved.

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
using HidGlobal.OK.SampleCodes.Utilities;

namespace HidGlobal.OK.SampleCodes.MenuSections
{
    public class MenuItem : IMenuItem
    {
        private readonly Action _command;
        private readonly bool _isMainMenu;
        private readonly bool _waitKeyPress;
        private readonly string _description;
        private readonly List<IMenuItem> _subItems = new List<IMenuItem>();

        private bool IsTreeLeaf => !_subItems.Any();
        private string ReturnItemDescription => _isMainMenu ? "Quit" : "Back";

        public string Description => _description; 
        public Action Command => _command;

        public MenuItem(string description, bool isMainMenu = false) : this(description, null, false)
        {
            _isMainMenu = isMainMenu;
        }

        private MenuItem(string description, Action command, bool waitKeyPress)
        {
            _isMainMenu = false;
            _waitKeyPress = waitKeyPress;
            _description = description;
            _command = command;
        }

        public IMenuItem AddSubItem(string description, Action command = null, bool waitKeyPress = true)
        {
            IMenuItem item = new MenuItem(description, command, waitKeyPress);
            _subItems.Add(item);
            return item;
        }

        public IMenuItem AddSubItem(IMenuItem menuItem)
        {
            _subItems.Add(menuItem);
            return menuItem;
        }

        public void AddSubItems(IEnumerable<IMenuItem> items)
        {
            _subItems.AddRange(items);
        }

        public void Clear()
        {
            _subItems.Clear();
        }

        public void Execute()
        {
            if (IsTreeLeaf)
            {
                ExecuteCommand();
            }
            else
            {
                RunMenu();
            }
        }

        private void RunMenu()
        {
            do
            {
                PrintMenu();
            } while (ParseUserInput());
        }

        private void PrintMenu()
        {
            Console.Clear();
            PrintHeader();

            for (int i = 0; i < _subItems.Count; ++i)
            {
                PrintEntry(i + 1, _subItems[i].Description);
            }

            Console.WriteLine();
            Console.Write(">>> ");
        }

        private void PrintHeader()
        {
            Console.WriteLine(_description);
            Console.WriteLine();
            PrintEntry(0, ReturnItemDescription);
        }

        private void PrintEntry(int ord, string description)
        {
            Console.WriteLine(FormatEntry(ord, description));
        }

        private string FormatEntry(int ord, string description)
        {
            string formatedEntry = $"{ord,3}. ";

            return description.Split('\n').Aggregate(formatedEntry, (current, data) => current + (data + "\n     "))
                .TrimEnd(new char[] {'\n', ' '});
        }

        private void ExecuteCommand()
        {
            if (_command != null)
            {
                _command.Invoke();

                if (_waitKeyPress)
                {
                    ConsoleWriter.Instance.WaitKeyPress();
                }
            }
        }

        private bool ParseUserInput()
        {
            uint commandRequest;
            if (uint.TryParse(Console.ReadLine(), out commandRequest))
            {
                if (commandRequest > 0)
                {
                    HandleCommandRequest(commandRequest);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void HandleCommandRequest(uint commandRequest)
        {
            int index = (int)commandRequest - 1;
            if (index < _subItems.Count)
            {
                _subItems[index].Execute();
            }
        }
    }
}
