using System;
using System.Runtime.InteropServices;
using KbwToCcidSwitchApi.Utilities;

namespace KbwToCcidSwitchApi.NativeAdapter
{
    internal class DeviceIdBuffer : IDisposable
    {
        public readonly SafeHGlobalHandle Handle;

        public DeviceIdBuffer(int size)
        {
            Handle = new SafeHGlobalHandle(size);
        }

        public string GetInstanceIdString()
        {
            if (Handle.IsClosed || Handle.IsInvalid)
                return string.Empty;

            return Marshal.PtrToStringAuto(Handle.DangerousGetHandle());
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

        ~DeviceIdBuffer()
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
