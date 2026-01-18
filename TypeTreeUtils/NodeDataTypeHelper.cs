namespace AssetParser.TypeTreeUtils;

public static class NodeDataTypeHelper
{
    private readonly static HashSet<string> SIntTypeNameSet =
    [
        "SInt8",
        "SInt16", "short",
        "SInt32", "int",
        "SInt64", "long long"
    ];

    private readonly static HashSet<string> UIntTypeNameSet =
    [
        "UInt8",
        "UInt16", "unsigned short",
        "UInt32", "unsigned int", "Type*",
        "UInt64", "unsigned long long", "FileSize"
    ];

    private readonly static HashSet<string> DecimalTypeNameSet =
    [
        "float",
        "double"
    ];

    public static bool IsSInt(this TypeTreeNode node)
    {
        return SIntTypeNameSet.Contains(node.type) && node.children.Count == 0;
    }

    public static bool IsUInt(this TypeTreeNode node)
    {
        return UIntTypeNameSet.Contains(node.type) && node.children.Count == 0;
    }

    public static bool IsDecimal(this TypeTreeNode node)
    {
        return DecimalTypeNameSet.Contains(node.type) && node.children.Count == 0;
    }

    public static bool IsBool(this TypeTreeNode node)
    {
        return node.type == "bool" && node.byteSize == 1 && node.children.Count == 0;
    }

    public static bool IsChar(this TypeTreeNode node)
    {
        return node.type == "char" && node.children.Count == 0 && (node.byteSize == 1 || node.byteSize == 2);
    }

    public static bool IsArray(this TypeTreeNode node)
    {
        if (node.children.Count != 1)
        {
            return false;
        }
        var arrayNode = node.children[0];
        if (arrayNode.name != "Array" || arrayNode.children.Count != 2)
        {
            return false;
        }
        return arrayNode.children[0].DataType == NodeDataType.Int32;
    }

    public static bool IsString(this TypeTreeNode node)
    {
        if (node.type != "string") return false;
        if (!node.IsArray()) return false;
        var elementType = node.children[0].children[1].DataType;
        return elementType.IsCharType();
    }

    public static bool IsPair(this TypeTreeNode node)
    {
        return node.type == "pair" && node.children.Count == 2;
    }

    public static bool IsMap(this TypeTreeNode node)
    {
        return node.type == "map" && node.IsArray() && node.children[0].children[1].DataType == NodeDataType.Pair;
    }

    public static bool IsTypeless(this TypeTreeNode node)
    {
        if (node.type != "TypelessData") return false;
        if (node.children.Count != 2) return false;
        if (node.children[0].DataType != NodeDataType.Int32) return false;
        var elementType = node.children[1].DataType;
        return elementType.IsSIntType() || elementType.IsUIntType();
    }

    public static bool IsByteArray(this TypeTreeNode node)
    {
        return node.IsArray() && node.children[0].children[1].DataType == NodeDataType.UInt8;
    }

    public static NodeDataType MatchDataType(TypeTreeNode node)
    {
        if (node.IsSInt())
        {
            return node.byteSize switch
            {
                1 => NodeDataType.Int8,
                2 => NodeDataType.Int16,
                4 => NodeDataType.Int32,
                8 => NodeDataType.Int64,
                _ => NodeDataType.Class
            };
        }
        else if (node.IsUInt())
        {
            return node.byteSize switch
            {
                1 => NodeDataType.UInt8,
                2 => NodeDataType.UInt16,
                4 => NodeDataType.UInt32,
                8 => NodeDataType.UInt64,
                _ => NodeDataType.Class
            };
        }
        else if (node.IsDecimal())
        {
            return node.byteSize switch
            {
                4 => NodeDataType.Float,
                8 => NodeDataType.Double,
                _ => NodeDataType.Class
            };
        }
        else if (node.IsBool())
        {
            return NodeDataType.Bool;
        }
        else if (node.IsChar())
        {
            return node.byteSize switch
            {
                1 => NodeDataType.Char,
                2 => NodeDataType.WideChar,
                _ => NodeDataType.Class
            };
        }
        else if (node.IsString())
        {
            return NodeDataType.String;
        }
        else if (node.IsMap())
        {
            return NodeDataType.Map;
        }
        else if (node.IsPair())
        {
            return NodeDataType.Pair;
        }
        else if (node.IsArray())
        {
            return NodeDataType.Array;
        }
        else if (node.IsTypeless())
        {
            return NodeDataType.Typeless;
        }
        else if (node.IsByteArray())
        {
            return NodeDataType.ByteArray;
        }
        else
        {
            return NodeDataType.Class;
        }
    }

    public static bool IsSIntType(this NodeDataType type)
    {
        return type < NodeDataType.UInt8;
    }

    public static bool IsUIntType(this NodeDataType type)
    {
        return type >= NodeDataType.UInt8 && type < NodeDataType.Float;
    }

    public static bool IsDecimalType(this NodeDataType type)
    {
        return type >= NodeDataType.Float && type < NodeDataType.Char;
    }

    public static bool IsCharType(this NodeDataType type)
    {
        return type >= NodeDataType.Char && type < NodeDataType.Bool;
    }

    public static bool IsPrimitiveType(this NodeDataType type)
    {
        return type <= NodeDataType.ByteArray;
    }

    public static bool IsArrayType(this NodeDataType type)
    {
        return type >= NodeDataType.ByteArray && type < NodeDataType.Pair;
    }

    public static bool IsCompositeArrayType(this NodeDataType type)
    {
        return type >= NodeDataType.Array && type < NodeDataType.Pair;
    }

    public static bool IsCompositeType(this NodeDataType type)
    {
        return type >= NodeDataType.Pair && type < NodeDataType.Unknown;
    }
}
