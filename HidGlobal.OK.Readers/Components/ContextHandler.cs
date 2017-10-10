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

namespace HidGlobal.OK.Readers.Components
{
    /// <summary>
    /// Object to manage an application context for PC/SC resource manager.
    /// </summary>
    public class ContextHandler : IContextHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Field containing WinSCard function context handle.
        /// </summary>
        private IntPtr _contextHandle;
        /// <summary>
        /// Variable indicating if context has been established.
        /// </summary>
        private bool _hasContext;
        /// <summary>
        /// Value of scope parameter used in previous call of <see cref="Establish(Scope)"/>.
        /// </summary>
        private Scope _previousScope;

        public System.Text.Encoding Encoding { get; set; }
      
        public ContextHandler()
        {
            _contextHandle = IntPtr.Zero;
            _hasContext = false;
            Encoding = System.Text.Encoding.ASCII;

            _previousScope = Scope.System;
        }

        /// <summary>
        /// Establish context handle for winscard resource manager.
        /// </summary>
        /// <param name="scope">Defines for the scope input parameter.</param>
        public void Establish(Scope scope)
        {
            if (_hasContext && IsValid())
                Release();

            IntPtr contextHandle = IntPtr.Zero;
            var retCode = WinSCard.EstablishContext(scope, out contextHandle);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
            {
                log.Error($"Establish failed\nError Code: {retCode}");
                _hasContext = false;
                return;
            }

            _contextHandle = contextHandle;
            _hasContext = true;
            _previousScope = scope;
            return;
        }
        /// <summary>
        /// Release context handle established by <see cref="Establish(Scope)"/>.
        /// </summary>
        public void Release()
        {
            if (!_hasContext)
            {
                // Nothing to release
                _contextHandle = IntPtr.Zero;
                return;
            }
            var contextHandle = _contextHandle;
            var retCode = WinSCard.ReleaseContext(contextHandle);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                log.Error("Release context failed");
            else
            {
                _hasContext = false;
                _contextHandle = IntPtr.Zero;
            }
        }
        /// <summary>
        /// Establish context handle with<see cref= "Scope" /> used in previous call of<see cref="Establish(Scope)"/>.
        /// </summary>
        public void ReEstablish() => Establish(_previousScope);
        /// <summary>
        /// Check if curently held context handle is valid.
        /// </summary>
        /// <returns>True if context handle is valid, false otherwise.</returns>
        public bool IsValid()
        {
            return CheckValidity() == ErrorCodes.SCARD_S_SUCCESS;
        }
        /// <summary>
        /// Checks validity of currently used context handle.
        /// </summary>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes CheckValidity()
        {
            return WinSCard.IsValidContext(_contextHandle);
        }
        /// <summary>
        /// Lists readers within given reader groups.
        /// </summary>
        /// <param name="readerGroups">Names of the reader groups</param>
        /// <returns>String array with names of active readers.</returns>
        public string[] ListReaders(string[] readerGroups)
        {
            var names = new string[0];
            var retCode = WinSCard.ListReaders(Handle, readerGroups, Encoding, out names);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                return null;
            return names;            
        }
        /// <summary>
        /// List all available readers.
        /// </summary>
        /// <returns>String array with names of active readers.</returns>
        public string[] ListReaders()
        {
            return ListReaders(null);
        }

        /// <summary>
        /// List reader groups existing in the system.
        /// </summary>
        /// <returns>String array with names of available reader groups.</returns>
        public string[] ListReaderGroups()
        {
            var groups = new string[0];
            var retCode = WinSCard.ListReaderGroups(Handle, Encoding, out groups);
            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                return null;
            return groups;
        }
        /// <summary>
        /// Introduce new reader group to the smart card resource manager.
        /// </summary>
        /// <remarks>Reader group will be created only after additon of first reader to it.</remarks>
        /// <param name="groupName">Name of the reader group.</param>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes IntroduceReaderGroup(string groupName)
        {
            return WinSCard.IntroduceReaderGroup(Handle, groupName, Encoding);
        }
        /// <summary>
        /// Deletes reader group from smart card resource manager.
        /// </summary>
        /// <param name="groupName">Name of the reader group.</param>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes ForgetReaderGroup(string groupName)
        {
            return WinSCard.ForgetReaderGroup(Handle, groupName, Encoding);
        }
        /// <summary>
        /// Add specified reader to the reader group. Group need to be introduced by <see cref="IntroduceReaderGroup(string)"/> beforehand.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes AddReaderToGroup(string readerName, string groupName)
        {
            return WinSCard.AddReaderToGroup(Handle, readerName, groupName, Encoding);
        }
        /// <summary>
        /// Removes reader form the group.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes RemoveReaderFromGroup(string readerName, string groupName)
        {
            return WinSCard.RemoveReaderFromGroup(Handle, readerName, groupName, Encoding);
        }
        /// <summary>
        /// Cancel blocking request of <see cref="GetStatusChange(int, ReaderState[])"/>.
        /// </summary>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes Cancel()
        {
            return WinSCard.Cancel(Handle);
        }
        /// <summary>
        /// Introduce additional alias of given device to the smart card resource manager.
        /// </summary>
        /// <param name="readerName">Additional name for the device.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes IntroduceReader(string readerName, string deviceName)
        {
            return WinSCard.IntroduceReader(Handle, readerName, deviceName, Encoding);
        }
        /// <summary>
        /// Removes reader from the smart card resource manager.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes ForgetReader(string readerName)
        {
            return WinSCard.ForgetReader(Handle, readerName, Encoding);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="readerStates"></param>
        /// <returns><see cref="ErrorCodes"/></returns>
        public ErrorCodes GetStatusChange(int timeout, ref ReaderState[] readerStates)
        {
            return WinSCard.GetStatusChange(Handle, timeout, ref readerStates, readerStates.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readers"></param>
        /// <returns><see cref="ReaderState"/></returns>
        public ReaderState[] GetReaderState(string[] readers)
        {
            var status = new ReaderState[readers.Length];
            for (int i = 0; i < readers.Length; i++)
            {
                status[i] = new ReaderState
                {
                    ReaderName = readers[i],
                    CurrentState = (int)ReaderStates.Unaware,
                    AtrLength = 36,
                    Atr = new byte[36],
                };
            }
            GetStatusChange(100, ref status);
            return status;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns><see cref="ReaderState"/></returns>
        public ReaderState GetReaderState(string reader)
        {
            return GetReaderState(new string[] {reader})[0];
        }

        /// <summary>
        /// Retrives the context handle.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                if (!IsValid())
                {
                    ReEstablish();
                }

                return _contextHandle;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Release();
                disposedValue = true;
            }
        }

        ~ContextHandler()
        {
           Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
