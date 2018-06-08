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
using HidGlobal.OK.Readers.WinSCard;

namespace HidGlobal.OK.Readers.Components
{
    /// <summary>
    /// Current state of the reader, as seen by the application. 
    /// This field can take on any of the following values, in combination, as a bitmask. 
    /// </summary>
    [Flags]
    public enum ReaderStates : int
    {
        /// <summary>
        /// The application is unaware of the current state, and would like to know. 
        /// The use of this value results in an immediate return from state transition 
        /// monitoring services. This is represented by all bits set to zero.
        /// </summary>
        [Description("Application wants status.")]
        Unaware          = 0,

        /// <summary>
        /// The application is not interested in this reader, and it should not be considered during monitoring operations. 
        /// If this bit value is set, all other bits are ignored.
        /// </summary>
        [Description("Ignore this reader.")]
        Ignore           = 1,

        /// <summary>
        /// This implies that there is a difference between the state believed by 
        /// the application, and the state known by the Service Manager.  When this 
        /// bit is set, the application may assume a significant state change has
        /// occurred on this reader. 
        /// </summary>
        [Description("State has changed.")]
        Changed          = 1 << 1,

        /// <summary>
        /// This implies that the given reader name is not recognized by the Service Manager.
        /// If this bit is set, then <see cref="ReaderStates.Changed"/> and <see cref="ReaderStates.Ignore"/> will also be set.
        /// </summary>
        [Description("Reader unknown.")]
        Unknown          = 1 << 2,

        /// <summary>
        /// The application expects that this reader is not available for use. 
        /// If this bit is set, then all the following bits are ignored.
        /// </summary>
        [Description("Status unavailable.")]
        Unavailable      = 1 << 3,

        /// <summary>
        /// The application expects that there is no card in the reader. 
        /// If this bit is set, all the following bits are ignored.
        /// </summary>
        [Description("Card removed.")]
        Empty            = 1 << 4,

        /// <summary>
        /// The application expects that there is a card in the reader.
        /// </summary>
        [Description("Card inserted.")]
        Present          = 1 << 5,

        /// <summary>
        /// The application expects that there is a card in the reader with an ATR that matches one of the target cards. 
        /// If this bit is set, <see cref="ReaderStates.Present"/> is assumed. 
        /// This bit has no meaning to <see cref="WinSCardWrapper.SCardGetStatusChange(IntPtr, int, SCardReaderState, int)"/> beyond <see cref="ReaderStates.Present"/>.
        /// </summary>
        [Description("ATR matches card.")]
        AtrMatch         = 1 << 6,

        /// <summary>
        /// The application expects that the card in the reader is allocated for exclusive use by another application. 
        /// If this bit is set, <see cref="ReaderStates.Present"/> is assumed.
        /// </summary>
        [Description("Exclusive Mode.")]
        Exclusive        = 1 << 7,

        /// <summary>
        /// The application expects that the card in the reader is in use by one or more other applications, but may be connected to in shared mode. 
        /// If this bit is set, <see cref="ReaderStates.Present"/> is assumed.
        /// </summary>
        [Description("In use.")]
        InUse            = 1 << 8,

        /// <summary>
        /// The application expects that there is an unresponsive card in the reader.
        /// </summary>
        [Description("Unresponsive card.")]
        Mute             = 1 << 9,

        /// <summary>
        /// This implies that the card in the reader has not been powered up.
        /// </summary>
        [Description("Unpowered card.")]
        Unpowered        = 1 << 10,
    };

}
