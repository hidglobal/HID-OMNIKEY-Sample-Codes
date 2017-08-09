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
using System.ComponentModel;

namespace HidGlobal.OK.Readers.Components
{
    public enum ErrorCodes : int
    {
        /// <summary>
        /// The client attempted a smart card operation in a remote session, such as a client session running on a terminal server, and the operating system in use does not support smart card redirection.
        /// </summary>
        [Description("The client attempted a smart card operation in a remote session, such as a client session running on a terminal server, and the operating system in use does not support smart card redirection.")]
        ERROR_BROKEN_PIPE                           = 0x00000109,

        /// <summary>
        /// An error occurred in setting the smart card file object pointer.
        /// </summary>
        [Description("An error occurred in setting the smart card file object pointer.")]
        SCARD_E_BAD_SEEK                            = unchecked((int)0x80100029),

        /// <summary>
        /// The action was canceled by an <see cref="WinSCard.SCardCancel(IntPtr)"/> request.
        /// </summary>
        [Description("The action was canceled by an SCardCancel request.")]
        SCARD_E_CANCELLED                           = unchecked((int)0x80100002),

        /// <summary>
        /// The system could not dispose of the media in the requested manner.
        /// </summary>
        [Description("The system could not dispose of the media in the requested manner.")]
        SCARD_E_CANT_DISPOSE                        = unchecked((int)0x8010000E),

        /// <summary>
        /// The Smart card does not meet minimal requirements for support.
        /// </summary>
        [Description("The Smart card does not meet minimal requirements for support.")]
        SCARD_E_CARD_UNSUPPORTED                    = unchecked((int)0x8010001C),

        /// <summary>
        /// The requested certificate could not be obtained.
        /// </summary>
        [Description("The requested certificate could not be obtained.")]
        SCARD_E_CERTIFICATE_UNAVALIBLE              = unchecked((int)0x8010002D),

        /// <summary>
        /// A communications error with the smart card has been detected.
        /// </summary>
        [Description("A communications error with the smart card has been detected.")]
        SCARD_E_COMM_DATA_LOST                      = unchecked((int)0x8010002F),

        /// <summary>
        /// The specified directory does not exist in the smart card.
        /// </summary>
        [Description("The specified directory does not exist in the smart card.")]
        SCARD_E_DIR_NOT_FOUND                       = unchecked((int)0x80100023),

        /// <summary>
        /// The reader driver did not produce unique reader name.
        /// </summary>
        [Description("The reader driver did not produce unique reader name.")]
        SCARD_E_DUPLICATE_READER                    = unchecked((int)0x8010001B),

        /// <summary>
        /// The specified file does not exist in the smart card.
        /// </summary>
        [Description("The specified file does not exist in the smart card.")]
        SCARD_E_FILE_NOT_FOUND                      = unchecked((int)0x80100024),

        /// <summary>
        /// The requested order of object creation is not supported.
        /// </summary>
        [Description("The requested order of object creation is not supported.")]
        SCARD_E_ICC_CREATEORDER                     = unchecked((int)0x80100021),

        /// <summary>
        /// No primary provider can be found for the smart card.
        /// </summary>
        [Description("No primary provider can be found for the smart card.")]
        SCARD_E_ICC_INSTALLATION                    = unchecked((int)0x80100020),

        /// <summary>
        /// The data buffer for returned data is too small for the returned data.
        /// </summary>
        [Description("The data buffer for returned data is too small for the returned data.")]
        SCARD_E_INSUFFICIENT_BUFFER                 = unchecked((int)0x80100008),

        /// <summary>
        /// An ATR string obtained from the registry is not a valid ATR string.
        /// </summary>
        [Description("An ATR string obtained from the registry is not a valid ATR string.")]
        SCARD_E_INVALID_ATR                         = unchecked((int)0x80100015),

        /// <summary>
        /// The supplied PIN is incorrect.
        /// </summary>
        [Description("The supplied PIN is incorrect.")]
        SCARD_E_INVALID_CHV                         = unchecked((int)0x8010002A),

        /// <summary>
        /// The supplied handle was not valid.
        /// </summary>
        [Description("The supplied handle was not valid.")]
        SCARD_E_INVALID_HANDLE                      = unchecked((int)0x80100003),

        /// <summary>
        /// One or more of the supplied parameters could not be properly interpreted.
        /// </summary>
        [Description("One or more of the supplied parameters could not be properly interpreted.")]
        SCARD_E_INVALID_PARAMETER                   = unchecked((int)0x80100004),

        /// <summary>
        /// Registry startup information is missing or not valid.
        /// </summary>
        [Description("Registry startup information is missing or not valid.")]
        SCARD_E_INVALID_TARGET                      = unchecked((int)0x80100005),

        /// <summary>
        /// One or more of the supplied parameter values could not be properly interpreted.
        /// </summary>
        [Description("One or more of the supplied parameter values could not be properly interpreted.")]
        SCARD_E_INVALID_VALUE                       = unchecked((int)0x80100011),

        /// <summary>
        /// Access is denied to the file.
        /// </summary>
        [Description("Access is denied to the file.")]
        SCARD_E_NO_ACCESS                           = unchecked((int)0x80100027),

        /// <summary>
        /// The supplied path does not represent a smart card directory.
        /// </summary>
        [Description("The supplied path does not represent a smart card directory.")]
        SCARD_E_NO_DIR                              = unchecked((int)0x80100025),

        /// <summary>
        /// The supplied path does not represent a smart card file.
        /// </summary>
        [Description("The supplied path does not represent a smart card file.")]
        SCARD_E_NO_FILE                             = unchecked((int)0x80100026),

        /// <summary>
        /// The requested key container does not exist on the smart card.
        /// </summary>
        [Description("The requested key container does not exist on the smart card.")]
        SCARD_E_NO_KEY_CONTAINER                    = unchecked((int)0x80100030),

        /// <summary>
        /// Not enough memory avalible to complete this command.
        /// </summary>
        [Description("Not enough memory avalible to complete this command.")]
        SCARD_E_NO_MEMORY                           = unchecked((int)0x80100006),

        /// <summary>
        /// The smart card PIN cannot be cached.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This error code is not avalible.
        /// </summary>
        [Description("The smart card PIN cannot be cached.")]
        SCARD_E_NO_PIN_CACHE                        = unchecked((int)0x80100033),

        /// <summary>
        /// No smart card reader avalible.
        /// </summary>
        [Description("No smart card reader avalible.")]
        SCARD_E_NO_READERS_AVALIBLE                 = unchecked((int)0x8010002E),

        /// <summary>
        /// The smart card resource manager is not running.
        /// </summary>
        [Description("The smart card resource manager is not running.")]
        SCARD_E_NO_SERVICE                          = unchecked((int)0x8010001D),

        /// <summary>
        /// The operation requires a smart card, but no smart card is currently in the device.
        /// </summary>
        [Description("The operation requires a smart card, but no smart card is currently in the device.")]
        SCARD_E_NO_SMARTCARD                        = unchecked((int)0x8010000C),

        /// <summary>
        /// The required certificate does not exist.
        /// </summary>
        [Description("The required certificate does not exist.")]
        SCARD_E_NO_SUCH_CERTIFICATE                 = unchecked((int)0x8010002C),

        /// <summary>
        /// The reader or card is not ready to accept commands.
        /// </summary>
        [Description("The reader or card is not ready to accept commands.")]
        SCARD_E_NOT_READY                           = unchecked((int)0x80100010),

        /// <summary>
        /// An attempt was made to end a nonexistent transaction.
        /// </summary>
        [Description("An attempt was made to end a nonexistent transaction.")]
        SCARD_E_NOT_TRANSACTED                      = unchecked((int)0x80100016),

        /// <summary>
        /// The PCI recive buffer was to small.
        /// </summary>
        [Description("The PCI recive buffer was to small.")]
        SCARD_E_PCI_TOO_SMALL                       = unchecked((int)0x80100019),

        /// <summary>
        /// The smart card PIN cache has expired.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This error code is not avalible.
        /// </summary>
        [Description("The smart card PIN cache has expired.")]
        SCARD_E_PIN_CACHE_EXPIRED                   = unchecked((int)0x80100032),

        /// <summary>
        /// The requested protocols are incompatibile with the protocol currently in use with the card.
        /// </summary>
        [Description("The requested protocols are incompatibile with the protocol currently in use with the card.")]
        SCARD_E_PROTO_MISMATCH                      = unchecked((int)0x8010000F),

        /// <summary>
        /// The smart card is read-only and cannotbe written to.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This error code is not avalible.
        /// </summary>
        [Description("The smart card is read-only and cannotbe written to.")]
        SCARD_E_READ_ONLY_CARD                      = unchecked((int)0x80100034),

        /// <summary>
        /// The specified reader is not currently available for use.
        /// </summary>
        [Description("The specified reader is not currently available for use.")]
        SCARD_E_READER_UNAVALIBLE                   = unchecked((int)0x80100017),

        /// <summary>
        /// The reader driver does not meet minimal requirements for support.
        /// </summary>
        [Description("The reader driver does not meet minimal requirements for support.")]
        SCARD_E_READER_UNSUPPORTED                  = unchecked((int)0x8010001A),

        /// <summary>
        /// The smart card resource manager is too busy to complete this operation.
        /// </summary>
        [Description("The smart card resource manager is too busy to complete this operation.")]
        SCARD_E_SERVER_TO_BUSY                      = unchecked((int)0x80100031),

        /// <summary>
        /// The smart card resource manager has shut down.
        /// </summary>
        [Description("The smart card resource manager has shut down.")]
        SCARD_E_SERVICE_STOPPERD                    = unchecked((int)0x8010001E),

        /// <summary>
        /// The smart card cannot be accessed because of other outstanding connections.
        /// </summary>
        [Description("The smart card cannot be accessed because of other outstanding connections.")]
        SCARD_E_SHARING_VIOLATION                   = unchecked((int)0x8010000B),

        /// <summary>
        /// The action was canceled by the system, presumbly to log off or shut down.
        /// </summary>
        [Description("The action was canceled by the system, presumbly to log off or shut down.")]
        SCARD_E_SYSTEM_CANCELLED                    = unchecked((int)0x80100012),

        /// <summary>
        /// The user-specified time-out value expired.
        /// </summary>
        [Description("The user-specified time-out value expired.")]
        SCARD_E_TMEOUT                              = unchecked((int)0x8010000A),

        /// <summary>
        /// An unexpected card error has occured.
        /// </summary>
        [Description("An unexpected card error has occured.")]
        SCARD_E_UNEXPECTED                          = unchecked((int)0x8010001F),

        /// <summary>
        /// The specified smart card nameis not recognized.
        /// </summary>
        [Description("The specified smart card nameis not recognized.")]
        SCARD_E_UNKNOWN_CARD                        = unchecked((int)0x8010000D),

        /// <summary>
        /// The specified reader name is not recognized.
        /// </summary>
        [Description("The specified reader name is not recognized.")]
        SCARD_E_UNKNOWN_READER                      = unchecked((int)0x80100009),

        /// <summary>
        /// An unrecognized error code was returned.
        /// </summary>
        [Description("An unrecognized error code was returned.")]
        SCARD_E_UNKNOWN_RES_MNG                     = unchecked((int)0x8010002B),

        /// <summary>
        /// This smart card does not support the requested feature.
        /// </summary>
        [Description("This smart card does not support the requested feature.")]
        SCARD_E_UNSUPPORTED_FEATURE                 = unchecked((int)0x80100022),

        /// <summary>
        /// An attempt was made to write more data than would fit in the target object.
        /// </summary>
        [Description("An attempt was made to write more data than would fit in the target object.")]
        SCARD_E_WRITE_TOO_MANY                      = unchecked((int)0x80100028),

        /// <summary>
        /// An internal communications error has been detected.
        /// </summary>
        [Description("An internal communications error has been detected.")]
        SCARD_F_COMM_ERROR                          = unchecked((int)0x80100013),

        /// <summary>
        /// An internal consistency check failed.
        /// </summary>
        [Description("An internal consistency check failed.")]
        SCARD_F_INTERNAL_ERROR                      = unchecked((int)0x80100001),

        /// <summary>
        /// An internal error has been detected, but the source is unknown.
        /// </summary>
        [Description("An internal error has been detected, but the source is unknown.")]
        SCARD_F_UNKNOWN_ERROR                       = unchecked((int)0x80100014),

        /// <summary>
        /// An internal consistency timer has expired.
        /// </summary>
        [Description("An internal consistency timer has expired.")]
        SCARD_F_WAITED_TOO_LONG                     = unchecked((int)0x80100007),

        /// <summary>
        /// The operation has been aborted to allow the server application to exit.
        /// </summary>
        [Description("The operation has been aborted to allow the server application to exit.")]
        SCARD_P_SHUTDOWN                            = unchecked((int)0x80100018),

        /// <summary>
        /// No error was encountered.
        /// </summary>
        [Description("No error was encountered.")]
        SCARD_S_SUCCESS                             = 0x00000000,
        
        /// <summary>
        /// A call to the unknown function. Retuned when trying to send escape command to reader without prior enabling escape commands.
        /// </summary>
        [Description("A call to the unknown function. Retuned when trying to send escape command to reader without prior enabling escape commands.")]
        ERROR_INVALID_FUNCTION                      = 0x00000001,

        /// <summary>
        /// The action was canceled by the user.
        /// </summary>
        [Description("The action was canceled by the user.")]
        SCARD_W_CANCELLED_BY_USER                   = unchecked((int)0x8010006E),

        /// <summary>
        /// The requested item could not be found in the cache.
        /// </summary>
        [Description("The requested item could not be found in the cache.")]
        SCARD_W_CACHE_ITEM_NOT_FOUND                = unchecked((int)0x80100070),

        /// <summary>
        /// The requested cache item is too old and was deleted from the cache.
        /// </summary>
        [Description("The requested cache item is too old and was deleted from the cache.")]
        SCARD_W_CACHE_ITEM_STALE                    = unchecked((int)0x80100071),

        /// <summary>
        /// The new cache item exceeds the maximum per-item size defined for the cache.
        /// </summary>
        [Description("The new cache item exceeds the maximum per-item size defined for the cache.")]
        SCARD_W_CACHE_ITEM_TOO_BIG                  = unchecked((int)0x80100072),

        /// <summary>
        /// No PIN was presented to the smart card.
        /// </summary>
        [Description(" No PIN was presented to the smart card.")]
        SCARD_W_CARD_NOT_AUTHENTICATED              = unchecked((int)0x8010006F),

        /// <summary>
        /// The card cannot be accessed because the maximum number of PIN entry attempts has been reached.
        /// </summary>
        [Description("The card cannot be accessed because the maximum number of PIN entry attempts has been reached.")]
        SCARD_W_CHV_BLOCKED                         = unchecked((int)0x8010006C),

        /// <summary>
        /// The end of the smart card file has been reached.
        /// </summary>
        [Description("The end of the smart card file has been reached.")]
        SCARD_W_EOF                                 = unchecked((int)0x8010006D),

        /// <summary>
        /// The smart card has been removed, so further communication is not possible.
        /// </summary>
        [Description("The smart card has been removed, so further communication is not possible.")]
        SCARD_W_REMOVED_CARD                        = unchecked((int)0x80100069),

        /// <summary>
        /// The smart card was reset.
        /// </summary>
        [Description("The smart card was reset.")]
        SCARD_W_RESET_CARD                          = unchecked((int)0x80100068),

        /// <summary>
        /// Access was denied because of security violation.
        /// </summary>
        [Description("Access was denied because of security violation.")]
        SCARD_W_SECUTIRY_VIOLATION                  = unchecked((int)0x8010006A),

        /// <summary>
        /// Power has been removed from the smart card, so that further communication is not possible.
        /// </summary>
        [Description("Power has been removed from the smart card, so that further communication is not possible.")]
        SCARD_W_UNPOWERED_CARD                      = unchecked((int)0x80010067),

        /// <summary>
        /// The smart card is not respondsive to a reset.
        /// </summary>
        [Description("The smart card is not responsive to a reset.")]
        SCARD_W_UNRESPONSIVE_CARD                   = unchecked((int)0x80100066),

        /// <summary>
        /// The reader cannot communicate with the card, due to ATR string configuration conflicts.
        /// </summary>
        [Description("The reader cannot communicate with the card, due to ATR string configuration conflicts.")]
        SCARD_W_UNSUUPPORTED_CARD                   = unchecked((int)0x80100065),

        /// <summary>
        /// The card cannot be accessed because the wrong PIN was presented.
        /// </summary>
        [Description("The card cannot be accessed because the wrong PIN was presented.")]
        SCARD_W_WRONG_CHV                           = unchecked((int)0x8010006B)
    };
}
