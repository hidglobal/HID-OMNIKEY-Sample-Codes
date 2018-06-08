using System;
using System.Runtime.InteropServices;

namespace KbwToCcidSwitchApi.NativeLibraries
{
    internal static class Wrapper
    {
        [Flags]
        internal enum GetDeviceInfoSetFlags : uint
        {
            /// <summary>
            /// Return only the device that is associated with the system default device interface, if one is set, for the specified device interface classes.
            /// </summary>
            Default = 0x01,
            /// <summary>
            /// Return only devices that are currently present in a system.
            /// </summary>
            Present = 0x02,
            /// <summary>
            /// Return a list of installed devices for all device setup classes or all device interface classes.
            /// </summary>
            AllClasses = 0x04,
            /// <summary>
            /// Return only devices that are a part of the current hardware profile.
            /// </summary>
            Profile = 0x08,
            /// <summary>
            /// Return devices that support device interfaces for the specified device interface classes.
            /// This flag must be set in the Flags parameter if the Enumerator parameter specifies a device instance ID.
            /// </summary>
            DeviceInterface = 0x10
        };

        [Flags]
        internal enum DesiredFileAccess
        {
            QueryWithoutAccess = 0,
            GenericAll = 1 << 28,
            GenericExecute = 1 << 29,
            GenericWrite = 1 << 30,
            GenericRead = 1 << 31,
        }

        [Flags]
        internal enum ShareModes
        {
            ExclusiveAccess = 0,
            FileShareRead = 1,
            FileShareWrite = 2,
            FileShareDelete = 4,
        }

        internal enum CreationDisposition
        {
            CreateNew = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5,
        }

        public static int Win32ErrorSuccess => 0x00000000;
        public static int Win32ErrorInsufficientBuffer => 0x0000007A;
        public static int Win32ErrorNoMoreItems => 0x00000103;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SpDevInfoData
        {
            public int Size;
            public Guid ClassGuid;
            public IntPtr Reserved;
            public uint DeviceInstance;

            public SpDevInfoData(bool autoSize = true)
            {
                Size = default(int);
                ClassGuid = default(Guid);
                Reserved = default(IntPtr);
                DeviceInstance = default(uint);

                if (!autoSize)
                    return;

                Size = Marshal.SizeOf(typeof(SpDevInfoData));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SpDeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr Reserved;

            public SpDeviceInterfaceData(bool autoSize = true)
            {
                Size = default(int);
                Flags = default(uint);
                Reserved = default(IntPtr);
                InterfaceClassGuid = default(Guid);

                if (!autoSize)
                    return;

                Size = Marshal.SizeOf(typeof(SpDeviceInterfaceData));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DevPropKey
        {
            public Guid fmtid;
            public uint pid;

            // from devpkey.h
            public static readonly DevPropKey DEVPKEY_Device_InstanceId = new DevPropKey
            {
                fmtid = new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57),
                pid = 256
            };
            public static readonly DevPropKey DEVPKEY_Device_BusRelations = new DevPropKey
            {
                fmtid = new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7),
                pid = 7
            };
            public static readonly DevPropKey DEVPKEY_Device_Parent = new DevPropKey
            {
                fmtid = new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7),
                pid = 8
            };
            public static readonly DevPropKey DEVPKEY_Device_Children = new DevPropKey
            {
                fmtid = new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7),
                pid = 9
            };
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile([In] string fileName, 
                                               [In] int desiredAccess, 
                                               [In] int shareMode, 
                                               [In, Optional] IntPtr securityAttributes, 
                                               [In] int creationDisposition, 
                                               [In] int flagsAndAttributes, 
                                               [In, Optional] int templateFile);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_SetFeature([In] IntPtr deviceFileHandle,
                                                   [In] byte[] reportBuffer,
                                                   [In] int reportBufferLength);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs([In] ref Guid classGuid,
                                                        [In, Optional] string enumerator,
                                                        [In, Optional] IntPtr parent,
                                                        [In] uint flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList([In] IntPtr deviceInfoSet);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces([In] IntPtr deviceInfoSet,
                                                               [In, Optional] IntPtr deviceInfoData,
                                                               [In, Out] ref Guid interfaceClassGuid,
                                                               [In] int memberIndex,
                                                               [In, Out] ref SpDeviceInterfaceData deviceInterfaceData);
        

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail([In] IntPtr deviceInfoSet,
                                                                   [In, Out] ref SpDeviceInterfaceData deviceInterfaceData,
                                                                   [In, Out] IntPtr deviceInterfaceDetailData,
                                                                   [In] int deviceInterfaceDetailDataSize,
                                                                   [In, Out] ref int requiredSize,
                                                                   [In] IntPtr deviceInfoData);


        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo([In] IntPtr deviceInfoSet,
                                                        [In] int memberIndex,
                                                        [In, Out] ref SpDevInfoData deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "SetupDiGetDevicePropertyW")]
        public static extern bool SetupDiGetDeviceProperty([In] IntPtr deviceInfoSet, 
                                                           [In, Out] ref SpDevInfoData deviceInfoData, 
                                                           [In, Out] ref DevPropKey propertyKey, 
                                                           [Out] out int propertyType,
                                                           [In, Out] IntPtr propertyBuffer,
                                                           [In] int propertyBufferSize,
                                                           [Out] out int requiredSize, 
                                                           [In, Optional] int flags);
    }
}
