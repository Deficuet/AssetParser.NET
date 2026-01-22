using System.Buffers.Binary;

namespace AssetParser.Tools;

public class EndianBinaryReader(Stream stream, EndianType endian = EndianType.BigEndian) : BinaryReader(stream)
{
    private readonly long offset = stream.Position;

    public EndianType Endian = endian;

    public long Position
    {
        get => BaseStream.Position - offset;
        set => BaseStream.Position = value + offset;
    }

    public void AlignStream(int alignment = 4)
    {
        var currentPos = Position;
        var mod = currentPos % alignment;
        if (mod != 0)
        {
            Position = currentPos + alignment - mod;
        }
    }

    public override short ReadInt16()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(short)];
            Read(buf);
            return BinaryPrimitives.ReadInt16BigEndian(buf);
        }
        return base.ReadInt16();
    }

    public override int ReadInt32()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(int)];
            Read(buf);
            return BinaryPrimitives.ReadInt32BigEndian(buf);
        }
        return base.ReadInt32();
    }

    public override long ReadInt64()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(long)];
            Read(buf);
            return BinaryPrimitives.ReadInt64BigEndian(buf);
        }
        return base.ReadInt64();
    }

    public Int128 ReadInt128()
    {
        Span<byte> buf = stackalloc byte[16];
        Read(buf);
        return Endian switch
        {
            EndianType.BigEndian => BinaryPrimitives.ReadInt128BigEndian(buf),
            EndianType.LittleEndian => BinaryPrimitives.ReadInt128LittleEndian(buf),
            _ => throw new InvalidOperationException($"Undefined endianess {Endian}"),
        };
    }

    public override ushort ReadUInt16()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(ushort)];
            Read(buf);
            return BinaryPrimitives.ReadUInt16BigEndian(buf);
        }
        return base.ReadUInt16();
    }

    public override uint ReadUInt32()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(uint)];
            Read(buf);
            return BinaryPrimitives.ReadUInt32BigEndian(buf);
        }
        return base.ReadUInt32();
    }

    public override ulong ReadUInt64()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(ulong)];
            Read(buf);
            return BinaryPrimitives.ReadUInt64BigEndian(buf);
        }
        return base.ReadUInt64();
    }

    public UInt128 ReadUInt128()
    {
        Span<byte> buf = stackalloc byte[16];
        Read(buf);
        return Endian switch
        {
            EndianType.BigEndian => BinaryPrimitives.ReadUInt128BigEndian(buf),
            EndianType.LittleEndian => BinaryPrimitives.ReadUInt128LittleEndian(buf),
            _ => throw new InvalidOperationException($"Undefined endianess {Endian}"),
        };
    }

#if NETFRAMEWORK
    public override float ReadSingle()
    {
        if (Endian == EndianType.BigEndian)
        {
            Read(buffer, 0, 4);
            buffer.AsSpan(0, 4).Reverse();
            return BitConverter.ToSingle(buffer, 0);
        }
        return base.ReadSingle();
    }

    public override double ReadDouble()
    {
        if (Endian == EndianType.BigEndian)
        {
            Read(buffer, 0, 8);
            buffer.AsSpan().Reverse();
            return BitConverter.ToDouble(buffer, 0);
        }
        return base.ReadDouble();
    }
#else
    public override float ReadSingle()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(float)];
            Read(buf);
            return BinaryPrimitives.ReadSingleBigEndian(buf);
        }
        return base.ReadSingle();
    }

    public override double ReadDouble()
    {
        if (Endian == EndianType.BigEndian)
        {
            Span<byte> buf = stackalloc byte[sizeof(double)];
            Read(buf);
            return BinaryPrimitives.ReadDoubleBigEndian(buf);
        }
        return base.ReadDouble();
    }
#endif
}
