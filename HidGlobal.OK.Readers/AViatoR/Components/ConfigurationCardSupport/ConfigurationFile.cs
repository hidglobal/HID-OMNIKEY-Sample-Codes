using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.AViatoR.Components.ConfigurationCardSupport
{
    internal interface IConfigurationFile : IDisposable
    {
        int MaxFileSize { get; }

        /// <summary>
        /// Writes data to a config file structure.
        /// </summary>
        /// <param name="data"> Data to be written.</param>
        /// <returns>True if successful, false if data size is larger then free space available in a config file structure.</returns>
        bool WriteData(ICollection<byte> data);

        /// <summary>
        /// Use to retrive data bytes form config file data object.
        /// </summary>
        /// <returns>Collection of bytes representing config file data.</returns>
        ICollection<byte> GetFileContent();
    };

    internal abstract class ConfigurationFile : IConfigurationFile
    {
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly ICollection<byte> _productIdentifierBytes;
        protected int FileTemplateSize;
        public abstract int MaxFileSize { get; }

        protected ConfigurationFile(ICollection<byte> productIdentifier)
        {
            _productIdentifierBytes = productIdentifier ?? throw new ArgumentNullException(nameof(productIdentifier));
            _hashAlgorithm = new CrcX25Algorithm();

            FileTemplateSize = _productIdentifierBytes.Count + 4; // 4 -> Length Field + CRC
        }

        protected abstract byte[] GetData();

        public abstract bool WriteData(ICollection<byte> data);

        public ICollection<byte> GetFileContent()
        {
            var fileContent = new List<byte>(MaxFileSize);

            var data = GetData();
            var crc = _hashAlgorithm.ComputeHash(data);

            fileContent.AddRange(_productIdentifierBytes);
            fileContent.AddRange(BinaryHelper.ConvertToBytes((ushort) (data.Length + crc.Length),
                BinaryHelper.ByteOrder.LsbFirst));

            fileContent.AddRange(data);
            fileContent.AddRange(crc);

            return fileContent;
        }

        public virtual void Dispose()
        {
            _hashAlgorithm.Dispose();
        }
    };

    internal sealed class ConfigurationHeaderFile : ConfigurationFile
    {
        private readonly byte[] _dataBytes;

        public byte VersionMajor
        {
            get => _dataBytes[0];
            private set => _dataBytes[0] = value;
        }

        public byte VersionMinor
        {
            get => _dataBytes[1];
            private set => _dataBytes[1] = value;
        }

        public byte Rfu0
        {
            get => _dataBytes[2];
            set => _dataBytes[2] = value;
        }

        public byte Rfu1
        {
            get => _dataBytes[3];
            set => _dataBytes[3] = value;
        }

        public override int MaxFileSize { get; } = 10;

        public ConfigurationHeaderFile(ICollection<byte> productIdentifier, byte versionMajor, byte versionMinor) :
            base(productIdentifier)
        {
            _dataBytes = new byte[4];
            VersionMajor = versionMajor;
            VersionMinor = versionMinor;
        }

        public override bool WriteData(ICollection<byte> data)
        {
            if (data == null || data.Count > 2) return false;

            if (data.Count >= 1) Rfu0 = data.ToArray()[0];

            if (data.Count == 2) Rfu1 = data.ToArray()[1];

            return true;
        }

        protected override byte[] GetData()
        {
            return _dataBytes;
        }
    };

    internal sealed class ConfigurationDataFile : ConfigurationFile
    {
        private readonly MemoryStream _memoryStream;

        private int _availableSpace;

        public override int MaxFileSize { get; }

        public ConfigurationDataFile(ICollection<byte> productIdentifier, int maxFileSize) : base(productIdentifier)
        {
            _memoryStream = new MemoryStream(MaxFileSize);
            MaxFileSize = maxFileSize;
            _availableSpace = MaxFileSize - FileTemplateSize;
        }

        protected override byte[] GetData() => _memoryStream.ToArray();

        public override bool WriteData(ICollection<byte> data)
        {
            if (data == null || data.Count > _availableSpace) return false;

            _memoryStream.Write(data.ToArray(), 0, data.Count);

            _availableSpace -= data.Count;

            return true;
        }

        public override void Dispose()
        {
            _memoryStream.Dispose();
            base.Dispose();
        }
    };
}
