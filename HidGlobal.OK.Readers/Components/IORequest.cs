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
using System.Runtime.InteropServices;

namespace HidGlobal.OK.Readers.Components
{
    /// <summary>
    /// The SCardIORequest structure begins a protocol control information structure.
    /// Any protocol-specific information then immediately follows this structure.
    /// </summary>
    /// <remarks>
    /// The entire length of the structure must be aligned with the underlying hardware architecture word size. 
    /// For example, in Win32 the length of any PCI information must be a multiple of four bytes so that it aligns on a 32-bit boundary.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct IORequest
    {
        /// <summary>
        /// Protocol in use <see cref="SCardProtocol"/>.
        /// </summary>
        public int Protocol;

        /// <summary>
        /// Length, in bytes, of the SCardIORequest structure plus any following PCI-specific information.
        /// </summary>
        public int PciLength;
    };
}
