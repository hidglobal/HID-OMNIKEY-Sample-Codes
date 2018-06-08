using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32.SafeHandles;
using KbwToCcidSwitchApi.NativeLibraries;

namespace KbwToCcidSwitchApi.NativeAdapter
{
    internal static class Adapter
    {
        private static string GuidInterfaceKeyboard => "{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";
        private static string GuidInterfaceUsbDevice => "{A5DCBF10-6530-11D2-901F-00C04FB951ED}";
        public static SafeFileHandle GetDeviceFileHandle(string devicePathName, Wrapper.DesiredFileAccess accessFlags, Wrapper.ShareModes shareMode, Wrapper.CreationDisposition creationDisposition)
        {
            IntPtr handle = Wrapper.CreateFile(devicePathName, (int)accessFlags, (int)shareMode, default(IntPtr), (int)creationDisposition, default(int));
            int errorCode = Marshal.GetLastWin32Error();

            if (errorCode != Wrapper.Win32ErrorSuccess)
                throw new Win32Exception(errorCode);

            return new SafeFileHandle(handle, true);
        }
        public static void SetFeature(SafeFileHandle hidDeviceFileHandle, byte[] featureReportBuffer)
        {
            if (hidDeviceFileHandle == null)
                throw new ArgumentNullException(nameof(hidDeviceFileHandle));

            if (featureReportBuffer == null)
                throw new ArgumentNullException(nameof(featureReportBuffer));

            if (featureReportBuffer.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(featureReportBuffer), "Feature report buffer length must be greater then 0.");

            if (hidDeviceFileHandle.IsInvalid)
                throw new ArgumentException("Handle invalid.", nameof(hidDeviceFileHandle));

            Wrapper.HidD_SetFeature(hidDeviceFileHandle.DangerousGetHandle(), featureReportBuffer, featureReportBuffer.Length);
            int errorCode = Marshal.GetLastWin32Error();

            if (errorCode != Wrapper.Win32ErrorSuccess)
                throw new Win32Exception(errorCode);
        }
        public static IEnumerable<KeyboardData> EnumerateKeyboards()
        {
            foreach (var keyboardBasicData in GetKeyboardBasicData())
            {
                var serialNumber = TryParseSerialNumber(keyboardBasicData.Item2);

                yield return new KeyboardData
                {
                    DeviceFileInstancePath = keyboardBasicData.Item1,
                    DeviceInstancePath = keyboardBasicData.Item2.DeviceId,
                    ParentDeviceInstancePath = keyboardBasicData.Item2.ParentDeviceId,
                    GrandParentDeviceInstancePath = keyboardBasicData.Item2.GrandParentDeviceId,
                    SerialNumber = serialNumber
                };
            }
        }
        private static string TryParseSerialNumber(DeviceHeritage deviceHeritage)
        {
            var result = string.Empty;
            var regexPattern = new Regex(@".*vid_[a-fA-F0-9]{4}&pid_[a-fA-F0-9]{4}.{0,6}\\(\w{4,})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

            foreach (var id in deviceHeritage)
            {
                var regexMatch = regexPattern.Match(id);
                if (!regexMatch.Success)
                    continue;

                result = regexMatch.Groups[1].Value; // first regex contains whole id so we pick the second one.
                break;
            }
            return result;
        }
        private static IEnumerable<Tuple<string, DeviceHeritage>> GetKeyboardBasicData()
        {
            var interfaceGuid = new Guid(GuidInterfaceKeyboard);

            using (var deviceInfoSet = new DeviceInfoSet(interfaceGuid, Wrapper.GetDeviceInfoSetFlags.Present | Wrapper.GetDeviceInfoSetFlags.DeviceInterface))
            {
                foreach (var interfaceData in EnumerateDeviceInterfaces(deviceInfoSet, interfaceGuid))
                {
                    var deviceFileInstancePath = GetDeviceFileInstancePath(deviceInfoSet, interfaceData.Item2);

                    var deviceData = GetDeviceInfoData(deviceInfoSet, interfaceData.Item1);

                    var deviceInstancePath = deviceData.Item1
                        ? GetPropertyString(deviceInfoSet, deviceData.Item2, Wrapper.DevPropKey.DEVPKEY_Device_InstanceId)
                        : string.Empty;

                    var parentInstancePath = deviceData.Item1
                        ? GetPropertyString(deviceInfoSet, deviceData.Item2, Wrapper.DevPropKey.DEVPKEY_Device_Parent)
                        : string.Empty;

                    var grandParentInstancePath = deviceData.Item1
                        ? GetGrandParent(parentInstancePath)
                        : string.Empty;

                    var deviceHeritage = new DeviceHeritage
                    {
                        DeviceId = deviceInstancePath,
                        ParentDeviceId = parentInstancePath,
                        GrandParentDeviceId = grandParentInstancePath
                    };

                    yield return new Tuple<string, DeviceHeritage>(deviceFileInstancePath, deviceHeritage);
                }
            }
        }
        private static IEnumerable<Tuple<int, Wrapper.SpDeviceInterfaceData>> EnumerateDeviceInterfaces(DeviceInfoSet deviceInfoSet, Guid deviceGuid)
        {
            bool isEnumerationCompleted;
            var memberIndex = -1;
            do
            {
                var deviceInformationData = new Wrapper.SpDeviceInterfaceData(true);

                memberIndex++;

                bool deviceAvailable = Wrapper.SetupDiEnumDeviceInterfaces(deviceInfoSet.Handle.DangerousGetHandle(), IntPtr.Zero, ref deviceGuid, memberIndex, ref deviceInformationData);
                isEnumerationCompleted = Marshal.GetLastWin32Error().Equals(Wrapper.Win32ErrorNoMoreItems);

                if (!deviceAvailable)
                    continue;

                yield return new Tuple<int, Wrapper.SpDeviceInterfaceData>(memberIndex, deviceInformationData);

            } while (!isEnumerationCompleted);
        }
        private static IEnumerable<Tuple<int, Wrapper.SpDevInfoData>> EnumerateDeviceInfoData(DeviceInfoSet deviceInfoSet)
        {
            bool isEnumerationCompleted;
            var memberIndex = -1;
            do
            {
                var deviceInfoData = new Wrapper.SpDevInfoData(true);

                memberIndex++;

                bool infoAvailable = Wrapper.SetupDiEnumDeviceInfo(deviceInfoSet.Handle.DangerousGetHandle(), memberIndex, ref deviceInfoData);
                isEnumerationCompleted = Marshal.GetLastWin32Error().Equals(Wrapper.Win32ErrorNoMoreItems);

                if (!infoAvailable)
                    continue;

                yield return new Tuple<int, Wrapper.SpDevInfoData>(memberIndex, deviceInfoData);

            } while (!isEnumerationCompleted);
        }
        private static Tuple<bool, Wrapper.SpDevInfoData> GetDeviceInfoData(DeviceInfoSet deviceInfoSet, int memberIndex)
        {
            var deviceInfoData = new Wrapper.SpDevInfoData(true);
            bool infoAvailable = Wrapper.SetupDiEnumDeviceInfo(deviceInfoSet.Handle.DangerousGetHandle(), memberIndex, ref deviceInfoData);
            return new Tuple<bool, Wrapper.SpDevInfoData>(infoAvailable, deviceInfoData);
        }
        private static string GetGrandParent(string parentDeviceInstancePath)
        {
            string grandParent = string.Empty;
            var guid = new Guid(GuidInterfaceUsbDevice);

            using (var deviceInfoSet = new DeviceInfoSet(guid, Wrapper.GetDeviceInfoSetFlags.DeviceInterface | Wrapper.GetDeviceInfoSetFlags.Present))
            {
                foreach (var usbDevice in EnumerateDeviceInfoData(deviceInfoSet))
                {
                    var childrenDevices = GetPropertyArray(deviceInfoSet, usbDevice.Item2, Wrapper.DevPropKey.DEVPKEY_Device_Children);

                    if (!childrenDevices.Contains(parentDeviceInstancePath))
                        continue;

                    grandParent = GetPropertyString(deviceInfoSet, usbDevice.Item2,Wrapper.DevPropKey.DEVPKEY_Device_InstanceId);
                    break;
                }
            }
            return grandParent;
        }
        private static string GetDeviceFileInstancePath(DeviceInfoSet deviceInfo, Wrapper.SpDeviceInterfaceData deviceInterfaceData)
        {
            int bufferSize = GetInterfaceDetailBufferSize(deviceInfo, deviceInterfaceData);
            using (var interfaceDetailBuffer = new DeviceInterfaceDetailData(bufferSize))
            {
                bool isSuccessful = Wrapper.SetupDiGetDeviceInterfaceDetail(deviceInfo.Handle.DangerousGetHandle(),
                                                                            ref deviceInterfaceData, 
                                                                            interfaceDetailBuffer.Handle.DangerousGetHandle(), 
                                                                            bufferSize, 
                                                                            ref bufferSize, 
                                                                            default(IntPtr));
                var errorCode = Marshal.GetLastWin32Error();
                if (!isSuccessful)
                    throw new Win32Exception(errorCode);

                return interfaceDetailBuffer.GetPathName();
            }
        }
        private static int GetInterfaceDetailBufferSize(DeviceInfoSet deviceInfo, Wrapper.SpDeviceInterfaceData deviceInterfaceData)
        {
            int bufferSize = 0;
            bool isSuccessful = Wrapper.SetupDiGetDeviceInterfaceDetail(deviceInfo.Handle.DangerousGetHandle(), ref deviceInterfaceData, default(IntPtr), 0, ref bufferSize, default(IntPtr));
            var errorCode = Marshal.GetLastWin32Error();
            if (!isSuccessful && errorCode != Wrapper.Win32ErrorInsufficientBuffer)
                throw new Win32Exception(errorCode);

            return bufferSize;
        }
        private static int GetPropertyBufferSize(DeviceInfoSet deviceInfoSet, Wrapper.SpDevInfoData deviceInfoData, Wrapper.DevPropKey key)
        {
            var parentInstanceIdProperty = key;

            bool isSuccessful = Wrapper.SetupDiGetDeviceProperty(deviceInfoSet.Handle.DangerousGetHandle(),
                ref deviceInfoData,
                ref parentInstanceIdProperty,
                out int _,
                IntPtr.Zero,
                0,
                out int size);
            int errorCode = Marshal.GetLastWin32Error();
            if (!isSuccessful && errorCode != Wrapper.Win32ErrorInsufficientBuffer)
                throw new Win32Exception(errorCode);

            return size;
        }
        private static string GetPropertyString(DeviceInfoSet deviceInfoSet, Wrapper.SpDevInfoData deviceInfoData, Wrapper.DevPropKey key)
        {
            var propertySize = GetPropertyBufferSize(deviceInfoSet, deviceInfoData, key);
            var devPropKey = key;

            using (var buffer = new DeviceIdBuffer(propertySize))
            {
                bool isSuccessful = Wrapper.SetupDiGetDeviceProperty(deviceInfoSet.Handle.DangerousGetHandle(),
                                                                     ref deviceInfoData,
                                                                     ref devPropKey,
                                                                     out int _,
                                                                     buffer.Handle.DangerousGetHandle(),
                                                                     propertySize,
                                                                     out int _);
                int errorCode = Marshal.GetLastWin32Error();
                if (!isSuccessful)
                    throw new Win32Exception(errorCode);

                return Marshal.PtrToStringAuto(buffer.Handle.DangerousGetHandle());
            }
        }
        private static IEnumerable<string> GetPropertyArray(DeviceInfoSet deviceInfoSet, Wrapper.SpDevInfoData deviceInfoData, Wrapper.DevPropKey key)
        {
            var result = new string[0];
            try
            {
                var propertySize = GetPropertyBufferSize(deviceInfoSet, deviceInfoData, key);
                var devPropKey = key;

                using (var buffer = new DeviceIdBuffer(propertySize))
                {
                    bool isSuccessful = Wrapper.SetupDiGetDeviceProperty(deviceInfoSet.Handle.DangerousGetHandle(),
                        ref deviceInfoData,
                        ref devPropKey,
                        out int _,
                        buffer.Handle.DangerousGetHandle(),
                        propertySize,
                        out int _);
                    int errorCode = Marshal.GetLastWin32Error();
                    if (!isSuccessful)
                        throw new Win32Exception(errorCode);

                    var data = new byte[propertySize];
                    Marshal.Copy(buffer.Handle.DangerousGetHandle(), data, 0, propertySize);

                    result = System.Text.Encoding.Unicode.GetString(data).TrimEnd('\0').Split('\0');
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
            return result;
        }
    }
}
