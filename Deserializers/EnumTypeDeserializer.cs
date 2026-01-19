using AssetParser.Tools;
using AssetParser.TypeTreeUtils;
using Serde;

namespace AssetParser.Deserializers
{
    internal class EnumTypeDeserializer : AbstractTypeDeserializer
    {
        internal EnumTypeDeserializer(EndianBinaryReader reader, TypeTreeNode rootNode): base(reader, rootNode)
        {
        }

        public override (int, string? errorName) TryReadIndexWithName(ISerdeInfo info)
        {
            return (ITypeDeserializer.EndOfType, "EnumValue");
        }

        public override TypeTreeNode GetCurrentNode(ISerdeInfo info, int index)
        {
            return rootNode;
        }

        public override void SkipValue(ISerdeInfo info, int index)
        {
            throw new NotImplementedException();
        }

        public override int? SizeOpt => throw new NotImplementedException();
    }
}
