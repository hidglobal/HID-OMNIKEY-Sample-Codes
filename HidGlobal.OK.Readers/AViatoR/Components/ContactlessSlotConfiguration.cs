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
using System.Collections.Generic;
using System.Linq;
using HidGlobal.OK.Readers.Components;

namespace HidGlobal.OK.Readers.AViatoR.Components
{

    #region Iso/Iec14443TypeA
    public class Iso14443TypeAConfiguration
    {
        public Iso14443TypeAEnable Enable => new Iso14443TypeAEnable();
        public Iso14443TypeARxTxBaudRate RxTxBaudRate => new Iso14443TypeARxTxBaudRate();
        public MifareKeyCache MifareKeyCache => new MifareKeyCache();
        public MifarePreferred MifarePreferred => new MifarePreferred();
    }

    public class Iso14443TypeAEnable
    {
        public string GetApdu => "FF70076B0AA208A006A404A202800000";
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A2038001" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038001") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    public class Iso14443TypeARxTxBaudRate
    {
        public string GetApdu => "FF70076B0AA208A006A404A202810000"; 
        public string SetApdu(BaudRate selectedRates)
            {
                byte data = 0x00;

                if (selectedRates.Rx212)
                    data |= (byte)BaudRates.Rate212 << 4;

                if (selectedRates.Rx424)
                    data |= (byte)BaudRates.Rate424 << 4;

                if (selectedRates.Rx848)
                    data |= (byte)BaudRates.Rate848 << 4;

                if (selectedRates.Tx212)
                    data |= (byte)BaudRates.Rate212;

                if (selectedRates.Tx424)
                    data |= (byte)BaudRates.Rate424;

                if (selectedRates.Tx848)
                    data |= (byte)BaudRates.Rate848;


                return "FF70076B0BA209A107A405A2038101" + data.ToString("X2") + "00";
            }
        public BaudRate TranslateGetResponse(string response)
            {
                response = response.Replace(" ", "");
                var avalibleRates = new BaudRate
                {
                    Rx106 = true,
                    Rx212 = false,
                    Rx424 = false,
                    Rx848 = false,

                    Tx106 = true,
                    Tx212 = false,
                    Tx424 = false,
                    Tx848 = false,
                };

                if (!(response.StartsWith("BD038101") && response.EndsWith("9000")))
                    throw new ArgumentException("Wrong response instruction, response should be following string: BD038101xx9000, where xx is a one byte variable.");

                int rxflags = Convert.ToInt32(response.Substring(8, 1), 16);
                int txflags = Convert.ToInt32(response.Substring(9, 1), 16);

                avalibleRates.Rx106 = true;
                avalibleRates.Rx212 = (Convert.ToInt32(BaudRates.Rate212) & rxflags) != 0;
                avalibleRates.Rx424 = (Convert.ToInt32(BaudRates.Rate424) & rxflags) != 0;
                avalibleRates.Rx848 = (Convert.ToInt32(BaudRates.Rate848) & rxflags) != 0;

                avalibleRates.Tx106 = true;
                avalibleRates.Tx212 = (Convert.ToInt32(BaudRates.Rate212) & txflags) != 0;
                avalibleRates.Tx424 = (Convert.ToInt32(BaudRates.Rate424) & txflags) != 0;
                avalibleRates.Tx848 = (Convert.ToInt32(BaudRates.Rate848) & txflags) != 0;

                return avalibleRates;
            }
    }

    public class MifareKeyCache
    {
        public string GetApdu => "FF70076B0AA208A006A404A202830000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A2038301" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038301") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    public class MifarePreferred
    {
        public string GetApdu => "FF70076B0AA208A006A404A202840000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A2038401" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038401") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    #endregion

    #region Iso/Iec14443TypeB

    public class Iso14443TypeBConfiguration
    {
        public Iso14443TypeBEnable Enable => new Iso14443TypeBEnable();
        public Iso14443TypeBRxTxBaudRate RxTxBaudRate => new Iso14443TypeBRxTxBaudRate();
    }

    public class Iso14443TypeBEnable
    {
        public string GetApdu => "FF70076B0AA208A006A404A302800000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A3038001" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038001") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    public class Iso14443TypeBRxTxBaudRate
    {
        public string GetApdu => "FF70076B0AA208A006A404A302810000"; 
        public string SetApdu(BaudRate selectedRates)
            {
                byte data = 0x00;

                if (selectedRates.Rx212)
                    data |= (byte)BaudRates.Rate212 << 4;

                if (selectedRates.Rx424)
                    data |= (byte)BaudRates.Rate424 << 4;

                if (selectedRates.Rx848)
                    data |= (byte)BaudRates.Rate848 << 4;

                if (selectedRates.Tx212)
                    data |= (byte)BaudRates.Rate212;

                if (selectedRates.Tx424)
                    data |= (byte)BaudRates.Rate424;

                if (selectedRates.Tx848)
                    data |= (byte)BaudRates.Rate848;

                return "FF70076B0BA209A107A405A3038101" + data.ToString("X2") + "00";
            }
        public BaudRate TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            var avalibleRates = new BaudRate
            {
                Rx106 = true,
                Rx212 = false,
                Rx424 = false,
                Rx848 = false,

                Tx106 = true,
                Tx212 = false,
                Tx424 = false,
                Tx848 = false,
            };

            if (!(response.StartsWith("BD038101") && response.EndsWith("9000")))
                throw new ArgumentException("Wrong response instruction, response should be following string: BD038101xx9000, where xx is a one byte variable.");

            int rxflags = Convert.ToInt32(response.Substring(8, 1), 16);
            int txflags = Convert.ToInt32(response.Substring(9, 1), 16);

            avalibleRates.Rx106 = true;
            avalibleRates.Rx212 = (Convert.ToInt32(BaudRates.Rate212) & rxflags) != 0;
            avalibleRates.Rx424 = (Convert.ToInt32(BaudRates.Rate424) & rxflags) != 0;
            avalibleRates.Rx848 = (Convert.ToInt32(BaudRates.Rate848) & rxflags) != 0;

            avalibleRates.Tx106 = true;
            avalibleRates.Tx212 = (Convert.ToInt32(BaudRates.Rate212) & txflags) != 0;
            avalibleRates.Tx424 = (Convert.ToInt32(BaudRates.Rate424) & txflags) != 0;
            avalibleRates.Tx848 = (Convert.ToInt32(BaudRates.Rate848) & txflags) != 0;

            return avalibleRates;
        }
    }


    #endregion

    #region Iso15693

    public class Iso15693Configuration
    {
        public Iso15693Enable Enable => new Iso15693Enable();
    }

    public class Iso15693Enable
    {
        public string GetApdu => "FF70076B0AA208A006A404A402800000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A4038001" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038001") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    #endregion

    #region FelicaConfiguration

    public class FelicaConfiguration
    {
        public FelicaEnable Enable => new FelicaEnable();
        public FelicaRxTxBaudRate RxTxBaudRate => new FelicaRxTxBaudRate();
    }

    public class FelicaEnable
    {
        public string GetApdu => "FF70076B0AA208A006A404A502800000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A5038001" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038001") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    public class FelicaRxTxBaudRate
    {
        public string GetApdu => "FF70076B0AA208A006A404A502810000";
        public string SetApdu(BaudRate selectedRates)
        {
            byte data = 0x00;

            if (selectedRates.Rx212)
                data |= (byte)BaudRates.Rate212 << 4;

            if (selectedRates.Rx424)
                data |= (byte)BaudRates.Rate424 << 4;

            if (selectedRates.Rx848)
                data |= (byte)BaudRates.Rate848 << 4;

            if (selectedRates.Tx212)
                data |= (byte)BaudRates.Rate212;

            if (selectedRates.Tx424)
                data |= (byte)BaudRates.Rate424;

            if (selectedRates.Tx848)
                data |= (byte)BaudRates.Rate848;

            return "FF70076B0BA209A107A405A5038101" + data.ToString("X2") + "00";
        }
        public BaudRate TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            var avalibleRates = new BaudRate
            {
                Rx106 = true,
                Rx212 = false,
                Rx424 = false,
                Rx848 = false,

                Tx106 = true,
                Tx212 = false,
                Tx424 = false,
                Tx848 = false,
            };

            if (!(response.StartsWith("BD038101") && response.EndsWith("9000")))
                throw new ArgumentException("Wrong response instruction, response should be following string: BD038101xx9000, where xx is a one byte variable.");

            int rxflags = Convert.ToInt32(response.Substring(8, 1), 16);
            int txflags = Convert.ToInt32(response.Substring(9, 1), 16);

            avalibleRates.Rx106 = true;
            avalibleRates.Rx212 = (Convert.ToInt32(BaudRates.Rate212) & rxflags) != 0;
            avalibleRates.Rx424 = (Convert.ToInt32(BaudRates.Rate424) & rxflags) != 0;
            avalibleRates.Rx848 = (Convert.ToInt32(BaudRates.Rate848) & rxflags) != 0;

            avalibleRates.Tx106 = true;
            avalibleRates.Tx212 = (Convert.ToInt32(BaudRates.Rate212) & txflags) != 0;
            avalibleRates.Tx424 = (Convert.ToInt32(BaudRates.Rate424) & txflags) != 0;
            avalibleRates.Tx848 = (Convert.ToInt32(BaudRates.Rate848) & txflags) != 0;

            return avalibleRates;
        }
    }
    #endregion

    #region iClass15693

    public class iClassConfiguration
    {
        public iClass15693Enable Enable => new iClass15693Enable();
    }

    public class iClass15693Enable
    {
        public string GetApdu => "FF70076B0AA208A006A404A602830000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A6038301" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038301") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }
    
    #endregion

    #region ContactlessCommon

    public class PollingRFModuleEnable
    {
        public string GetApdu => "FF70076B0AA208A006A404A0028A0000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A0038A01" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038A01") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    public class EmdSuppresionEnable
    {
        public string GetApdu => "FF70076B0AA208A006A404A002870000";
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A0038701" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038701") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    public class SleepModeCardDetectionEnable
    {
        public string GetApdu => "FF70076B0AA208A006A404A0028E0000"; 
        public string SetApdu(bool enable) { return "FF70076B0BA209A107A405A0038E01" + (enable ? "01" : "00") + "00"; }
        public string TranslateGetResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038E01") && response.EndsWith("9000")))
                return null;

            switch (response.Substring(8, 2))
            {
                case "00":
                    return "Disabled";
                case "01":
                    return "Enabled";
                default:
                    return null;
            }
        }
    }

    public class PollingSearchOrderConfig
    {
        public string GetApdu => "FF70076B0AA208A006A404A002890000"; 
        public string SetApdu(PollingSearchOrder first) { return "FF70076B0FA20DA10BA409A0078905" + ((byte)first).ToString("X2") + "0000000000"; }
        public string SetApdu(PollingSearchOrder first, 
                              PollingSearchOrder second)
            {
                return "FF70076B0FA20DA10BA409A0078905" + ((byte)first).ToString("X2") + ((byte)second).ToString("X2") + "00000000";
            }
        public string SetApdu(PollingSearchOrder first, 
                              PollingSearchOrder second, 
                              PollingSearchOrder third)
            {
                return "FF70076B0FA20DA10BA409A0078905" + ((byte)first).ToString("X2") + ((byte)second).ToString("X2") + ((byte)third).ToString("X2") + "000000";
            }
        public string SetApdu(PollingSearchOrder first, 
                              PollingSearchOrder second, 
                              PollingSearchOrder third, 
                              PollingSearchOrder forth)
            {
                return "FF70076B0FA20DA10BA409A0078905" + ((byte)first).ToString("X2") + ((byte)second).ToString("X2") + ((byte)third).ToString("X2") + ((byte)forth).ToString("X2") + "0000";
            }
        public string SetApdu(PollingSearchOrder first, 
                              PollingSearchOrder second, 
                              PollingSearchOrder third, 
                              PollingSearchOrder forth, 
                              PollingSearchOrder fifth)
            {
                return "FF70076B0FA20DA10BA409A0078905" + ((byte)first).ToString("X2") + ((byte)second).ToString("X2") + ((byte)third).ToString("X2")
                       + ((byte)forth).ToString("X2") + ((byte)fifth).ToString("X2") + "00";
            }
        public List<PollingSearchOrder> TranslateGetResponse(string response)
            {
                response = response.Replace(" ", "");
                if (!(response.StartsWith("BD078905") && response.EndsWith("9000")))
                    return null;

                response = response.Substring(8, 10);
                var order = new List<PollingSearchOrder>();
                order.AddRange(Enumerable.Range(0, 5).Select(x => (PollingSearchOrder)Convert.ToInt16(response.Substring(2 * x, 2), 16)).ToArray());

                return order;
            }
    }

    public class SleepModePollingFrequency
    {
        public string GetApdu => "FF70076B0AA208A006A404A0028D0000"; 
        public string SetApdu(PollingFrequency frequency) { return "FF70076B0BA209A107A405A0038D01" + ((byte)(frequency)).ToString("X2") + "00"; }
        public PollingFrequency TranslateGetResponse(string response)
            {
                response = response.Replace(" ", "");
                if (!(response.StartsWith("BD038D01") && response.EndsWith("9000")))
                    throw new ArgumentException("Wrong response instruction, response should be following string: BD038D01xx9000.");

                return (PollingFrequency)Convert.ToByte(response.Substring(8, 2), 16);
            }
    }

    #endregion


}
