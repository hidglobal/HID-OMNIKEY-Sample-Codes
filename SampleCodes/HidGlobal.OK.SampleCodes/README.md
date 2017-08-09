# HID OMNIKEY Smart Card Readers' Sample Codes

Sample codes for the OMNIKEY Smart Card Readers. Application contains simple examples of reader's functionalities based on the software developer guide.

## Getting Started

Sample codes are written in C# and developed for Windows. 

## Supported readers

* OMNIKEY 5022
* OMNIKEY 5422

## Functionality

* reader discovery based on the PCSC reader's name
* connects only to the supported readers
* OMNIKEY 5022
    * reader information
        * read 
    * contactless slot configuration
        * read
        * write  
    * user EEPROM
        *  read
        *  write
    * MIFARE Classic 1K/4K
        * load key 
        * read one block  
        * write one block 
    * iClass 
        * secure session example (requires keys)
* OMNIKEY 5422
    * reader information
        * read 
    * contactless slot configuration
        * read
        * write  
	* contact slot configuration
        * read
        * write  
    * user EEPROM
        *  read
        *  write
    * MIFARE Classic 1K/4K
        * load key 
        * read one block  
        * write one block 
    * iClass 
        * secure session example (requires keys)
	* synchronous card
		* L2
		* L3
		* I2C

## Prerequisites

.Net Framework 4.5.2

## Documentation

The code is well commented.
You can use doxygen to obtain documentation, or if you downlaoded the zip bundle from hidglobal.com open the index.html 
file attached to the samples in the "documentation" folder.

## License

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
