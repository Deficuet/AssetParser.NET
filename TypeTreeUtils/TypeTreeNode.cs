using AssetParser.Tools;
using Serde;
using System.Buffers;
using System.Text;

namespace AssetParser.TypeTreeUtils;

[GenerateSerde]
public partial class TypeTreeNode
{
    public required string type;
    public required string name;
    public int byteSize;
    public int index;
    public int typeFlags;
    public int version;
    public int metaFlag;
    public int level;

    [SerdeMemberOptions(Skip = true)]
    public List<TypeTreeNode> children = [];

    private NodeDataType _dataType = NodeDataType.Unknown;

    [SerdeMemberOptions(Skip = true)]
    public NodeDataType DataType
    {
        get
        {
            if (_dataType == NodeDataType.Unknown)
            {
                _dataType = NodeDataTypeHelper.MatchDataType(this);
            }
            return _dataType;
        }
    }

    public void CheckAlignmentWith(EndianBinaryReader reader, int alignment = 4)
    {
        if ((metaFlag & 0x4000) != 0)
        {
            reader.AlignStream(alignment);
        }
    }

    private T FinishRead<T>(EndianBinaryReader reader, T result)
    {
        CheckAlignmentWith(reader);
        return result;
    }

    public sbyte ReadSByte(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadSByte());
    }

    public short ReadShort(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadInt16());
    }

    public int ReadInt(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadInt32());
    }

    public long ReadLong(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadInt64());
    }

    public byte ReadByte(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadByte());
    }

    public ushort ReadUShort(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadUInt16());
    }

    public uint ReadUInt(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadUInt32());
    }

    public ulong ReadULong(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadUInt64());
    }

    public float ReadFloat(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadSingle());
    }

    public double ReadDouble(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadDouble());
    }

    public bool ReadBool(EndianBinaryReader reader)
    {
        return FinishRead(reader, reader.ReadBoolean());
    }

    public char ReadChar(EndianBinaryReader reader)
    {
        char c = DataType switch
        {
            NodeDataType.Char => Convert.ToChar(reader.ReadByte()),
            NodeDataType.WideChar => Convert.ToChar(reader.ReadUInt16()),
            _ => throw new NotSupportedException("Char must be 1 or 2 bytes wide")
        };
        return FinishRead(reader, c);
    }

    public void ReadBytes(EndianBinaryReader reader, IBufferWriter<byte> writer)
    {
        var sizeNode = children[0];
        var size = sizeNode.ReadInt(reader);
        var buffer = writer.GetSpan(size);
        reader.Read(buffer);
        CheckAlignmentWith(reader);
    }

    public string ReadString(EndianBinaryReader reader)
    {
        var sizeNode = children[0];
        var charNode = children[1];
        var size = sizeNode.ReadInt(reader);
        var strByteSize = size * charNode.byteSize;
        Span<byte> strBuf = stackalloc byte[strByteSize];
        reader.Read(strBuf);
        string ret = charNode.byteSize switch
        {
            1 => Encoding.UTF8.GetString(strBuf),
            2 => Encoding.Unicode.GetString(strBuf),
            _ => throw new NotSupportedException("Char of a string must be 1 or 2 bytes wide")
        };
        return FinishRead(reader, ret);
    }

    public void Skip(EndianBinaryReader reader)
    {
        if (DataType.IsPrimitiveType())
        {
            reader.Position += byteSize;
            CheckAlignmentWith(reader);
        }
        else if(DataType.IsArrayType())
        {
            var arrayNode = DataType switch
            {
                NodeDataType.Typeless => this,
                _ => children[0]
            };
            var size = arrayNode.children[0].ReadInt(reader);
            var elementNode = arrayNode.children[1];
            for (int i = 0; i < size; ++i)
            {
                elementNode.Skip(reader);
            }
            arrayNode.CheckAlignmentWith(reader);
        }
        else if (DataType.IsCompositeType())
        {
            foreach (var child in children)
            {
                child.Skip(reader);
            }
            CheckAlignmentWith(reader);
        }
    }
} 
