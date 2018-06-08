/*****************************************************************************************
    (c) 2017-2018 HID Global Corporation/ASSA ABLOY AB.  All rights reserved.

      Redistribution and use in source and binary forms, with or without modification,
      are permitted provided that the following conditions are met:
         - Redistributions of source code must retain the above copyright notice,
           this list of conditions and the following disclaimer.
         - Redistributions in binary form must reproduce the above copyright notice,
           this list of conditions and the following disclaimer in the documentation
           and/or other materials provided with the distribution.
           THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
           AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
           THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
           ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
           FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
           (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
           LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
           ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
           (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
           THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*****************************************************************************************/
using System;

namespace HidGlobal.OK.Readers.Components
{
    enum DeviceType
    {
        FileDeviceSmartCard = 0x31
    };

    public enum ReaderControlCode : int
    {
        /// <summary>
        /// SCARD_CTL_CODE(1)
        /// </summary>
        IOCTL_SMARTCARD_POWER = (DeviceType.FileDeviceSmartCard << 16) + (1 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(2)
        /// </summary>
        IOCTL_SMARTCARD_GET_ATTRIBUTE = (DeviceType.FileDeviceSmartCard << 16) + (2 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3)
        /// </summary>
        IOCTL_SMARTCARD_SET_ATTRIBUTE = (DeviceType.FileDeviceSmartCard << 16) + (3 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(4)
        /// </summary>
        IOCTL_SMARTCARD_CONFISCATE = (DeviceType.FileDeviceSmartCard << 16) + (4 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(5)
        /// </summary>
        IOCTL_SMARTCARD_TRANSMIT = (DeviceType.FileDeviceSmartCard << 16) + (5 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(6)
        /// </summary>
        IOCTL_SMARTCARD_EJECT = (DeviceType.FileDeviceSmartCard << 16) + (6 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(7)
        /// </summary>
        IOCTL_SMARTCARD_SWALLOW = (DeviceType.FileDeviceSmartCard << 16) + (7 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(8)
        /// </summary>
        [Obsolete]
        IOCTL_SMARTCARD_READ = (DeviceType.FileDeviceSmartCard << 16) + (8 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(9)
        /// </summary>
        [Obsolete]
        IOCTL_SMARTCARD_WRITE = (DeviceType.FileDeviceSmartCard << 16) + (9 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(10)
        /// </summary>
        IOCTL_SMARTCARD_IS_PRESENT = (DeviceType.FileDeviceSmartCard << 16) + (10 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(11)
        /// </summary>
        IOCTL_SMARTCARD_IS_ABSENT = (DeviceType.FileDeviceSmartCard << 16) + (11 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(12)
        /// </summary>
        IOCTL_SMARTCARD_SET_PROTOCOL = (DeviceType.FileDeviceSmartCard << 16) + (12 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(14)
        /// </summary>
        IOCTL_SMARTCARD_GET_STATE = (DeviceType.FileDeviceSmartCard << 16) + (14 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(15)
        /// </summary>
        IOCTL_SMARTCARD_GET_LAST_ERROR = (DeviceType.FileDeviceSmartCard << 16) + (15 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(16)
        /// </summary>
        IOCTL_SMARTCARD_GET_PERF_CNTR = (DeviceType.FileDeviceSmartCard << 16) + (16 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3001)
        /// </summary>
        CM_IOCTL_GET_FW_VERSION = (DeviceType.FileDeviceSmartCard << 16) + (3001 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3058)
        /// Set LED and buzzer for 5x21 Pay
        /// </summary>
        CM_IOCTL_SIGNAL = (DeviceType.FileDeviceSmartCard << 16) + (3058 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3105)
        /// SyncAPI and set antenna properties
        /// </summary>
        CM_IOCTL_RFID_GENERIC = (DeviceType.FileDeviceSmartCard << 16) + (3105 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3107)
        /// Set RFID Operation Mode PayPass / ISO
        /// </summary>
        CM_IOCTL_SET_OPERATION_MODE = (DeviceType.FileDeviceSmartCard << 16) + (3107 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3208)
        /// Get information about 14443 baud rate
        /// </summary>
        CM_IOCTL_GET_MAXIMUM_RFID_BAUDRATE = (DeviceType.FileDeviceSmartCard << 16) + (3208 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3213)
        /// Set the RFID Control Flags dynamic
        /// </summary>
        CM_IOCTL_SET_RFID_CONTROL_FLAGS = (DeviceType.FileDeviceSmartCard << 16) + (3213 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3215)
        /// Get/Set asymmetric ISO14443 baud rate
        /// </summary>
        CM_IOCTL_GET_SET_RFID_BAUDRATE = (DeviceType.FileDeviceSmartCard << 16) + (3215 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3400)
        /// </summary>
        CM_IOCTL_GET_FEATURE_REQUEST = (DeviceType.FileDeviceSmartCard << 16) + (3400 << 2),

        /// <summary>
        /// SCARD_CTL_CODE(3500)
        /// </summary>
        IOCTL_CCID_ESCAPE = (DeviceType.FileDeviceSmartCard << 16) + (3500 << 2)
    }
}
