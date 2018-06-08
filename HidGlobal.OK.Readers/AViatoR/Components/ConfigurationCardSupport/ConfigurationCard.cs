using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HidGlobal.OK.Readers.Utilities;

namespace HidGlobal.OK.Readers.AViatoR.Components.ConfigurationCardSupport
{
    public interface IConfigurationCard : IDisposable
    {
        ICollection<byte> ProductIdentifier { get; }
        string Version { get; }
        byte VersionMajor { get; }
        byte VersionMinor { get; }
        int NumberOfFiles { get; }
        IEnumerable<IApduCommand> GetConfigurationCardCreationCommands();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationApdu"></param>
        void Add(IApduCommand configurationApdu);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationApduEnumerable"></param>
        void AddRange(IEnumerable<IApduCommand> configurationApduEnumerable);
    };

    public class ConfigurationCard : IConfigurationCard
    {
        private const int DataBatchMaxLength = 200; // OMNIKEY 5027 firmawre constrains
        private readonly List<IConfigurationFile> _configurationFiles;
        private readonly ConfigurationCardInitializationCommand _cardInitializationCommand;
        private readonly ConfigurationCardWriteDataCommand _cardWriteDataCommand;
        private readonly int _dataFileMaxSize;
        public ICollection<byte> ProductIdentifier { get; }
        public string Version => $"{VersionMajor:D}.{VersionMinor:D}.0.0";
        public byte VersionMajor { get; }
        public byte VersionMinor { get; }
        public int NumberOfFiles => _configurationFiles.Count;

        public ConfigurationCard([MinLength(2)][MaxLength(2)]ICollection<byte> productIdentifier, byte majorVersion, byte minorVersion)
        {
            _dataFileMaxSize = 236; // OMNIKEY 5027 firmware constrains
            _cardInitializationCommand = new ConfigurationCardInitializationCommand();
            _cardWriteDataCommand = new ConfigurationCardWriteDataCommand();

            _configurationFiles = new List<IConfigurationFile>
            {
                new ConfigurationHeaderFile(productIdentifier, majorVersion, minorVersion),
            };

            ProductIdentifier = productIdentifier;
            VersionMajor = majorVersion;
            VersionMinor = minorVersion;
        }

        public ConfigurationCard(ICollection<byte> productIdentifier, byte majorVersion, byte minorVersion,
            byte fileMaxSize, byte rfu0 = 0x00, byte rfu1 = 0x00) : this(productIdentifier, majorVersion, minorVersion)
        {
            _dataFileMaxSize = fileMaxSize;
            ((ConfigurationHeaderFile) _configurationFiles[0]).Rfu0 = rfu0;
            ((ConfigurationHeaderFile) _configurationFiles[0]).Rfu1 = rfu1;
        }


        public void Add(IApduCommand configurationApdu)
        {
            if (configurationApdu == null) throw new ArgumentNullException(nameof(configurationApdu));

            var writeOperationSuccessful = _configurationFiles.Last().WriteData(configurationApdu.Payload.ToArray());
            if (writeOperationSuccessful) return;

            _configurationFiles.Add(new ConfigurationDataFile(ProductIdentifier, _dataFileMaxSize));
            writeOperationSuccessful = _configurationFiles.Last().WriteData(configurationApdu.Payload.ToArray());

            if (writeOperationSuccessful) return;

            throw new ArgumentOutOfRangeException(nameof(configurationApdu), configurationApdu.GetBytes().Count(),
                $"Configuration command size exceeds max configuration file size of {_dataFileMaxSize:D} bytes.");
        }

        public void AddRange(IEnumerable<IApduCommand> configurationApduEnumerable)
        {
            foreach (var apduCommand in configurationApduEnumerable) Add(apduCommand);
        }

        private IEnumerable<IApduCommand> GetWriteCommands(byte fileNumber, IConfigurationFile configurationFile)
        {
            ushort offset = 0;
            var fileData = configurationFile.GetFileContent();
            
            foreach (var batch in fileData.Batch(batchSize:DataBatchMaxLength))
            {
                var data = batch.ToArray();
                yield return _cardWriteDataCommand.GetApdu(fileNumber, offset, data);
                offset += (ushort) data.Length;
            }
        }

        public IEnumerable<IApduCommand> GetConfigurationCardCreationCommands()
        {
            yield return _cardInitializationCommand.GetApdu((byte) NumberOfFiles);

            byte fileNumber = 0;
            foreach (var configurationFile in _configurationFiles)
            {
                foreach (var writeCommand in GetWriteCommands(fileNumber, configurationFile)) yield return writeCommand;

                fileNumber++;
            }
        }

        public void Dispose()
        {
            foreach (var configurationFile in _configurationFiles) configurationFile.Dispose();
        }
    };
}
