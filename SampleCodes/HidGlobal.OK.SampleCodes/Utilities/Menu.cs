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
using System.Threading;

namespace HidGlobal.OK.SampleCodes.Utilities
{
    /// <summary>
    /// Class representing Menu interface.
    /// </summary>
    public class Menu : List<MenuEntry>
    {
        /// <summary>
        /// Menu description.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Indicates if RunMenu methode should quit.
        /// </summary>
        bool stopMenu;

        /// <summary>
        /// Returns the Menu class object.
        /// </summary>
        /// <param name="menuDescription">Description of the menu.</param>
        /// <param name="isSubMenu"></param>
        /// <param name="entries">Menu menu entries of type MenuEntry.</param>
        public Menu(string menuDescription, bool isSubMenu, params MenuEntry[] entries)
        {
            string menuEntryDescription;
            stopMenu = false;
            Description = menuDescription + "\n";
            if (isSubMenu)
                menuEntryDescription = "Back";
            else
                menuEntryDescription = "Quit";

            this.Add(new MenuEntry(menuEntryDescription, new Action(() => { this.stopMenu = true; })));
            this.AddRange(entries);
        }
        /// <summary>
        /// Prints menu on the console screen.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="suffix"></param>
        void PrintMenu(string separator = ".", string suffix = "\n")
        {
            int number = 0;
            string menu = "";
            foreach (var entry in this)
            {
                menu += string.Format("{0,3}{1,-2}{2}{3}", number++, separator, entry.Description, suffix);
            }
            Console.Clear();
            Console.WriteLine(Description);
            Console.WriteLine(menu);
        }
        /// <summary>
        /// Runs the menu.
        /// </summary>
        public void RunMenu()
        {
            int selectedEntry;
            do
            {
                selectedEntry = -1;
                PrintMenu();
                Console.Write(">>> ");
                if (int.TryParse(Console.ReadLine(), out selectedEntry))
                    ExecuteEntry(selectedEntry);
                Thread.Sleep(200);
            } while (stopMenu != true);
        }
        /// <summary>
        /// Executes the methode encapsulated by menu entry.
        /// </summary>
        /// <param name="selectedEntry"></param>
        /// <returns></returns>
        public bool ExecuteEntry(int selectedEntry)
        {
            if (selectedEntry >= this.Count)
                return false;
            this[selectedEntry].ExecuteEntry();
            return true;
        }

    }
}
