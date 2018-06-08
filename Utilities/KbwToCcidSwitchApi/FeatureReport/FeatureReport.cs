using System;
using KbwToCcidSwitchApi.NativeAdapter;
using KbwToCcidSwitchApi.NativeLibraries;
using Microsoft.Win32.SafeHandles;

namespace KbwToCcidSwitchApi.FeatureReport
{
    public class FeatureReport : IFeatureReport
    {
        public static FeatureReport EnableCcidInterface = new FeatureReport(new byte[] {0x00, 0xA5, 0x5A},
            "Feature Report to enable CCID interface in OMNIKEY 5027 KeyboardWedge device");

        public static FeatureReport DisableCcidInterface = new FeatureReport(new byte[] {0x00, 0x5A, 0xA5},
            "Feature Report to disable CCID interface in OMNIKEY 5027 KeyboardWedge device");
        public byte[] Report { get; }
        public byte ReportId => Report[0];
        public string Description { get; }
        public FeatureReport(byte[] buffer, string description = "")
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            Report = new byte[buffer.Length];
            Array.Copy(buffer, Report, Report.Length);

            Description = description;
        }
        public void Send(KeyboardData keyboardData)
        {
            if (Report == null)
                throw new ArgumentNullException(nameof(Report));

            using (SafeFileHandle deviceFileName = GetSafeFileHandle(keyboardData.DeviceFileInstancePath))
            {
                Adapter.SetFeature(deviceFileName, Report);
            }
        }
        private SafeFileHandle GetSafeFileHandle(string deviceInstanceFileName)
        {
            return Adapter.GetDeviceFileHandle(deviceInstanceFileName, Wrapper.DesiredFileAccess.QueryWithoutAccess,
                Wrapper.ShareModes.FileShareRead, Wrapper.CreationDisposition.OpenExisting);
        }
    }
}
