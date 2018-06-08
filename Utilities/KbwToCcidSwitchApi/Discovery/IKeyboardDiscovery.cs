using System.Collections.Generic;

namespace KbwToCcidSwitchApi.Discovery
{
    public interface IKeyboardDiscovery
    {
        IEnumerable<KeyboardData> ListKeyboards();
        IEnumerable<KeyboardData> ListKeyboards(ushort vendorId);
        IEnumerable<KeyboardData> ListKeyboards(ushort vendorId, ushort productId);
    }
}
