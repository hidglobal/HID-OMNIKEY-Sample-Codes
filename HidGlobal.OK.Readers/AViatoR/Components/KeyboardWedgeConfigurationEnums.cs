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

namespace HidGlobal.OK.Readers.AViatoR.Components
{
    public enum LedIdleState : byte
    {
        Off = 0x00,
        On = 0x01
    };

    public enum ConfigurationType : byte
    {
        KeyboardWedgeConfig1 = 0xA8,
        KeyboardWedgeConfig2 = 0xA9,
        KeyboardWedgeConfig3 = 0xAa,
    };

    public enum CardType : byte
    {
        NotUsed = 0x00,
        MifareClassic = 0x01,
        Ultralight = 0x02,
        DesFire = 0x03,
        Seos = 0x04,
        Iclass = 0x05,
        FeliCa = 0x06,
        Iso15693 = 0x08,
        Iso14443B = 0x09,
        GenericIso14443A = 0x0a,
    };

    public enum KbWedgeConfigCommand : byte
    {
        KbwCardType = 0x80,
        KbwKeyboardLayout = 0x80,
        KbwExtendedCharSupport = 0x80,
        KbwOutputFormat = 0x81,
        KbwCharactersDiff = 0x81,
        KbwFlags = 0x82,
        KbwRangeStart = 0x83,
        KbwRangeLength = 0x84,
        KbwPostStrokesStart = 0x85,
        KbwPrePostStrokes = 0x86,
    };



    public enum OutputFormat : byte
    {
        Ascii = 0x00,
        Bcd = 0x01,
        Bin = 0x02,
        HexLowerCase = 0x03,
        Dec = 0x04,
        HexUpperCase = 0x05,
    };

    public enum OutputType : byte
    {
        Uid = 0x00,
        Pacs = 0x01,
    };

    public enum DataOrder : byte
    {
        Normal = 0x00,
        Reversed = 0x01,
    };

    public enum KeyboardLayout : byte
    {
        DefaultUsLayout = 0x00,
        SecondLayout = 0x01,
    };

    public enum ExtendedCharacterSupport : byte
    {
        Windows = 0x00,
        Linux = 0x01,
        MacOs = 0x02,
    };

    public enum FeatureSupport : byte
    {
        Disabled = 0x00,
        Enabled = 0x01
    };
}
