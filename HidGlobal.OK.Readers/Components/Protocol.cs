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
    /// <summary>
    /// The Smart Card Protocol
    /// </summary>
    [Flags]
    public enum Protocol : int
    {
        /// <summary>
        /// There is no active protocol, usable when connecting reader in Direct mode. <see cref="SCardSharedMode.Direct"/>
        /// </summary>
        [Description("Protocol not set.")]
        None                = 0,

        /// <summary>
        /// T0 is a active protocol.
        /// </summary>
        [Description("T=0 active protocol.")]
        T0                  = 1,

        /// <summary>
        /// T1 is a active protocol.
        /// </summary>
        [Description("T=1 active protocol.")]
        T1                  = 2,

        /// <summary>
        /// IFD determines a active protocol.
        /// </summary>
        [Description("IFD determines active protocol")]
        Any                 = T0|T1,

        /// <summary>
        /// Raw is a active protocol.
        /// </summary>
        [Description("Raw active protocol.")]
        Raw                 = 4,

        /// <summary>
        /// T15 is a active protocol.
        /// </summary>
        [Description("T=15 protocol")]
        T15                 = 8,
    };
}
