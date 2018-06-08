using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HidGlobal.OK.Readers
{
    public enum LengthFieldFormat
    {
        Absent,
        Short,
        Extended,
    };
    public enum ApduFormat
    {
        Short,
        Extended,
    };

    public interface ILengthField
    {
        LengthFieldFormat Format { get; }
        int PayloadLength { get; }
        IEnumerable<byte> GetBytes();
    }

    public interface IApduCommand
    {
        byte Cla { get; }
        byte Ins { get; }
        byte P1 { get; }
        byte P2 { get; }
        ILengthField Lc { get; }
        IReadOnlyList<byte> Payload { get; }
        ILengthField Le { get; }
        ApduFormat Format { get; }
        IEnumerable<byte> GetBytes();
        string ToString();
    }

    internal class LeField : ILengthField
    {
        private readonly byte[] _leField;
        public LengthFieldFormat Format { get; }
        public IEnumerable<byte> GetBytes() => _leField;
        public int PayloadLength { get; }

        public LeField(ApduFormat apduFormat, LengthFieldFormat lcFieldFormat, int maximumExpectedResponseDataFieldLength)
        {
            if (maximumExpectedResponseDataFieldLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maximumExpectedResponseDataFieldLength),
                    maximumExpectedResponseDataFieldLength,
                    $"{maximumExpectedResponseDataFieldLength} must have positive integer value.");

            if (maximumExpectedResponseDataFieldLength == 0)
            {
                PayloadLength = 0;
                Format = LengthFieldFormat.Absent;
                _leField = new byte[0];
                return;
            }

            switch (apduFormat)
            {
                case ApduFormat.Short:
                    if (maximumExpectedResponseDataFieldLength < byte.MaxValue + 1)
                    {
                        Format = LengthFieldFormat.Short;
                        PayloadLength = maximumExpectedResponseDataFieldLength;
                        _leField = new[] { (byte)maximumExpectedResponseDataFieldLength };
                    }
                    else if (maximumExpectedResponseDataFieldLength == byte.MaxValue + 1)
                    {
                        Format = LengthFieldFormat.Short;
                        PayloadLength = maximumExpectedResponseDataFieldLength;
                        _leField = new byte[] {0x00};
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(maximumExpectedResponseDataFieldLength),
                            maximumExpectedResponseDataFieldLength,
                            "Short APDU maximal expected response payload length is 256 bytes.");
                    }
                    break;

                case ApduFormat.Extended:
                    if (maximumExpectedResponseDataFieldLength < ushort.MaxValue + 1)
                    {
                        Format = LengthFieldFormat.Extended;
                        PayloadLength = maximumExpectedResponseDataFieldLength;
                        _leField = lcFieldFormat != LengthFieldFormat.Absent
                            ? new[] { (byte)(maximumExpectedResponseDataFieldLength >> 8), (byte)maximumExpectedResponseDataFieldLength }
                            : new[] { (byte)0, (byte)(maximumExpectedResponseDataFieldLength >> 8), (byte)maximumExpectedResponseDataFieldLength };
                    }
                    else if (maximumExpectedResponseDataFieldLength == ushort.MaxValue + 1)
                    {
                        Format = LengthFieldFormat.Extended;
                        PayloadLength = maximumExpectedResponseDataFieldLength;
                        _leField = lcFieldFormat != LengthFieldFormat.Absent
                            ? new[] { (byte)0, (byte)0 }
                            : new[] { (byte)0, (byte)0, (byte)0 };
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(maximumExpectedResponseDataFieldLength),
                            $"Maximal value exceeded. Maximal available value: {ushort.MaxValue + 1}, actual value: {maximumExpectedResponseDataFieldLength}");
                    }
                    break;
                        
                default:
                    throw new ArgumentOutOfRangeException(nameof(apduFormat), apduFormat, null);
            }
        }
    }

    internal class LcField : ILengthField
    {
        private readonly byte[] _lcField;

        public IEnumerable<byte> GetBytes() => _lcField;
        public LengthFieldFormat Format { get; }
        public int PayloadLength { get; }
        
        public LcField(ApduFormat format, int payloadLength)
        {
            if (payloadLength < 0) throw new ArgumentOutOfRangeException(nameof(payloadLength), payloadLength, $"{payloadLength} must have positive integer value.");

            if (payloadLength == 0)
            {
                _lcField = new byte[0];
                PayloadLength = 0;
                Format = LengthFieldFormat.Absent;
                return;
            }
            
            switch (format)
            {
                case ApduFormat.Short:
                    if (payloadLength <= byte.MaxValue)
                    {
                        Format = LengthFieldFormat.Short;
                        PayloadLength = payloadLength;
                        _lcField = new[] {(byte) payloadLength};
                    }
                    else throw new ArgumentOutOfRangeException(nameof(payloadLength), payloadLength, "Short APDU maximal payload length is 255 bytes.");

                    break;

                case ApduFormat.Extended:
                    if (payloadLength <= ushort.MaxValue)
                    {
                        Format = LengthFieldFormat.Extended;
                        PayloadLength = payloadLength;
                        _lcField = new[] { (byte)0, (byte)(payloadLength >> 8), (byte)payloadLength };
                    }
                    else throw new ArgumentOutOfRangeException(nameof(payloadLength), payloadLength, "Extended APDU maximal payload length is 65535 bytes.");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }

    public class ApduCommand : IApduCommand
    {
        public static byte[] HidGlobalSpecificApduCommandHeader { get; } = {0xFF, 0x70, 0x07, 0x6B};
        public const int ShortApduMaximalExpectedResponsePayloadLength = 256;
        public const int ExtendedApduMaximalExpectedResponsePayloadLength = 65536;

        private readonly byte[] _payloadBytes;
        private readonly LcField _lc;
        private readonly LeField _le;

        public ApduFormat Format { get; }
        public byte Cla { get; }
        public byte Ins { get; }
        public byte P1 { get; }
        public byte P2 { get; }
        public ILengthField Lc => _lc;
        public IReadOnlyList<byte> Payload => _payloadBytes;
        public ILengthField Le => _le;

        private ApduCommand(IEnumerable<byte> apduHeader, IEnumerable<byte> payload,
            int? maximumExpectedResponseDataFieldLength, ApduFormat? format)
        {
            var header = apduHeader.ToArray();
            var temp = payload.ToArray();

            if (header.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(apduHeader),
                    $"APDU header must have 4 bytes long, actual length: {header.Length}");
            }

            switch (format)
            {
                case null:
                    if (temp.Length <= byte.MaxValue)
                    {
                        Format = ApduFormat.Short;
                    }
                    else if (temp.Length <= ushort.MaxValue)
                    {
                        Format = ApduFormat.Extended;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(payload), temp.Length,
                            "Payload length exceeds maximal permitted value of 65535 bytes");
                    }
                    break;

                case ApduFormat.Short:
                    if (temp.Length > byte.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(payload), temp.Length,
                            "Payload length exceeds maximal permitted value of 255 bytes");
                    }
                    Format = ApduFormat.Short;
                    break;

                case ApduFormat.Extended:
                    if (temp.Length > ushort.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(payload), temp.Length,
                            "Payload length exceeds maximal permitted value of 65535 bytes");
                    }
                    Format = ApduFormat.Extended;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }


            var maxResponeLength = Format == ApduFormat.Short
                ? (maximumExpectedResponseDataFieldLength ?? ShortApduMaximalExpectedResponsePayloadLength)
                : (maximumExpectedResponseDataFieldLength ?? ExtendedApduMaximalExpectedResponsePayloadLength);

            Cla = header[0];
            Ins = header[1];
            P1 = header[2];
            P2 = header[3];

            _lc = new LcField(Format, temp.Length);
            _payloadBytes = temp;
            _le = new LeField(Format, _lc.Format, maxResponeLength);
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="apduHeader"></param>
        /// <param name="payload"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(IEnumerable<byte> apduHeader, IEnumerable<byte> payload,
            int? maximumExpectedResponseDataFieldLength = null) : this(apduHeader, payload,
            maximumExpectedResponseDataFieldLength, format: null)
        {
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="apduHeader"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(IEnumerable<byte> apduHeader, int? maximumExpectedResponseDataFieldLength = null)
            : this(apduHeader, new byte[0], maximumExpectedResponseDataFieldLength, format: null)
        {
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="payload"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(byte cla, byte ins, byte p1, byte p2, IEnumerable<byte> payload,
            int? maximumExpectedResponseDataFieldLength = null)
            : this(new[] {cla, ins, p1, p2}, payload, maximumExpectedResponseDataFieldLength, format: null)
        {
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(byte cla, byte ins, byte p1, byte p2, int? maximumExpectedResponseDataFieldLength = null)
            : this(new[] {cla, ins, p1, p2}, new byte[0], maximumExpectedResponseDataFieldLength, format: null)
        {
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="explicitFormat"></param>
        /// <param name="apduHeader"></param>
        /// <param name="payload"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(ApduFormat explicitFormat, IEnumerable<byte> apduHeader, IEnumerable<byte> payload,
            int? maximumExpectedResponseDataFieldLength = null) : this(apduHeader, payload,
            maximumExpectedResponseDataFieldLength, format: explicitFormat)
        {
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="explicitFormat"></param>
        /// <param name="apduHeader"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(ApduFormat explicitFormat, IEnumerable<byte> apduHeader,
            int? maximumExpectedResponseDataFieldLength = null)
            : this(apduHeader, new byte[0], maximumExpectedResponseDataFieldLength, format: explicitFormat)
        {
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="explicitFormat"></param>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="payload"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(ApduFormat explicitFormat, byte cla, byte ins, byte p1, byte p2, IEnumerable<byte> payload,
            int? maximumExpectedResponseDataFieldLength = null)
            : this(new[] {cla, ins, p1, p2}, payload, maximumExpectedResponseDataFieldLength,
                format: explicitFormat)
        {
        }

        /// <summary>
        /// Initialize new Apdu command object.
        /// </summary>
        /// <param name="explicitFormat"></param>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="maximumExpectedResponseDataFieldLength">Set to null to specify maximal available response size for give apdu format.</param>
        public ApduCommand(ApduFormat explicitFormat, byte cla, byte ins, byte p1, byte p2,
            int? maximumExpectedResponseDataFieldLength = null)
            : this(new[] {cla, ins, p1, p2}, new byte[0], maximumExpectedResponseDataFieldLength,
                format: explicitFormat)
        {
        }


        public IEnumerable<byte> GetBytes()
        {
            yield return Cla;
            yield return Ins;
            yield return P1;
            yield return P2;
            foreach (var b in Lc.GetBytes())
            {
                yield return b;
            }
            foreach (var b in Payload)
            {
                yield return b;
            }
            foreach (var b in Le.GetBytes())
            {
                yield return b;
            }
        }

        public override string ToString()
        {
            var value = $"{Cla:X2}{Ins:X2}{P1:X2}{P2:X2}";

            if (Lc.PayloadLength != 0)
                value += BitConverter.ToString(Lc.GetBytes().ToArray()).Replace("-", "");

            if (_payloadBytes.Length != 0)
                value += BitConverter.ToString(_payloadBytes).Replace("-", "");

            if (Le.PayloadLength != 0)
                value += BitConverter.ToString(Le.GetBytes().ToArray()).Replace("-", "");

            return value;
        }

        public static explicit operator byte[](ApduCommand apduCommand)
        {
            return apduCommand.GetBytes().ToArray();
        }
    };
}
