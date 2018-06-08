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
using System.ComponentModel;

namespace HidGlobal.OK.Readers.Components
{
    /// <summary>
    /// Defines the action to take on the card in the connected reader when calling SCardDisconnect <see cref="WinSCard.SCardDisconnect(IntPtr, int)"/>
    /// </summary>
    public enum CardDisposition : int
    {
        /// <summary>
        /// Do not do anything special.
        /// </summary>
        [Description("No action.")]
        Leave                   = 0,

        /// <summary>
        /// Reset the card.
        /// </summary>
        [Description("Reset the card")]
        Reset                   = 1,

        /// <summary>
        /// Power down the card.
        /// </summary>
        [Description("Unpower the card")]
        Unpower                 = 2,

        /// <summary>
        /// Eject the card.
        /// </summary>
        [Description("Eject the card")]
        Eject                   = 3
    };

    public enum CardInitialization : int
    {
        /// <summary>
        /// Do not do anything special on reconnect.
        /// </summary>
        [Description("No action.")]
        Leave = 0,

        /// <summary>
        /// Reset the card (Warm Reset).
        /// </summary>
        [Description("Reset the card (Warm Reset)")]
        WarmReset = 1,

        /// <summary>
        /// Power down the card and reset it (Cold Reset).
        /// </summary>
        [Description("Power down the card and reset it (Cold Reset)")]
        ColdReset = 2,
    };

}
