using AssetParser.TypeTreeUtils;
using AssetParser.Tools;
using Serde;

namespace AssetParser.Deserializers;

internal class CustomTypeDeserializer: AbstractTypeDeserializer
{
    private readonly Dictionary<string, int> fieldNameIndexMap = [];
    private List<TypeTreeNode>.Enumerator nodeIter;
    private TypeTreeNode? currentNode = null;

    internal CustomTypeDeserializer(EndianBinaryReader reader, TypeTreeNode rootNode, ISerdeInfo serdeInfo):
        base(reader, rootNode)
    {
        nodeIter = rootNode.children.GetEnumerator();
        for (int i = 0; i < serdeInfo.FieldCount; ++i)
        {
            fieldNameIndexMap[serdeInfo.GetFieldStringName(i)] = i;
        }
    }

    public override (int, string? errorName) TryReadIndexWithName(ISerdeInfo info)
    {
        if (!nodeIter.MoveNext())
        {
            return (ITypeDeserializer.EndOfType, null);
        }
        currentNode = nodeIter.Current;
        return (
            fieldNameIndexMap.GetValueOrDefault(
                currentNode.name, ITypeDeserializer.IndexNotFound
            ),
            currentNode.name
        );
    }

    public override void SkipValue(ISerdeInfo info, int index)
    {
        var currentNode = GetCurrentNode(info, index);
        currentNode.Skip(reader);
    }

    public override TypeTreeNode GetCurrentNode(ISerdeInfo info, int index)
    {
        return currentNode!;
    }

    public void Dispose()
    {
        nodeIter.Dispose();
    }

    public override int? SizeOpt => throw new NotImplementedException();
}
