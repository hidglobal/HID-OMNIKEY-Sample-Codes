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
using System.Collections.Generic;
using System.Text;

namespace HidGlobal.OK.Readers.Utilities
{
    //TODO: KW: write unit tests for this class 

    public class PrePostStrokesBuilder
    {
        public enum SpecialCharacter : byte
        {
            NullTerminator = 0x00,
            Enter          = 0x01,
            CursorDown     = 0x02,
            CursorLeft     = 0x03,
            CursorRight    = 0x04,
            CursorUp       = 0x05,
            Space          = 0x06,
            CarriageReturn = 0x07,
            LineFeed       = 0x08,
            Tab            = 0x09,
            LEDnBuzzer     = 0x0a,
        }

        private readonly List<byte> _data = new List<byte>();

        public void AppendText(string text)
        {  
            if(text == null) throw new ArgumentNullException(nameof(text));

            _data.AddRange(Encoding.ASCII.GetBytes(text));
        }

        public void AppendBytes(byte[] bytes)
        {
            if(bytes == null) throw  new ArgumentNullException(nameof(bytes));

            _data.AddRange(bytes);
        }

        public void AppendSpecialCharacter(SpecialCharacter specialCharacter)
        {
            if (Enum.IsDefined(typeof(SpecialCharacter), specialCharacter))
            {
                _data.Add((byte) specialCharacter);
            }
            else
            {
                throw new ArgumentException(nameof(specialCharacter));
            }
        }

        public byte[] GetData()
        {
            return _data.ToArray();
        }
    }
}
