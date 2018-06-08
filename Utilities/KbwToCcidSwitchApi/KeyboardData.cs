using System.Collections;
using System.Collections.Generic;

namespace KbwToCcidSwitchApi
{
    public struct KeyboardData : IEnumerable<string>
    {
        public string DeviceInstancePath;
        public string SerialNumber;
        public string ParentDeviceInstancePath;
        public string GrandParentDeviceInstancePath;
        public string DeviceFileInstancePath;
        public IEnumerator<string> GetEnumerator()
        {
            if (!string.IsNullOrWhiteSpace(DeviceInstancePath))
                yield return DeviceInstancePath;

            if (!string.IsNullOrWhiteSpace(SerialNumber))
                yield return SerialNumber;

            if (!string.IsNullOrWhiteSpace(ParentDeviceInstancePath))
                yield return ParentDeviceInstancePath;

            if (!string.IsNullOrWhiteSpace(GrandParentDeviceInstancePath))
                yield return GrandParentDeviceInstancePath;

            if (!string.IsNullOrWhiteSpace(DeviceFileInstancePath))
                yield return DeviceFileInstancePath;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return GetEnumerator();
        }
    }
}
