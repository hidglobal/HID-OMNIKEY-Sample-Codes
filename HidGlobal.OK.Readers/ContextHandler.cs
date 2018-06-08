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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.Readers.Utilities;
using HidGlobal.OK.Readers.WinSCard;

namespace HidGlobal.OK.Readers
{
    /// <summary>
    /// Object to manage an application context for PC/SC resource manager.
    /// </summary>
    public class ContextHandler : IContextHandler
    {
        private static IContextHandler _instance;

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

        public static IContextHandler Instance => _instance ?? (_instance = new ContextHandler());

        private ContextHandler()
        {
            _hasContext = false;
            _contextHandle = IntPtr.Zero;
            _previousScope = Scope.System;
            Encoding = System.Text.Encoding.ASCII;
            _disposedValue = false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Establish context handle for winscard resource manager.
        /// </summary>
        /// <param name="scope">Defines for the scope input parameter.</param>
        public void Establish(Scope scope)
        {
            if (_hasContext && IsValid())
                Release();
            
            _contextHandle = WinSCardWrapper.EstablishContext(scope);

            _hasContext = true;
            _previousScope = scope;
        }

        /// <inheritdoc />
        /// <summary>
        /// Release context handle established by <see cref="M:HidGlobal.OK.Readers.ContextHandler.Establish(HidGlobal.OK.Readers.Components.Scope)" />.
        /// </summary>
        public void Release()
        {
            if (!_hasContext) return;

            WinSCardWrapper.ReleaseContext(_contextHandle);

            _hasContext = false;
            _contextHandle = IntPtr.Zero;
        }
        /// <inheritdoc />
        /// <summary>
        /// Establish context handle with<see cref="T:HidGlobal.OK.Readers.Components.Scope" /> used in previous call of<see cref="M:HidGlobal.OK.Readers.ContextHandler.Establish(HidGlobal.OK.Readers.Components.Scope)" />.
        /// </summary>
        public void ReEstablish() => Establish(_previousScope);
        
        /// <inheritdoc />
        /// <summary>
        /// Check if curently held context handle is valid.
        /// </summary>
        /// <returns>True if context handle is valid, false otherwise.</returns>
        public bool IsValid()
        {
            var isValid = true;
            try
            {
                WinSCardWrapper.IsValidContext(_contextHandle);
            }
            catch
            {
                // exception Ignored
                isValid = false;
            }
            return isValid;
        }

        
        /// <inheritdoc />
        /// <summary>
        /// Lists readers within given reader groups.
        /// </summary>
        /// <param name="readerGroups">Names of the reader groups</param>
        /// <returns>String array with names of active readers.</returns>
        public IReadOnlyList<string> ListReaders(IReadOnlyList<string> readerGroups)
        {
            byte[] multiNullTerminatedBytesArray;

            try
            {

                var readerGroupsBytes = readerGroups == null 
                    ? null
                    : BinaryHelper.ConvertMultiNullTerminatedByteArrayFromStringEnumerable(Encoding, readerGroups)
                                  .ToArray();

                multiNullTerminatedBytesArray = WinSCardWrapper.ListReaders(Handle, readerGroupsBytes);
            }
            catch (Win32Exception e) when (e.ErrorCodeEquals(0x8010002E)) // 0x8010002E -> SCARD_E_NO_READERS_AVAILABLE
            {
                multiNullTerminatedBytesArray = new byte [0];
            }

            return (IReadOnlyList<string>)BinaryHelper.ConvertMultiNullTerminatedStringFromBytesToStringArray(Encoding,
                multiNullTerminatedBytesArray);
        }

        /// <inheritdoc />
        /// <summary>
        /// List all available readers.
        /// </summary>
        /// <returns>String array with names of active readers.</returns>
        public IReadOnlyList<string> ListReaders() => ListReaders(null);

        /// <inheritdoc />
        /// <summary>
        /// List reader groups existing in the system.
        /// </summary>
        /// <returns>String array with names of available reader groups.</returns>
        public IReadOnlyList<string> ListReaderGroups()
        {
            var multiNullTerminatedBytesArray = WinSCardWrapper.ListReaderGroups(Handle);

            return (IReadOnlyList<string>)BinaryHelper.ConvertMultiNullTerminatedStringFromBytesToStringArray(Encoding,
                multiNullTerminatedBytesArray);
        }

        /// <inheritdoc />
        /// <summary>
        /// Introduce new reader group to the smart card resource manager.
        /// </summary>
        /// <remarks>Reader group will be created only after additon of first reader to it.</remarks>
        /// <param name="groupName">Name of the reader group.</param>
        public void IntroduceReaderGroup(string groupName)
        {
            var nullTerminatedBytesArray = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, groupName).ToArray();

            WinSCardWrapper.IntroduceReaderGroup(Handle, nullTerminatedBytesArray);
        }

        /// <inheritdoc />
        /// <summary>
        /// Deletes reader group from smart card resource manager.
        /// </summary>
        /// <param name="groupName">Name of the reader group.</param>
        public void ForgetReaderGroup(string groupName)
        {
            var nullTerminatedBytesArray = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, groupName).ToArray();

            WinSCardWrapper.ForgetReaderGroup(Handle, nullTerminatedBytesArray);
        }

        /// <inheritdoc />
        /// <summary>
        /// Add specified reader to the reader group. Group need to be introduced by <see cref="M:HidGlobal.OK.Readers.ContextHandler.IntroduceReaderGroup(System.String)" /> beforehand.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <param name="groupName">Name of the group.</param>
        public void AddReaderToGroup(string readerName, string groupName)
        {
            var nullTerminatedBytesArray1 = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, readerName).ToArray();
            var nullTerminatedBytesArray2 = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, groupName).ToArray();

            WinSCardWrapper.AddReaderToGroup(Handle, nullTerminatedBytesArray1, nullTerminatedBytesArray2);
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes reader form the group.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <param name="groupName">Name of the group.</param>
        public void RemoveReaderFromGroup(string readerName, string groupName)
        {
            var nullTerminatedBytesArray1 = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, readerName).ToArray();
            var nullTerminatedBytesArray2 = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, groupName).ToArray();

            WinSCardWrapper.RemoveReaderFromGroup(Handle, nullTerminatedBytesArray1, nullTerminatedBytesArray2);
        }

        /// <inheritdoc />
        /// <summary>
        /// Cancel blocking request of <see cref="M:HidGlobal.OK.Readers.ContextHandler.GetStatusChange(System.Int32,HidGlobal.OK.Readers.Components.ReaderState[]@)" />.
        /// </summary>
        public void Cancel()
        {
            WinSCardWrapper.Cancel(Handle);
        }

        /// <inheritdoc />
        /// <summary>
        /// Introduce additional alias of given device to the smart card resource manager.
        /// </summary>
        /// <param name="readerName">Additional name for the device.</param>
        /// <param name="deviceName">Name of the device.</param>
        public void IntroduceReader(string readerName, string deviceName)
        {
            var nullTerminatedBytesArray1 = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, readerName).ToArray();
            var nullTerminatedBytesArray2 = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, deviceName).ToArray();

            WinSCardWrapper.IntroduceReader(Handle, nullTerminatedBytesArray1, nullTerminatedBytesArray2);
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Removes reader from the smart card resource manager.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        public void ForgetReader(string readerName)
        {
            var nullTerminatedBytesArray1 = BinaryHelper.ConvertNullTerminatedByteArrayFromString(Encoding, readerName).ToArray();

            WinSCardWrapper.ForgetReader(Handle, nullTerminatedBytesArray1);
        }
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="readerStates"></param>
        public IReadOnlyList<ReaderState> GetStatusChange(int timeout, IReadOnlyList<ReaderState> readerStates)
        {
            var temporaryStructArray = readerStates.ToArray();

            WinSCardWrapper.GetStatusChange(Handle, timeout, ref temporaryStructArray, temporaryStructArray.Length);

            return temporaryStructArray;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="readerNamesWithStatesDictionary"></param>
        /// <returns><see cref="T:HidGlobal.OK.Readers.Components.ReaderState" /></returns>
        public IReadOnlyList<ReaderState> GetReaderState(IReadOnlyDictionary<string, ReaderStates> readerNamesWithStatesDictionary)
        {
            var list = new List<ReaderState>(readerNamesWithStatesDictionary.Count);
            list.AddRange(readerNamesWithStatesDictionary.Select(keyValuePair => new ReaderState
            {
                Atr = new byte[36],
                AtrLength = 36,
                CurrentState = (int) keyValuePair.Value,
                ReaderName = keyValuePair.Key,
            }));

            return GetStatusChange(1000, list);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="stateOfTheReader"></param>
        /// <returns><see cref="T:HidGlobal.OK.Readers.Components.ReaderState" /></returns>
        public ReaderState GetReaderState(string reader, ReaderStates stateOfTheReader)
        {
            return GetReaderState(new Dictionary<string, ReaderStates> {{reader, stateOfTheReader}})[0];
        }

        /// <inheritdoc />
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

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            Release();
            _disposedValue = true;
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
