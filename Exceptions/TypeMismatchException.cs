using AssetParser.TypeTreeUtils;
using Serde;

namespace AssetParser.Exceptions;

public class TypeMismatchException: Exception
{
    internal TypeMismatchException(TypeTreeNode node, string typeDesc): 
        base($"Deserialize node {node.name} as <{typeDesc}>, but the node is {node.DataType}")
    { }

    internal TypeMismatchException(TypeTreeNode node, TypeTreeNode elementNode, string typeDesc) :
        base(
            $"Deserialize node {node.name} as <{typeDesc}>, but the node is {node.DataType} " +
            $"with element {elementNode.DataType}"
        )
    { }

    internal TypeMismatchException(TypeTreeNode node, ISerdeInfo serdeInfo, int index, string typeDesc): 
        base(
            $"In class {serdeInfo.Name} field {serdeInfo.GetFieldStringName(index)}" +
            $"requires <{typeDesc}> but the node {node.name}({node.type}) is {node.DataType}."
        )
    { }

    internal TypeMismatchException(
        TypeTreeNode node, TypeTreeNode elementNode, 
        ISerdeInfo serdeInfo, int index, string typeDesc
    ) : base(
            $"In class {serdeInfo.Name} field {serdeInfo.GetFieldStringName(index)}" +
            $"requires <{typeDesc}> but the node {node.name}({node.type}) is {node.DataType} " +
            $"with element {elementNode.DataType}"
        )
    { }
}
