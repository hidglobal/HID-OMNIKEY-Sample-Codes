/*****************************************************************************************
    (c) 2017 HID Global Corporation/ASSA ABLOY AB.  All rights reserved.

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
using System.ComponentModel;

namespace HidGlobal.OK.Readers.Components
{
    [Flags]
    public enum EnabledCLFeatures : ushort
    {
        [Description("Felica Support Available")]
        Felica = 0x0001,
        [Description("EMVco Support Available")]
        EMVCoCL = 0x0002,
        [Description("Calypso Support Available")]
        CalypsoSupport = 0x0004,
        [Description("NFC P2P Support Available")]
        NfcP2p = 0x0008,
        [Description("SIO Processor Available")]
        SioProcessor = 0x0010,
        [Description("SDR (LF Processor) Available")]
        Sdr = 0x0020,
        [Description("Native FW Secure Engine Available")]
        NativeFWSecureEngine = 0x0040,
        [Description("T = CL Support Available")]
        Cl = 0x0080,
        [Description("ISO 14443 Type A Support Available")]
        Iso14443a = 0x0100,
        [Description("ISO 14443 Type B Support Available")]
        Iso14443b = 0x0200,
        [Description("ISO 15693 Support Available")]
        Iso15693 = 0x0400,
        [Description("PicoPass 15693-2 Support Available")]
        PicoPass15693 = 0x0800,
        [Description("PicoPass 14443B-2 Support Available")]
        PicoPass14443b = 0x1000,
        [Description("Picopass 14443B-3 support available")]
        PicoPass14443a = 0x2000,
        [Description("Reserved For Future Use")]
        Rfu1 = 0x4000,
        [Description("Reserved For Future Use")]
        Rfu2 = 0x8000,
    };
}
