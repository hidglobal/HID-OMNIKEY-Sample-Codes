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

namespace HidGlobal.OK.SampleCodes.Utilities
{
    /// <summary>
    /// Class representing single menu entry.
    /// </summary>
    public class MenuEntry
    {
        /// <summary>
        /// Entry Description.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Hold information about the type of the entry.
        /// </summary>
        Type entryType;
        public Type EntryType
        {
            get { return entryType; }
            private set { entryType = value; }
        }
        /// <summary>
        /// Action to be executed on entry selection.
        /// </summary>
        Action EntryAction;
        /// <summary>
        /// Action to be executed on exit.
        /// </summary>
        Action ExitAction;
        /// <summary>
        /// Submenu stored in menu entry.
        /// </summary>
        Menu SubMenu;
        /// <summary>
        /// Creates the menu entry that holds action to be executed when selected.
        /// </summary>
        /// <param name="entryDescription">Description of menu entry.</param>
        /// <param name="entryAction">Action delegate for enclosed methode.</param>
        public MenuEntry(string entryDescription, Action entryAction, Action exitAction = null )
        {
            Description = entryDescription;
            EntryAction = entryAction;
            EntryType = typeof(Action);
            if (exitAction != null)
                ExitAction = exitAction;
        }
        /// <summary>
        /// Creates the menu entry that submenu.
        /// </summary>
        /// <param name="entryDescription">Description of menu entry.</param>
        /// <param name="subMenu">Submenu to be enclosed in the entry.</param>
        public MenuEntry(string entryDescription, Menu subMenu)
        {
            Description = entryDescription;
            SubMenu = subMenu;
            EntryType = typeof(Menu);
        }
        /// <summary>
        /// Executes entry.
        /// </summary>
        /// <returns></returns>
        public bool ExecuteEntry()
        {
            try
            {
                if (entryType.Equals(typeof(Action)))
                {
                    EntryAction.Invoke();
                    if (ExitAction != null)
                        ExitAction.Invoke();
                }
                else
                    SubMenu.GetType().GetMethod("RunMenu").Invoke(SubMenu, null);
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }
        }

    }
}
