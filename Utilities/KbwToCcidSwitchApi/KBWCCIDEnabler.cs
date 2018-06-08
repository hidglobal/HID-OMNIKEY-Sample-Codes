using System.Collections.Generic;
using System.Linq;
using KbwToCcidSwitchApi.Discovery;
using KbwToCcidSwitchApi.FeatureReport;

namespace KbwToCcidSwitchApi
{       
    public static class KBWCCIDEnabler
    {
        private const ushort HIDGlobalVid = 0x076b;
        private const ushort OK5027PidWhenCCIDDisabled = 0x5027;
        private const ushort OK5027PidWhenCCIDEnabled = 0x5A27;

        private static readonly IFeatureReport EnableReport = FeatureReport.FeatureReport.EnableCcidInterface;
        private static readonly IFeatureReport DisableReport = FeatureReport.FeatureReport.DisableCcidInterface;

        /// <summary>
        /// Enables CCID for all devices that appear only in the form of a keyboard 
        /// Device in that state exposes path that doesn't contain interface section (mi_xx)
        /// for example HID\VID_076B&PID_5027\8&1C6CF67F&0&0000
        /// </summary>
        public static void EnableAll(bool blockTillSwitched = true)
        {
            var disabledDevices = GetDisabledDevices().ToList();
            foreach (var device in disabledDevices)
            {
                EnableReport.Send(device);
            }

            if (!blockTillSwitched) return;
            do
            {
                System.Threading.Thread.Sleep(100);
            } while (disabledDevices.Count > 0 && GetEnabledDevices().Count() != disabledDevices.Count);
        }

        /// <summary>
        /// Disables CCID for all devices that appear in dual form of a keyboard and a reader at the same time
        /// Device in that state exposes path that contains interface section (mi_xx)
        /// for example HID\VID_076B&PID_5027&MI_00\8&1C6CF67F&0&0000
        /// </summary>]
        public static void DisableAll(bool blockTillSwitched = true)
        {
            var enabledDevices = GetEnabledDevices().ToList();
            foreach (var device in enabledDevices)
            {
                DisableReport.Send(device);
            }

            if (!blockTillSwitched) return;
            do
            {
                System.Threading.Thread.Sleep(100);
            } while (enabledDevices.Count > 0 && GetDisabledDevices().Count() != enabledDevices.Count);
        }

        private static IEnumerable<KeyboardData> GetEnabledDevices()
        {
            return KeyboardDiscovery.Instance.ListKeyboards(HIDGlobalVid, OK5027PidWhenCCIDEnabled);
        }

        private static IEnumerable<KeyboardData> GetDisabledDevices()
        {
            return KeyboardDiscovery.Instance.ListKeyboards(HIDGlobalVid, OK5027PidWhenCCIDDisabled);
        }
    }
}
