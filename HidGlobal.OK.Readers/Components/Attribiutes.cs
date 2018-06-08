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
namespace HidGlobal.OK.Readers.Components
{

    public enum Class
    {
        /// <summary>
        /// Vendor information definitions
        /// </summary>
        VendorInfo                   = 1,

        /// <summary>
        /// Communication definitions
        /// </summary>
        Communication                = 2,

        /// <summary>
        /// Protocol definitions
        /// </summary>
        Protocol                     = 3,

        /// <summary>
        /// Power Management definitions
        /// </summary>
        PowerManagement              = 4,

        /// <summary>
        /// Security Assurance definitions
        /// </summary>
        Security                     = 5,

        /// <summary>
        /// Mechanical characteristic definitions
        /// </summary>
        Mechanical                   = 6,

        /// <summary>
        /// Vendor specific definitions
        /// </summary>
        VendorDefined                = 7,

        /// <summary>
        /// Interface Device Protocol options
        /// </summary>
        InterfaceDeviceProtocol      = 8,

        /// <summary>
        /// ICC State specific definitions
        /// </summary>
        ICCState                     = 9,

        /// <summary>
        /// System-specific definitions
        /// </summary>
        System                       = 0x7fff
    };

    public enum Attribiutes
    {
        /// <summary>
        /// Vendor name. (SCARD_ATTR_VENDOR_NAME)
        /// </summary>
        VendorName                              = (Class.VendorInfo << 16) | 0x0100,
        /// <summary>
        /// Vendor-supplied interface device type (model designation of reader). (SCARD_ATTR_VENDOR_IFD_TYPE)
        /// </summary>
        VendorInterfaceDeviceType               = (Class.VendorInfo << 16) | 0x0101,
        /// <summary>
        /// Vendor-supplied interface device version (DWORD in the form 0xMMmmbbbb where MM = major version, mm = minor version, and bbbb = build number).  (SCARD_ATTR_VENDOR_IFD_VERSION)
        /// </summary>
        VendorInterfaceDeviceTypeVersion        = (Class.VendorInfo << 16) | 0x0102,
        /// <summary>
        /// Vendor-supplied interface device serial number. (SCARD_ATTR_VENDOR_IFD_SERIAL_NO)
        /// </summary>
        VendorInterfaceDeviceTypeSerialNumber   = (Class.VendorInfo << 16) | 0x0103,
        /// <summary>
        /// DWORD encoded as 0xDDDDCCCC, where DDDD = data channel type and CCCC = channel number (SCARD_ATTR_CHANNEL_ID)
        /// </summary>
        ChannelId                               = (Class.Communication << 16) | 0x0110,
        /// <summary>
        /// Asynchronous protocol types (SCARD_ATTR_ASYNC_PROTOCOL_TYPES)
        /// </summary>
        AsynchronusProtocolTypes                = (Class.Protocol << 16) | 0x0120,
        /// <summary>
        /// Default clock rate, in kHz. (SCARD_ATTR_DEFAULT_CLK)
        /// </summary>
        DefaultClockRate                        = (Class.Protocol << 16) | 0x0121,
        /// <summary>
        /// Maximum clock rate, in kHz. (SCARD_ATTR_MAX_CLK)
        /// </summary>
        MaxClockRate                            = (Class.Protocol << 16) | 0x0122,
        /// <summary>
        /// Default data rate, in bps. (SCARD_ATTR_DEFAULT_DATA_RATE)
        /// </summary>
        DefaultDataRate                         = (Class.Protocol << 16) | 0x0123,
        /// <summary>
        /// Maximum data rate, in bps. (SCARD_ATTR_MAX_DATA_RATE)
        /// </summary>
        MaxDataRate                             = (Class.Protocol << 16) | 0x0124,
        /// <summary>
        /// Maximum bytes for information file size device. (SCARD_ATTR_MAX_IFSD)
        /// </summary>
        MaxInformationFileSizeDevice            = (Class.Protocol << 16) | 0x0125,
        /// <summary>
        /// Synchronous protocol types (SCARD_ATTR_SYNC_PROTOCOL_TYPES)
        /// </summary>
        SyncProtocolTypes                       = (Class.Protocol << 16) | 0x0126,
        /// <summary>
        /// Zero if device does not support power down while smart card is inserted. Nonzero otherwise. (SCARD_ATTR_POWER_MGMT_SUPPORT)
        /// </summary>
        PowerManagementSupport                  = (Class.PowerManagement << 16) | 0x0131,
        /// <summary>
        /// User to card authentication device (SCARD_ATTR_USER_TO_CARD_AUTH_DEVICE)
        /// </summary>
        UserToCardAuthDevice                    = (Class.Security << 16) | 0x0140,
        /// <summary>
        /// User authentication input device (SCARD_ATTR_USER_AUTH_INPUT_DEVICE)
        /// </summary>
        UserAuthInputDevice                     = (Class.Security << 16) | 0x0142,
        /// <summary>
        /// DWORD indicating which mechanical characteristics are supported. If zero, no special characteristics are supported. Note that multiple bits can be set (SCARD_ATTR_CHARACTERISTICS)
        /// </summary>
        Characteristics                         = (Class.Mechanical << 16) | 0x0150,

        /// <summary>
        /// Current protocol type (SCARD_ATTR_CURRENT_PROTOCOL_TYPE)
        /// </summary>
        CurrentProtocolType                     = (Class.InterfaceDeviceProtocol << 16) | 0x0201,
        /// <summary>
        /// Current clock rate, in kHz. (SCARD_ATTR_CURRENT_CLK)
        /// </summary>
        CurrentClockRate                        = (Class.InterfaceDeviceProtocol << 16) | 0x0202,
        /// <summary>
        /// Clock conversion factor. (SCARD_ATTR_CURRENT_F)
        /// </summary>
        CurrentClockConversionFactor            = (Class.InterfaceDeviceProtocol << 16) | 0x0203,
        /// <summary>
        /// Bit rate conversion factor. (SCARD_ATTR_CURRENT_D)
        /// </summary>
        CurrentBitRateConversionFactor          = (Class.InterfaceDeviceProtocol << 16) | 0x0204,
        /// <summary>
        /// Current guard time. (SCARD_ATTR_CURRENT_N)
        /// </summary>
        CurrentGuardTime                        = (Class.InterfaceDeviceProtocol << 16) | 0x0205,
        /// <summary>
        /// Current work waiting time. (SCARD_ATTR_CURRENT_W)
        /// </summary>
        CurrentWaitingTime                      = (Class.InterfaceDeviceProtocol << 16) | 0x0206,
        /// <summary>
        /// Current byte size for information field size card. (SCARD_ATTR_CURRENT_IFSC)
        /// </summary>
        CurrentInformationFieldSizeCard         = (Class.InterfaceDeviceProtocol << 16) | 0x0207,
        /// <summary>
        /// Current byte size for information field size device. (SCARD_ATTR_CURRENT_IFSD)
        /// </summary>
        CurrentInformationFieldSizeDevice       = (Class.InterfaceDeviceProtocol << 16) | 0x0208,
        /// <summary>
        /// Current block waiting time. (SCARD_ATTR_CURRENT_BWT)
        /// </summary>
        CurrentBlockWaitingTime                 = (Class.InterfaceDeviceProtocol << 16) | 0x0209,
        /// <summary>
        /// Current character waiting time. (SCARD_ATTR_CURRENT_CWT)
        /// </summary>
        CurrentCharacterWaitingTime             = (Class.InterfaceDeviceProtocol << 16) | 0x020a,
        /// <summary>
        /// Current error block control encoding. (SCARD_ATTR_CURRENT_EBC_ENCODING)
        /// </summary>
        CurrentErrorBlockControlEncoding        = (Class.InterfaceDeviceProtocol << 16) | 0x020b,
        /// <summary>
        /// Extended block wait time. (SCARD_ATTR_EXTENDED_BWT)
        /// </summary>
        ExtendedBlockWaitTime                   = (Class.InterfaceDeviceProtocol << 16) | 0x020c,
        /// <summary>
        /// Single byte indicating smart card presence(SCARD_ATTR_ICC_PRESENCE)
        /// </summary>
        ICCPresence                             = (Class.ICCState << 16) | 0x0300,
        /// <summary>
        /// Single byte. Zero if smart card electrical contact is not active; nonzero if contact is active. (SCARD_ATTR_ICC_INTERFACE_STATUS)
        /// </summary>
        ICCInterfaceStatus                      = (Class.ICCState << 16) | 0x0301,
        /// <summary>
        /// Current IO state (SCARD_ATTR_CURRENT_IO_STATE)
        /// </summary>
        CurrentIOState                          = (Class.ICCState << 16) | 0x0302,
        /// <summary>
        /// Answer to reset (ATR) string. (SCARD_ATTR_ATR_STRING)
        /// </summary>
        AnswerToResetString                     = (Class.ICCState << 16) | 0x0303,
        /// <summary>
        /// Single byte indicating smart card type (SCARD_ATTR_ICC_TYPE_PER_ATR)
        /// </summary>
        ICCTypePerAtr                           = (Class.ICCState << 16) | 0x0304,
        /// <summary>
        /// Esc reset (SCARD_ATTR_ESC_RESET)
        /// </summary>
        EscReset                                = (Class.VendorDefined << 16) | 0xA000,
        /// <summary>
        /// Esc cancel (SCARD_ATTR_ESC_CANCEL)
        /// </summary>
        EscCancel                               = (Class.VendorDefined << 16) | 0xA003,
        /// <summary>
        /// Esc authentication request (SCARD_ATTR_ESC_AUTHREQUEST)
        /// </summary>
        EscAuthRequest                          = (Class.VendorDefined << 16) | 0xA005,
        /// <summary>
        /// Maximum input (SCARD_ATTR_MAXINPUT)
        /// </summary>
        MaxInput                                = (Class.VendorDefined << 16) | 0xA007,
        /// <summary>
        /// Instance of this vendor's reader attached to the computer. The first instance will be device unit 0, the next will be unit 1 (if it is the same brand of reader) and so on. Two different brands of readers will both have zero for this value. (SCARD_ATTR_DEVICE_UNIT)
        /// </summary>
        DeviceUnit                              = (Class.System << 16) | 0x0001,
        /// <summary>
        /// Reserved for future use. (SCARD_ATTR_DEVICE_IN_USE)
        /// </summary>
        DeviceInUse                             = (Class.System << 16) | 0x0002,
        /// <summary>
        /// Device friendly name ASCII (SCARD_ATTR_DEVICE_FRIENDLY_NAME_A)
        /// </summary>
        DeviceFriendlyNameA                     = (Class.System << 16) | 0x0003,
        /// <summary>
        /// Device system name ASCII (SCARD_ATTR_DEVICE_SYSTEM_NAME_A)
        /// </summary>
        DeviceSystemNameA                       = (Class.System << 16) | 0x0004,
        /// <summary>
        /// Device friendly name UNICODE (SCARD_ATTR_DEVICE_FRIENDLY_NAME_W)
        /// </summary>
        DeviceFriendlyNameW                     = (Class.System << 16) | 0x0005,
        /// <summary>
        /// Device system name UNICODE (SCARD_ATTR_DEVICE_SYSTEM_NAME_W)
        /// </summary>
        DeviceSystemNameW                       = (Class.System << 16) | 0x0006,
        /// <summary>
        /// Supress T1 information file size request (SCARD_ATTR_SUPRESS_T1_IFS_REQUEST)
        /// </summary>
        SupressT1InformationFileSizeRequest     = (Class.System << 16) | 0x0007,
        /// <summary>
        /// Device friendly name (SCARD_ATTR_DEVICE_FRIENDLY_NAME)
        /// </summary>
        DeviceFriendlyName                      = DeviceFriendlyNameA,
        /// <summary>
        /// Device system name (SCARD_ATTR_DEVICE_SYSTEM_NAME)
        /// </summary>
        DeviceSystemName                        = DeviceSystemNameA
    };
}
