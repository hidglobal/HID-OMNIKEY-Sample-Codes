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
using HidGlobal.OK.Readers.Components;
using System;
using System.Collections.Generic;

namespace HidGlobal.OK.Readers.AViatoR.Components
{
    public class ContactSlotConfiguration
    {
        public VoltageSequence VoltageSequence => new VoltageSequence();
        public OperatingMode OperatingMode => new OperatingMode();
        public ContactSlotEnable ContactSlotEnable => new ContactSlotEnable();
    }

    public class VoltageSequence
    {
        public string GetApdu => "FF70076B0AA208A006A304A002820000";

        public string SetApdu(VoltageSequenceFlags first, VoltageSequenceFlags second, VoltageSequenceFlags third)
        {
            byte sequence = (byte)(((int)first ) + ((int)second << 2) + ((int)third << 4));
            return $"FF70076B0BA209A107A305A0038201" + sequence.ToString("X2") + "00";
        }
        /// <summary>
        /// Set automatic sequence (device driver decides).
        /// </summary>
        /// <returns></returns>
        public string SetAutomaticSequenceApdu() { return "FF70076B0BA209A107A305A00382010000"; }

        /// <summary>
        /// Returns list of voltage sequence, when voltage sequence is set to automatic mode retunes empty list.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<VoltageSequenceFlags> TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038201") && response.EndsWith("9000")))
                return null;

            byte flags = Byte.Parse(response.Substring(8, 2), System.Globalization.NumberStyles.HexNumber);

            if (flags == 0x00)
                return new List<VoltageSequenceFlags>();
            
            var order = new List<VoltageSequenceFlags>
            {
                (VoltageSequenceFlags) (flags & 3),
                (VoltageSequenceFlags) ((flags >> 2) & 3),
                (VoltageSequenceFlags) ((flags >> 4) & 3)
            };
            return order;
        }
    }

    public class OperatingMode
    {
        public string GetApdu => "FF70076B0AA208A006A304A002830000";
        public string SetApdu(OperatingModeFlags cardOperatingModeFlags)
        {
            return "FF70076B0BA209A107A305A0038301" + ((byte)cardOperatingModeFlags).ToString("X2") + "00";
        }
        public OperatingModeFlags TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038301") && response.EndsWith("9000")))
                throw new ArgumentException("Wrong response instruction, response should be following string: BD038301xx9000.");

            return (OperatingModeFlags)Convert.ToByte(response.Substring(8, 2), 16);
        }
    }
    
    public class ContactSlotEnable
    {
        public string GetApdu => "FF70076B0AA208A006A304A002850000";
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A305A0038501" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038501") && response.EndsWith("9000")))
                return null;

            if (response.Substring(8, 2) == "00")
                return "Disabled";
            else if (response.Substring(8, 2) == "01")
                return "Enabled";
            else
                return null;
        }
    }

}
