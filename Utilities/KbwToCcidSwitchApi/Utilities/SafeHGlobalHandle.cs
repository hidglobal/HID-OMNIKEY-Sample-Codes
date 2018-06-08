using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace KbwToCcidSwitchApi.Utilities
{
    internal class SafeHGlobalHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeHGlobalHandle(int size) : base(true)
        {
            SetHandle(Marshal.AllocHGlobal(size));
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(DangerousGetHandle());
            return true;
        }
    }
}
