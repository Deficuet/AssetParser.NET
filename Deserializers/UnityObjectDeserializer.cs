using AssetParser.Exceptions;
using AssetParser.Tools;
using AssetParser.TypeTreeUtils;
using Serde;

namespace AssetParser.Deserializers;

public class UnityObjectDeserializer(EndianBinaryReader reader, TypeTreeNode rootNode) : IDeserializer
{
    private readonly EndianBinaryReader reader = reader;
    private readonly TypeTreeNode rootNode = rootNode;

    public ITypeDeserializer ReadType(ISerdeInfo typeInfo)
    {
        if (typeInfo.Kind == InfoKind.CustomType)
        {
            if (rootNode.DataType.IsCompositeType())
            {
                return new CustomTypeDeserializer(reader, rootNode, typeInfo);
            }
        }
        else if (typeInfo.Kind == InfoKind.List)
        {
            if (rootNode.DataType.IsCompositeArrayType())
            {
                return new ArrayTypeDeserializer(reader, rootNode.children[0]);
            }
        }
        throw new TypeMismatchException(rootNode, typeInfo.Name);
    }

    private void CheckNode(string typeDesc, bool predicate)
    {
        if (!predicate)
        {
            throw new TypeMismatchException(rootNode, typeDesc);
        }
    }

    public sbyte ReadI8()
    {
        CheckNode("sbyte", rootNode.DataType == NodeDataType.Int8);
        return rootNode.ReadSByte(reader);
    }

    public short ReadI16()
    {
        CheckNode("short", rootNode.DataType == NodeDataType.Int16);
        return rootNode.ReadShort(reader);
    }

    public int ReadI32()
    {
        CheckNode("int", rootNode.DataType == NodeDataType.Int32);
        return rootNode.ReadInt(reader);
    }

    public long ReadI64()
    {
        CheckNode("long", rootNode.DataType == NodeDataType.Int64);
        return rootNode.ReadLong(reader);
    }

    public Int128 ReadI128()
    {
        throw new NotImplementedException();
    }

    public byte ReadU8()
    {
        CheckNode("byte", rootNode.DataType == NodeDataType.UInt8);
        return rootNode.ReadByte(reader);
    }

    public ushort ReadU16()
    {
        CheckNode("ushort", rootNode.DataType == NodeDataType.UInt16);
        return rootNode.ReadUShort(reader);
    }

    public uint ReadU32()
    {
        CheckNode("uint", rootNode.DataType == NodeDataType.UInt32);
        return rootNode.ReadUInt(reader);
    }

    public ulong ReadU64()
    {
        CheckNode("ulong", rootNode.DataType == NodeDataType.UInt64);
        return rootNode.ReadULong(reader);
    }

    public UInt128 ReadU128()
    {
        CheckNode("Guid/Hash128(UInt128)", rootNode.DataType.IsUInt128BasedType());
        return rootNode.ReadUInt128(reader);
    }

    public float ReadF32()
    {
        CheckNode("float", rootNode.DataType == NodeDataType.Float);
        return rootNode.ReadFloat(reader);
    }

    public double ReadF64()
    {
        CheckNode("double", rootNode.DataType == NodeDataType.Double);
        return rootNode.ReadDouble(reader);
    }

    public decimal ReadDecimal()
    {
        throw new NotImplementedException();
    }

    public bool ReadBool()
    {
        CheckNode("bool", rootNode.DataType == NodeDataType.Bool);
        return rootNode.ReadBool(reader);
    }

    public char ReadChar()
    {
        CheckNode("char", rootNode.DataType.IsCharType());
        return rootNode.ReadChar(reader);
    }

    public byte[] ReadBytes()
    {
        var arrayNode = rootNode.DataType switch
        {
            NodeDataType.ByteArray => rootNode.children[0],
            NodeDataType.Typeless => rootNode,
            _ => throw new TypeMismatchException(rootNode, "byte[]")
        };
        var elementNode = arrayNode.children[1];
        if (elementNode.DataType != NodeDataType.UInt8)
        {
            throw new TypeMismatchException(rootNode, elementNode, "byte[]");
        }
        return arrayNode.ReadBytes(reader);
    }

    public string ReadString()
    {
        CheckNode("string", rootNode.DataType == NodeDataType.String);
        return rootNode.ReadString(reader);
    }

    public (int, string?) ReadEnumIndex(ISerdeInfo enumInfo)
    {
        return (ITypeDeserializer.NonApplicable, null);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public T? ReadNullableRef<T>(IDeserialize<T> deserialize) where T : class
    {
        throw new NotImplementedException();
    }

    public DateTime ReadDateTime()
    {
        throw new NotImplementedException();
    }

    public DateTimeOffset ReadDateTimeOffset()
    {
        throw new NotImplementedException();
    }
}
