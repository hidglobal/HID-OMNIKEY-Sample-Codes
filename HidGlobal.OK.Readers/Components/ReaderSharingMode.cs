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
using System.ComponentModel;
using HidGlobal.OK.Readers.WinSCard;

namespace HidGlobal.OK.Readers.Components
{
    /// <summary>
    /// Defines for the share mode input parameter of SCardConnect <see cref="WinSCardWrapper.SCardConnect(IntPtr, string, int, int, IntPtr, int)"/>.
    /// </summary>
    public enum ReaderSharingMode : int
    {
        /// <summary>
        /// Sharing mode not specified, SCardConnect throws exception if an attempt to connect with this value is made.
        /// </summary>
        [Description("Not set.")]
        NotSet = 0,

        /// <summary>
        /// This application will not allow others to share the reader.
        /// </summary>
        [Description("Exclusive mode only.")]
        Exclusive               = 1,

        /// <summary>
        /// This application will allow others to share the reader.
        /// </summary>
        [Description("Shared mode only.")]
        Shared                  = 2,

        /// <summary>
        /// Direct control of the reader, even without a card. SCARD_SHARE_DIRECT can be used before using SCardControl() 
        /// to send control commands to the reader even if a card is not present in the reader.
        /// </summary>
        [Description("Raw mode only.")]
        Direct                  = 3
    };
}
