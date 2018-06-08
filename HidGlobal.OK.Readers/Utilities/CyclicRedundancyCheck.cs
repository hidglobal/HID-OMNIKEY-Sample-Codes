using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace HidGlobal.OK.Readers.Utilities
{
    public sealed class CrcX25Algorithm : HashAlgorithm
    {
        private static readonly ushort[] LookUpTable = new ushort[256];

        public static readonly ushort Polynomial;
        public static readonly ushort InitialValue;
        public static readonly ushort FinalXoR;
        public static readonly bool ReflectedInput;
        public static readonly bool ReflectedOutput;
        public static readonly string Description;

        private ushort _hash;

        static CrcX25Algorithm()
        {
            Polynomial = 0x1021;
            ReflectedInput = true;
            ReflectedOutput = true;
            Description = "CRC-16/X-25";
            FinalXoR = 0xFFFF;
            InitialValue = 0xFFFF;

            PrepareLookUpTable();
        }

        public CrcX25Algorithm()
        {
            HashSizeValue = 16;
            Initialize();
        }

        private static void PrepareLookUpTable()
        {
            var calculationConstant = ReverseBits(Polynomial);

            for (ushort i = 0; i < LookUpTable.Length; ++i)
            {
                ushort value = 0;
                var temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    var isXorNeeded = ((value ^ temp) & 0x0001) != 0;

                    value >>= 1;
                    temp >>= 1;

                    if (isXorNeeded)
                        value ^= calculationConstant;

                }
                LookUpTable[i] = value;
            }
        }

        private static ushort ReverseBits(ushort value)
        {
            ushort bitReversedValue = 0;
            for (var i = 0; i < 16; ++i)
            {
                bitReversedValue <<= 1;
                bitReversedValue |= (ushort)(value & 1);
                value >>= 1;
            }
            return bitReversedValue;
        }

        public override void Initialize()
        {
            _hash = InitialValue;
            HashValue = new byte[2];
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            foreach (var b in array.Skip(ibStart).Take(cbSize))
            {
                var ptr = (byte) (_hash & 0xFF) ^ b;
                _hash >>= 8;
                _hash ^= LookUpTable[ptr];
            }
        }

        public new byte[] ComputeHash(Stream inputStream)
        {
            int bytesRead;
            var buffer = new byte[4096];

            while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0)
            {
                HashCore(buffer, 0, bytesRead);
            }

            return HashFinal();
        }

        protected override byte[] HashFinal()
        {
            _hash ^= FinalXoR;

            HashValue[0] = (byte)((_hash >> 8) & 0x00FF);
            HashValue[1] = (byte)(_hash & 0x00FF);

            return HashValue;
        }
    }
}
