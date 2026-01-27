using AssetParser.Collections;
using Serde;
using System.Reflection;
using System.Text;

namespace AssetParser.SerdeUtils.Collections;

internal class MapPairSerdeInfo<TFirst, TSecond>(ISerdeInfo firstInfo, ISerdeInfo secondInfo) : ISerdeInfo
{
    private readonly string _name = typeof(MapPair<TFirst, TSecond>).Name;
    private readonly ISerdeInfo _firstInfo = firstInfo;
    private readonly ISerdeInfo _secondInfo = secondInfo;

    public string Name => _name;

    public InfoKind Kind => InfoKind.CustomType;

    public PrimitiveKind? PrimitiveKind => null;

    public IList<CustomAttributeData> Attributes => [];

    public int FieldCount => 2;

    public IList<CustomAttributeData> GetFieldAttributes(int index)
    {
        return [];
    }

    public ISerdeInfo GetFieldInfo(int index)
    {
        return index switch
        {
            0 => _firstInfo,
            1 => _secondInfo,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    public ReadOnlySpan<byte> GetFieldName(int index)
    {
        var name = index switch
        {
            0 => "first"u8,
            1 => "second"u8,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
        return name;
    }

    public string GetFieldStringName(int index)
    {
        var name = index switch
        {
            0 => "first",
            1 => "second",
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
        return name;
    }

    public int TryGetIndex(ReadOnlySpan<byte> fieldName)
    {
        var strName = Encoding.UTF8.GetString(fieldName);
        return strName switch
        {
            "first" => 0,
            "second" => 1,
            _ => ITypeDeserializer.IndexNotFound
        };
    }
}
