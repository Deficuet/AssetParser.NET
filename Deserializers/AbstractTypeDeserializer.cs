using AssetParser.Exceptions;
using AssetParser.Tools;
using AssetParser.TypeTreeUtils;
using Serde;
using System.Buffers;

namespace AssetParser.Deserializers;

internal abstract class AbstractTypeDeserializer : ITypeDeserializer
{
    protected readonly EndianBinaryReader reader;
    protected readonly TypeTreeNode rootNode;

    internal AbstractTypeDeserializer(EndianBinaryReader reader, TypeTreeNode rootNode)
    {
        this.reader = reader;
        this.rootNode = rootNode;
    }

    public abstract (int, string? errorName) TryReadIndexWithName(ISerdeInfo info);

    public int TryReadIndex(ISerdeInfo info)
    {
        return TryReadIndexWithName(info).Item1;
    }

    public abstract void SkipValue(ISerdeInfo info, int index);

    public abstract TypeTreeNode GetCurrentNode(ISerdeInfo info, int index);

    public abstract int? SizeOpt { get; }

    public virtual IDeserializer GetDeserializerForCompositeValue(TypeTreeNode node, ISerdeInfo info, int index)
    {
        return new UnityObjectDeserializer(reader, node);
    }

    private static void CheckNode(TypeTreeNode node, ISerdeInfo serdeInfo, int index, string typeDesc, bool predicate)
    {
        if (!predicate)
        {
            throw new TypeMismatchException(node, serdeInfo, index, typeDesc);
        }
    }

    public sbyte ReadI8(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "sbyte", currentNode.DataType == NodeDataType.Int8);
        return currentNode.ReadSByte(reader);
    }

    public short ReadI16(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "short", currentNode.DataType == NodeDataType.Int16);
        return currentNode.ReadShort(reader);
    }

    public int ReadI32(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "int", currentNode.DataType == NodeDataType.Int32);
        return currentNode.ReadInt(reader);
    }

    public long ReadI64(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "long", currentNode.DataType == NodeDataType.Int64);
        return currentNode.ReadLong(reader);
    }

    public Int128 ReadI128(ISerdeInfo info, int index)
    {
        throw new NotImplementedException();
    }

    public byte ReadU8(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "byte", currentNode.DataType == NodeDataType.UInt8);
        return currentNode.ReadByte(reader);
    }

    public ushort ReadU16(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "ushort", currentNode.DataType == NodeDataType.UInt16);
        return currentNode.ReadUShort(reader);
    }

    public uint ReadU32(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "uint", currentNode.DataType == NodeDataType.UInt32);
        return currentNode.ReadUInt(reader);
    }

    public ulong ReadU64(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "ulong", currentNode.DataType == NodeDataType.UInt64);
        return currentNode.ReadULong(reader);
    }

    public UInt128 ReadU128(ISerdeInfo info, int index)
    {
        throw new NotImplementedException();
    }

    public float ReadF32(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "float", currentNode.DataType == NodeDataType.Float);
        return currentNode.ReadFloat(reader);
    }

    public double ReadF64(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "double", currentNode.DataType == NodeDataType.Double);
        return currentNode.ReadDouble(reader);
    }

    public bool ReadBool(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "bool", currentNode.DataType == NodeDataType.Bool);
        return currentNode.ReadBool(reader);
    }

    public char ReadChar(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "char", currentNode.DataType.IsCharType());
        return currentNode.ReadChar(reader);
    }

    public void ReadBytes(ISerdeInfo info, int index, IBufferWriter<byte> writer)
    {
        var currentNode = GetCurrentNode(info, index);
        var arrayNode = currentNode.DataType switch
        {
            NodeDataType.ByteArray => currentNode.children[0],
            NodeDataType.Typeless => currentNode,
            _ => throw new TypeMismatchException(currentNode, info, index, "byte[]")
        };
        var elementNode = arrayNode.children[1];
        if (elementNode.DataType != NodeDataType.UInt8)
        {
            throw new TypeMismatchException(currentNode, elementNode, info, index, "byte[]");
        }
        arrayNode.ReadBytes(reader, writer);
    }

    public string ReadString(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        CheckNode(currentNode, info, index, "string", currentNode.DataType == NodeDataType.String);
        return currentNode.children[0].ReadString(reader);
    }

    public T ReadValue<T>(ISerdeInfo info, int index, IDeserialize<T> deserialize) where T : class?
    {
        var currentNode = GetCurrentNode(info, index);
        return deserialize.Deserialize(
            GetDeserializerForCompositeValue(currentNode, info, index)
        );
    }

    public virtual void End(ISerdeInfo info)
    {
        rootNode.CheckAlignmentWith(reader);
    }

    public DateTime ReadDateTime(ISerdeInfo info, int index)
    {
        throw new NotImplementedException();
    }

    public DateTimeOffset ReadDateTimeOffset(ISerdeInfo info, int index)
    {
        throw new NotImplementedException();
    }

    public decimal ReadDecimal(ISerdeInfo info, int index)
    {
        throw new NotImplementedException();
    }
}
