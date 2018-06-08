using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KbwToCcidSwitchApi.NativeAdapter;

namespace KbwToCcidSwitchApi.Discovery
{
    public class KeyboardDiscovery : IKeyboardDiscovery
    {
        private static IKeyboardDiscovery _instance;
        public static IKeyboardDiscovery Instance => _instance ?? (_instance = new KeyboardDiscovery());
        private KeyboardDiscovery() { } 
        public IEnumerable<KeyboardData> ListKeyboards()
        {
            return Adapter.EnumerateKeyboards();
        }
        public IEnumerable<KeyboardData> ListKeyboards(ushort vendorId)
        {
            var regexPattern = new Regex(@"w*vid_" + vendorId.ToString("x4") + @"&w*", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

            foreach (var keyboard in ListKeyboards())
            {
                if (regexPattern.IsMatch(keyboard.DeviceFileInstancePath))
                    yield return keyboard;
            }
        }
        public IEnumerable<KeyboardData> ListKeyboards(ushort vendorId, ushort productId)
        {
            var regexPattern = new Regex(@"w*pid_" + productId.ToString("x4") + @"w*", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

            foreach (var keyboard in ListKeyboards(vendorId))
            {
                if (regexPattern.IsMatch(keyboard.DeviceFileInstancePath))
                    yield return keyboard;
            }
        }
    }
}
