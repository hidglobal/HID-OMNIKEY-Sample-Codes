using System.Collections;
using System.Collections.Generic;

namespace KbwToCcidSwitchApi.NativeAdapter
{
    internal struct DeviceHeritage : IEnumerable<string>
    {
        public string DeviceId { get; set; }
        public string ParentDeviceId { get; set; }
        public string GrandParentDeviceId { get; set; }

        public IEnumerator<string> GetEnumerator()
        {
            if (string.IsNullOrWhiteSpace(DeviceId))
                yield break;

            yield return DeviceId;

            if (string.IsNullOrWhiteSpace(ParentDeviceId))
                yield break;

            yield return ParentDeviceId;

            if (string.IsNullOrWhiteSpace(GrandParentDeviceId))
                yield break;
            yield return GrandParentDeviceId;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return GetEnumerator();
        }
    }
}
