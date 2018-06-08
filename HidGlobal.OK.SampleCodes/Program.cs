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

using HidGlobal.OK.SampleCodes.MenuSections;

namespace HidGlobal.OK.SampleCodes
{
    static class Program
    {
        private static IMenuItem _rootMenu = new MenuItem("HID OMNIKEY Smart Card Readers' Sample Codes Application Menu", true);

        private static IMenuSection _keyboardWedgesSection = new KeyboardWedgesMenuSection(KeyboardWedgesMenuFactory.Instance);
        private static IMenuSection _smartCardReadersSection = new SmartCardReadersMenuSection(SmartCardReadersMenuFactory.Instance);

        private static int Main(string[] args)
        {
            _rootMenu.AddSubItem(_smartCardReadersSection.RootMenuItem);
            _rootMenu.AddSubItem(_keyboardWedgesSection.RootMenuItem);

            _rootMenu.Execute();
            
            return 0;
        }
    }
}
