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
using HidGlobal.OK.Readers;

namespace HidGlobal.OK.SampleCodes.MenuSections
{
    public class SmartCardReadersMenuSection : IMenuSection
    {
        private readonly IMenuSectionsFactory _menuSectionsFactory;
        private readonly IMenuItem _rootMenuItem = new MenuItem("Smart Card Readers");

        public IMenuItem RootMenuItem
        {
            get { return _rootMenuItem; }
        }

        public SmartCardReadersMenuSection(IMenuSectionsFactory menuSectionsFactory)
        {
            _menuSectionsFactory = menuSectionsFactory ?? throw new ArgumentNullException(nameof(menuSectionsFactory));

            RefreshMenu();
        }

        private void RefreshMenu()
        {
            _rootMenuItem.Clear();
            _rootMenuItem.AddSubItem("Refresh devices list", RefreshMenu, false);

            RefreshReadsList();
        }

        private void RefreshReadsList()
        {
            foreach(var reader in ContextHandler.Instance.ListReaders())
            {
                AddReaderSection(reader);
            }
        }

        private void AddReaderSection(string readerName)
        {
            IMenuSection section = _menuSectionsFactory.CreateSection(readerName);
            if (section != null)
            {
                _rootMenuItem.AddSubItem(section.RootMenuItem);
            }
        }
    }
}
