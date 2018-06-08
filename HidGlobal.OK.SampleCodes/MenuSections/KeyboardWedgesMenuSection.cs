using System;
using System.Threading;
using System.Threading.Tasks;
using HidGlobal.OK.Readers;

namespace HidGlobal.OK.SampleCodes.MenuSections
{
    public class KeyboardWedgesMenuSection : IMenuSection
    {
        private readonly IMenuSectionsFactory _menuSectionsFactory;
        private readonly IMenuItem _rootMenuItem = new MenuItem("Keyboard Wedges");

        public IMenuItem RootMenuItem
        {
            get { return _rootMenuItem; }
        }

        public KeyboardWedgesMenuSection(IMenuSectionsFactory menuSectionsFactory)
        {
            _menuSectionsFactory = menuSectionsFactory ?? throw new ArgumentNullException(nameof(menuSectionsFactory));

            RefreshMenu();
        }

        private void RefreshMenu()
        {
            _rootMenuItem.Clear();
            _rootMenuItem.AddSubItem("Enable  CCID for all devices", EnableKBWCCID, false);
            _rootMenuItem.AddSubItem("Disable CCID for all devices", DisableKBWCCID, false);
            _rootMenuItem.AddSubItem("Refresh devices list", RefreshMenu, false);

            RefreshReadersList();
        }

        private void EnableKBWCCID()
        {
            RunWithIndicator("Enabling", () => KbwToCcidSwitchApi.KBWCCIDEnabler.EnableAll());
            RefreshMenu();
        }

        private void DisableKBWCCID()
        {
            RunWithIndicator("Disabling", () => KbwToCcidSwitchApi.KBWCCIDEnabler.DisableAll());
            RefreshMenu();
        }

        private void RefreshReadersList()
        {
            foreach (var reader in ContextHandler.Instance.ListReaders())
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

        private void RunWithIndicator(string description, Action action)
        {
            Task t = Task.Factory.StartNew(action);

            Console.Write(description);
            while (!t.IsCompleted)
            {
                Thread.Sleep(200);
                Console.Write(".");
            }
        }
    }
}
