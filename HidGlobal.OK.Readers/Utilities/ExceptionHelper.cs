using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace HidGlobal.OK.Readers.Utilities
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Initialized new instance of exception appropriate for provided native error code.
        /// </summary>
        /// <param name="nativeErrorCode">Native function error code associated with created exception.</param>
        /// <returns>New instance of object derived from <see cref="ExternalException"/>.</returns>
        public static ExternalException PrepareException(int nativeErrorCode) => new Win32Exception(nativeErrorCode);

        /// <summary>
        /// Initialized new instance of exception appropriate for provided native error code.
        /// </summary>
        /// <param name="nativeErrorCode">Native function error code associated with created exception.</param>
        /// <param name="source">Name of the application or object that caused the error.</param>
        /// <returns>New instance of object derived from <see cref="ExternalException"/>.</returns>
        public static ExternalException PrepareException(int nativeErrorCode, string source)
        {
            var exception = PrepareException(nativeErrorCode);
            exception.Source = source;

            return exception;
        }
        
        /// <summary>
        /// Compares if supplied exception is related to given error code.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="errorCode"></param>
        /// <returns>True if supplied exception error code is equal to provided error code.</returns>
        public static bool ErrorCodeEquals(this Exception exception, uint errorCode)
        {
            bool isEqual;
            switch (exception)
            {
                case Win32Exception win32Exception:
                    isEqual = errorCode.Equals((uint) win32Exception.NativeErrorCode);
                    break;

                case ExternalException externalException:
                    isEqual = errorCode.Equals((uint)externalException.ErrorCode);
                    break;

                default:
                    isEqual = errorCode.Equals((uint)exception.HResult);
                    break;
            }
            return isEqual;
        }
    }
}
