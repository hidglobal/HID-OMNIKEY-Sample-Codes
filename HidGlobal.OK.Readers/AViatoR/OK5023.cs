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
using HidGlobal.OK.Readers.AViatoR.Components;

namespace HidGlobal.OK.Readers.AViatoR
{
    public class OK5023
    {
        public const int MaxInputEscapeCommandData = 262;
        public const int MaxOutputEscapeCommandData = 464;
        public ReaderCapabilities ReaderCapabilities => new ReaderCapabilities();
        public ReaderConfigurationControl ReaderConfigurationControl => new ReaderConfigurationControl();
        public ReaderEeprom ReaderEeprom => new ReaderEeprom();
        public OK5023ContactlessSlotConfiguration ContactlessSlotConfiguration => new OK5023ContactlessSlotConfiguration();
        public ContactlessCardCommunicationV2 ContactlessCardCommunication => new ContactlessCardCommunicationV2();
    }

    public class OK5023ContactlessSlotConfiguration
    {
        public FelicaConfiguration FelicaConfiguration => new FelicaConfiguration();
        public Iso14443TypeAConfiguration Iso14443TypeAConfiguration => new Iso14443TypeAConfiguration();
        public Iso14443TypeBConfiguration Iso14443TypeBConfiguration => new Iso14443TypeBConfiguration();
        public Iso15693Configuration Iso15693Configuration => new Iso15693Configuration();
        public iClassConfiguration iClassConfiguration => new iClassConfiguration();
        public OK5023ContactlessCommon ContactlessCommon => new OK5023ContactlessCommon();
    }

    public class OK5023ContactlessCommon
    {
        public PollingSearchOrderConfig PollingSearchOrder => new PollingSearchOrderConfig();
        public EmdSuppresionEnable EmdSuppression => new EmdSuppresionEnable();
    }


}