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
using System.ComponentModel;


namespace HidGlobal.OK.Readers.Components
{
    public enum PollingFrequency
    {
        [Description("41Hz (24ms)")]
        T24ms = 0x00,
        [Description("20Hz (48ms)")]
        T48ms = 0x01,
        [Description("10Hz (96ms)")]
        T96ms = 0x02,
        [Description("5Hz (0.2s)")]
        T200ms = 0x03,
        [Description("2.5Hz (0.4s)")]
        T400ms = 0x04,
        [Description("1.3Hz (0.8s)")]
        T800ms = 0x05,
        [Description("0.7Hz (1.4s)")]
        T1400ms = 0x06,
        [Description("0.3Hz (3.1s)")]
        T3100ms = 0x07,
        [Description("0.15Hz (6.2s)")]
        T6200ms = 0x08,
        [Description("0.08Hz (12.3s)")]
        T12300ms = 0x09,
    };
}
