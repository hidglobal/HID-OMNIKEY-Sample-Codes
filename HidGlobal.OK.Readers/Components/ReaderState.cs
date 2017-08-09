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
using System.Runtime.InteropServices;

namespace HidGlobal.OK.Readers.Components
{
    /// <summary>
    /// The SCardReaderState structure is used by functions for tracking smart cards within readers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ReaderState
    {
        /// <summary>
        /// Name of the reader being monitored.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string ReaderName;

        /// <summary>
        /// Not used by the smart card subsystem. This member is used by the application.
        /// </summary>
        public IntPtr UserData;

        /// <summary>
        /// Current state of the reader, as seen by the application. This field can take any value of <see cref="Enums.ReaderStates"/>, in combination, as a bitmask.
        /// </summary>
        public int CurrentState;

        /// <summary>
        /// Current state of the reader, as known by the smart card resource manager. This field can take any value of <see cref="Enums.ReaderStates"/>, in combination, as a bitmask.
        /// </summary>
        public int EventState;

        /// <summary>
        /// Number of bytes in the returned ATR.
        /// </summary>
        public int AtrLength;

        /// <summary>
        /// ATR of the inserted card, with extra alignment bytes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = UnmanagedType.U1)]
        public byte[] Atr;
    };
}
