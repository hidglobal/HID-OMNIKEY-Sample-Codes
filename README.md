# HID OMNIKEY Smart Card Readers' Sample Codes

Sample codes for the OMNIKEY Smart Card Readers. Application contains simple examples of reader's functionalities based on the software developer guide.

## Getting Started

Sample codes are written in C# and developed for Windows. 

## Supported readers

* OMNIKEY 5022
* OMNIKEY 5023
* OMNIKEY 5027
* OMNIKEY 5122
* OMNIKEY 5422

## Functionality

* reader discovery based on the PCSC reader's name
* connects only to the supported readers
* OMNIKEY 5022
    * Configuration
       * Read
       * Write
       * Display
    * User EEPROM
       * Read
       * Write
    * ISO/IEC 14443 Type A
       * MIFARE Classic 1K/4K Example
           * Read one block
           * Write one block
           * Increment value type block
           * Decrement value type block
           * Load key example
    * iClass 2ks/16k example with secure session (requires keys)
       * Load key example
       * Read Binary
       * Update Binary
    * Seos example
       * get UID
    * ISO 15693
       * Read one block
       * Update one block
* OMNIKEY 5023
    * Configuration
       * Read
       * Write
       * Display
    * User EEPROM
       * Read
       * Write
    * ISO/IEC 14443 Type A
       * MIFARE Classic 1K/4K Example
           * Read one block
           * Write one block
           * Increment value type block
           * Decrement value type block
           * Load key example
    * iClass 2ks/16k example with secure session (requires keys)
       * Load key example
       * Read Binary
       * Update Binary
    * Secure Processor Examples
	   * Read PACS data
	   * Desfire example 
* OMNIKEY 5027
    * Configuration
       * Read
       * Write
       * Display
    * User EEPROM
       * Read
       * Write
* OMNIKEY 5422 / 5122
    * Configuration
        * Read
        * Write
        * Display
    * User EEPROM
        * Read
        * Write
    *ISO/IEC 14443 Type A
        * MIFARE Classic 1K/4K Example
            * Read one block
            * Write one block
            * Increment value type block
            * Decrement value type block
            * Load key example
    * iClass 2ks/16k example with secure session
        * Load key example
        * Read Binary
        * Update Binary
    * Seos example
        * Get UID
    * Synchronous card example
        * 2WBP
        * 3WBP
        * I2C

## Release Notes
* v1.6.0
	* Added support for OMNIKEY 5027 reader
* v1.5.0
	* Added support for OMNIKEY 5023 reader
	* Added support for OMNIKEY 5122 reader
	* Refactored secure session examples

## Prerequisites

.Net Framework 4.5.2

## Documentation

The code is well commented.
You can use doxygen to obtain documentation, or if you downlaoded the zip bundle from hidglobal.com open the index.html 
file attached to the samples in the "documentation" folder.

## License

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
