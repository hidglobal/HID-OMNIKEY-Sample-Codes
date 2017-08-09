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
using System.Text;

namespace HidGlobal.OK.Readers.AViatoR.Components
{
    public class ReaderCapabilities
    {
        public TlvVersion TlvVersion => new TlvVersion();
        public DeviceId DeviceId => new DeviceId();
        public ProductName ProductName => new ProductName();
        public ProductPlatform ProductPlatform => new ProductPlatform();
        public EnabledClFeatures EnabledClFeatures => new EnabledClFeatures();
        public FirmwareVersion FirmwareVersion => new FirmwareVersion();
        public HfControllerVersion HfControllerVersion => new HfControllerVersion();
        public HardwareVersion HardwareVersion => new HardwareVersion();
        public HostInterfaces HostInterfaces => new HostInterfaces();
        public NumberOfContactSlots NumberOfContactSlots => new NumberOfContactSlots();
        public NumberOfContactlessSlots NumberOfContactlessSlots => new NumberOfContactlessSlots();
        public NumberOfAntennas NumberOfAntennas => new NumberOfAntennas();
        public VendorName VendorName => new VendorName();
        public ExchangeLevel ExchangeLevel => new ExchangeLevel();
        public SerialNumber SerialNumber => new SerialNumber();
        public HfControllerType HfControllerType => new HfControllerType();
        public SizeOfUserEEPROM SizeOfUserEEPROM => new SizeOfUserEEPROM();
        public FirmwareLabel FirmwareLabel => new FirmwareLabel();
        public HumanInterfaces HumanInterfaces => new HumanInterfaces();
    }

    public class TlvVersion
    {
        public string GetApdu => "FF70076B08A206A004A002800000";
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038001") && response.EndsWith("9000")))
                return null;

            return Convert.ToInt32(response.Substring(8, 2), 16).ToString();
        }
    }

    public class DeviceId
    {
        public string GetApdu => "FF70076B08A206A004A002810000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD048102") && response.EndsWith("9000")))
                return null;

            return response.Substring(8, 4);
        }
    }

    public class ProductName
    {
        public string GetApdu => "FF70076B08A206A004A002820000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD") && response.EndsWith("9000")))
                return null;

            byte[] data = Enumerable.Range(0, response.Length / 2).Select(x => Convert.ToByte(response.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(data, 4, data[3]).Replace("\0","");
        }
    }

    public class ProductPlatform
    {
        public string GetApdu => "FF70076B08A206A004A002830000";
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD") && response.EndsWith("9000")))
                return null;

            byte[] data = Enumerable.Range(0, response.Length / 2).Select(x => Convert.ToByte(response.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(data, 4, data[3]).Replace("\0", "");
        }
    }

    public class EnabledClFeatures
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string GetApdu => "FF70076B08A206A004A002840000";
        public string[] TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD048402") && response.EndsWith("9000")))
                return null;

            var data = new List<string>();
            int flags = Convert.ToInt32(response.Substring(8, 4), 16);

            foreach (var element in Enum.GetValues(typeof(Readers.Components.EnabledCLFeatures)))
            {
                if ((flags & Convert.ToInt32(element)) == 0) continue;
                try
                {
                    var fieldinformation = element.GetType().GetField(element.ToString());
                    var fieldAttribiutes = (System.ComponentModel.DescriptionAttribute[])fieldinformation.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                    data.Add((fieldAttribiutes.Length > 0) ? fieldAttribiutes[0].Description : element.ToString());
                }
                catch (Exception error)
                {
                    log.Error(null, error);
                }
            }
            return data.ToArray();
        }
    }

    public class FirmwareVersion
    {
        public string GetApdu => "FF70076B08A206A004A002850000";
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD058503") && response.EndsWith("9000")))
                return null;

            var major = Convert.ToInt32(response.Substring(8, 2), 16);
            var minor = Convert.ToInt32(response.Substring(10, 2), 16);
            var revision = Convert.ToInt32(response.Substring(12, 2), 16);

            return major + "." + minor + "." + revision;
        }
    }

    public class HfControllerVersion
    {
        public string GetApdu => "FF70076B08A206A004A002880000";
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038801") && response.EndsWith("9000")))
                return null;

            return Convert.ToInt32(response.Substring(8, 2), 16).ToString();
        }
    }

    public class HardwareVersion
    {
        public string GetApdu => "FF70076B08A206A004A002890000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD") && response.EndsWith("9000")))
                return null;

            byte[] data = Enumerable.Range(0, response.Length / 2).Select(x => Convert.ToByte(response.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(data, 4, data[3]);
        }
    }

    public class HostInterfaces
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string GetApdu => "FF70076B08A206A004A0028A0000";
        public string[] TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038A01") && response.EndsWith("9000")))
                return null;

            var data = new List<string>();
            int flags = Convert.ToInt32(response.Substring(8, 2), 16);

            foreach (var element in Enum.GetValues(typeof(HidGlobal.OK.Readers.Components.HostInterfaceFlags)))
            {
                if ((flags & Convert.ToInt32(element)) == 0)
                    continue;

                try
                {
                    var fieldinformation = element.GetType().GetField(element.ToString());
                    var fieldAttribiutes = (System.ComponentModel.DescriptionAttribute[])fieldinformation.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                    data.Add((fieldAttribiutes.Length > 0) ? fieldAttribiutes[0].Description : element.ToString());
                }
                catch (Exception error)
                {
                    log.Error(null, error);
                }
            }
            return data.ToArray();
        }
    }

    public class NumberOfContactSlots
    {
        public string GetApdu => "FF70076B08A206A004A0028B0000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038B01") && response.EndsWith("9000")))
                return null;

            return Convert.ToInt16(response.Substring(8, 2), 16).ToString();
        }
    }

    public class NumberOfContactlessSlots
    {
        public string GetApdu => "FF70076B08A206A004A0028C0000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038C01") && response.EndsWith("9000")))
                return null;

            return Convert.ToInt16(response.Substring(8, 2), 16).ToString();
        }
    }

    public class NumberOfAntennas
    {
        public string GetApdu => "FF70076B08A206A004A0028D0000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD038D01") && response.EndsWith("9000")))
                return null;

            return Convert.ToInt32(response.Substring(8, 2), 16).ToString();
        }
    }

    public class HumanInterfaces
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string GetApdu => "FF70076B08A206A004A0028E0000"; 
        public string[] TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD058E038001") && response.EndsWith("9000")))
                return null;

            var data = new List<string>();
            int flags = Convert.ToInt32(response.Substring(12, 2), 16);

            foreach (var element in Enum.GetValues(typeof(HidGlobal.OK.Readers.Components.HumanInterfaceFlags)))
            {
                if ((flags & Convert.ToInt32(element)) == 0)
                    continue;

                try
                {
                    var fieldinformation = element.GetType().GetField(element.ToString());
                    var fieldAttribiutes = (System.ComponentModel.DescriptionAttribute[])fieldinformation.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                    data.Add((fieldAttribiutes.Length > 0) ? fieldAttribiutes[0].Description : element.ToString());
                }
                catch (Exception error)
                {
                    log.Error(null, error);
                }
            }
            return data.ToArray();
        }
    }

    public class VendorName
    {
        public string GetApdu => "FF70076B08A206A004A0028F0000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD") && response.EndsWith("9000")))
                return null;

            byte[] data = Enumerable.Range(0, response.Length / 2).Select(x => Convert.ToByte(response.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(data, 4, data[3]);
        }
    }

    public class ExchangeLevel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string GetApdu => "FF70076B08A206A004A002910000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD039101") && response.EndsWith("9000")))
                return null;

            var output = string.Empty;

            int flags = Convert.ToInt32(response.Substring(8, 2), 16);

            foreach (var element in Enum.GetValues(typeof(HidGlobal.OK.Readers.Components.ExchangeLevelFlags)))
            {
                if ((flags & Convert.ToInt32(element)) != 0)
                {
                    try
                    {
                        var fieldinformation = element.GetType().GetField(element.ToString());
                        var fieldAttribiutes = (System.ComponentModel.DescriptionAttribute[])fieldinformation.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                        output = (fieldAttribiutes.Length > 0) ? fieldAttribiutes[0].Description : element.ToString();
                    }
                    catch (Exception error)
                    {
                        log.Error(null, error);
                    }
                }
            }
            return output;
        }
    }

    public class SerialNumber
    {
        public string GetApdu => "FF70076B08A206A004A002920000";
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD") && response.EndsWith("9000")))
                return null;

            byte[] data = Enumerable.Range(0, response.Length / 2).Select(x => Convert.ToByte(response.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(data, 4, data[3]);
        }
    }

    public class HfControllerType
    {
        public string GetApdu => "FF70076B08A206A004A002930000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD") && response.EndsWith("9000")))
                return null;

            byte[] data = Enumerable.Range(0, response.Length / 2).Select(x => Convert.ToByte(response.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(data, 4, data[3]);
        }
    }

    public class SizeOfUserEEPROM
    {
        public string GetApdu => "FF70076B08A206A004A002940000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD049402") && response.EndsWith("9000")))
                return null;

            return Convert.ToInt32(response.Substring(8, 4), 16).ToString() + " bytes";
        }
    }

    public class FirmwareLabel
    {
        public string GetApdu => "FF70076B08A206A004A002960000"; 
        public string TranslateResponse(string response)
        {
            response = response.Replace(" ", "");
            if (!(response.StartsWith("BD") && response.EndsWith("9000")))
                return null;

            byte[] data = Enumerable.Range(0, response.Length / 2).Select(x => Convert.ToByte(response.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(data, 4, data[3]);
        }
    }

}
