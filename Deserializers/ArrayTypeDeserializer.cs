using AssetParser.TypeTreeUtils;
using AssetParser.Tools;
using Serde;

namespace AssetParser.Deserializers;

internal class ArrayTypeDeserializer: AbstractTypeDeserializer
{
    private readonly int _size;
    private readonly UnityObjectDeserializer _elementDeserializer;

    internal ArrayTypeDeserializer(EndianBinaryReader reader, TypeTreeNode rootNode): base(reader, rootNode)
    {
        _size = rootNode.children[0].ReadInt(reader);
        _elementDeserializer = new UnityObjectDeserializer(reader, rootNode.children[1]);
    }

    public override int? SizeOpt => _size;

    public override TypeTreeNode GetCurrentNode(ISerdeInfo info, int index)
    {
        return rootNode.children[1];
    }

    public override IDeserializer GetDeserializerForCompositeValue(TypeTreeNode node, ISerdeInfo info, int index)
    {
        return _elementDeserializer;
    }

    public override (int, string? errorName) TryReadIndexWithName(ISerdeInfo info)
    {
        throw new NotImplementedException();
    }

    public override void SkipValue(ISerdeInfo info, int index)
    {
        throw new NotImplementedException();
    }
}
