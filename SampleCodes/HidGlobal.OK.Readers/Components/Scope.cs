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
    public enum Scope : int
    {
        /// <summary>
        /// Not used. The context is a user context, and any database operations are performed within the
        /// domain of the user. 
        /// </summary>
        [Description("Scope in user space.")]
        User                = 0,

        /// <summary>
        /// Not used. The context is that of the current terminal, and any database operations are performed
        /// within the domain of that terminal.  (The calling application must have appropriate
        /// access permissions for any database actions.)
        /// </summary>
        [Description("Scope in terminal")]
        Terminal            = 1,

        /// <summary>
        /// The context is the system context, and any database operations are performed within the
        /// domain of the system.  (The calling application must have appropriate access
        /// permissions for any database actions.)
        /// </summary>
        [Description("Scope in system.")]
        System              = 2,

        /// <summary>
        /// Not used. Global scope.
        /// </summary>
        [Description("Scope is global.")]
        Global              = 3
    };
}

