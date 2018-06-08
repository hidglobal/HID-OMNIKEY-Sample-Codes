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
    public class OK5422
    {
        public const int MaxInputEscapeCommandData = 262;
        public const int MaxOutputEscapeCommandData = 464;
        public ReaderCapabilities ReaderCapabilities => new ReaderCapabilities(); 
        public ContactSlotConfiguration ContactSlotConfiguration => new ContactSlotConfiguration();
        public ContactCardCommunication ContactCardCommunication => new ContactCardCommunication();
        public OK5422ContactlessSlotConfiguration ContactlessSlotConfiguration => new OK5422ContactlessSlotConfiguration();
        public ReaderEeprom ReaderEeprom => new ReaderEeprom();
        public ReaderConfigurationControl ReaderConfigurationControl => new ReaderConfigurationControl();
        public ContactlessCardCommunication ContactlessCardCommunication => new ContactlessCardCommunication();
    }

   
    public class OK5422ContactlessSlotConfiguration
    {
        public Iso14443TypeAConfiguration Iso14443TypeAConfiguration => new Iso14443TypeAConfiguration();
        public Iso14443TypeBConfiguration Iso14443TypeBConfiguration => new Iso14443TypeBConfiguration();
        public iClassConfiguration iClassConfiguration => new iClassConfiguration();
        public OK5422ContactlessCommon ContactlessCommon => new OK5422ContactlessCommon();
    }

    public class OK5422ContactlessCommon
    {
        public SleepModePollingFrequency SleepModePollingFrequency => new SleepModePollingFrequency();
        public SleepModeCardDetectionEnable SleepModeCardDetection => new SleepModeCardDetectionEnable();
        public EmdSuppresionEnable EmdSuppression => new EmdSuppresionEnable();
        public PollingRFModuleEnable PollingRFModule => new PollingRFModuleEnable();
    }

}
