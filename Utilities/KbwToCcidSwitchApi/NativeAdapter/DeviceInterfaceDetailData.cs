using System;
using System.Runtime.InteropServices;
using KbwToCcidSwitchApi.Utilities;

namespace KbwToCcidSwitchApi.NativeAdapter
{
    internal class DeviceInterfaceDetailData : IDisposable
    {
        public readonly SafeHGlobalHandle Handle;

        public DeviceInterfaceDetailData(int size)
        {
            Handle = new SafeHGlobalHandle(size);
            Marshal.WriteInt32(Handle.DangerousGetHandle(), IntPtr.Size != 8 ? 4 + Marshal.SystemDefaultCharSize : 8); // Fill structure size field
        }

        public string GetPathName()
        {
            if (Handle.IsClosed || Handle.IsInvalid)
                return string.Empty;

            var pointerToPathName = new IntPtr(IntPtr.Size == 4 ? 
                Handle.DangerousGetHandle().ToInt32() + 4 : 
                Handle.DangerousGetHandle().ToInt64() + 4); // Skip structure size field (4 bytes)  
            return Marshal.PtrToStringAuto(pointerToPathName); 
        } 


        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                Handle.Dispose();
            } 

            _disposedValue = true;
        }

        ~DeviceInterfaceDetailData()
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
