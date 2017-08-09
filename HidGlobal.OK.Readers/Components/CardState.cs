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
    /// <summary>State of the smart card in the reader.</summary>
    public enum CardState
    {
        /// <summary>
        /// Unknown status.
        /// </summary>
        [Description("Unknown state")]
        Unknown             = 1,

        /// <summary>
        /// There is no card in the reader.
        /// </summary>
        [Description("Card is absent")]
        Absent              = 2,
        /// <summary>
        /// There is a card in the reader, but it has not been moved into position for use.
        /// </summary>
        /// 
        [Description("Card is present")]
        Present             = 4,
        /// <summary>
        /// There is a card in the reader in position for use. The card is not powered.
        /// </summary>
        /// 
        [Description("Card not powered")]
        Swallowed           = 8,

        /// <summary>
        /// Power is being provided to the card, but the reader driver is unaware of the mode of the card.
        /// </summary>
        [Description("Card is powered")]
        Powered             = 16,

        /// <summary>
        /// The card has been reset and is awaiting PTS negotiation.
        /// </summary>
        [Description("Ready for PTS")]
        Negotiable          = 32,

        /// <summary>
        /// The card has been reset and specific communication protocols have been established.
        /// </summary>
        [Description("PTS has been set")]
        Specific            = 64
    }
}
