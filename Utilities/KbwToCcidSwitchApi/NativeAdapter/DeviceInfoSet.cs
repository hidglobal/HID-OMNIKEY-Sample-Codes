using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using KbwToCcidSwitchApi.NativeLibraries;
using KbwToCcidSwitchApi.Utilities;

namespace KbwToCcidSwitchApi.NativeAdapter
{
    internal sealed class DeviceInfoSet : IDisposable
    {
        public readonly SafeDeviceInfoListHandle Handle;
        
        public DeviceInfoSet(Guid interfaceClassGuid, Wrapper.GetDeviceInfoSetFlags flags, string enumerator = null)
        {      
            Handle = new SafeDeviceInfoListHandle(ref interfaceClassGuid, enumerator, IntPtr.Zero, flags);

            int errorCode = Marshal.GetLastWin32Error();
            if (errorCode != Wrapper.Win32ErrorSuccess)
                throw new Win32Exception(errorCode);
        }

        #region IDisposable Support
        private bool _disposedValue = false; 

        private void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                Handle.Dispose();
            } 

            _disposedValue = true;
        }

        ~DeviceInfoSet()
        {
           Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
