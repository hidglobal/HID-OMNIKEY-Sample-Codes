using System;
using KbwToCcidSwitchApi.NativeLibraries;
using Microsoft.Win32.SafeHandles;

namespace KbwToCcidSwitchApi.Utilities
{
    internal class SafeDeviceInfoListHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeDeviceInfoListHandle(ref Guid classGuid, string enumerator, IntPtr parent, Wrapper.GetDeviceInfoSetFlags flags) : base(true)
        {
            SetHandle(Wrapper.SetupDiGetClassDevs(ref classGuid, enumerator, parent, (uint)flags));
        }

        protected override bool ReleaseHandle()
        {
            return Wrapper.SetupDiDestroyDeviceInfoList(DangerousGetHandle());
        }
    }
}
