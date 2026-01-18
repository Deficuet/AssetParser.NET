namespace AssetParser.TypeTreeUtils;

public enum NodeDataType: byte
{
    Int8,
    Int16,
    Int32,
    Int64,
    //Int128,

    UInt8,
    UInt16,
    UInt32,
    UInt64,
    //UInt128,

    Float,
    Double,
    //Decimal,

    Char,
    WideChar,

    Bool,

    ByteArray,
    Typeless,
    String,

    Array,
    Map,

    Pair,
    Class,

    Unknown
}
