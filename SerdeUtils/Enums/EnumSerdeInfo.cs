using Serde;
using System.Reflection;
using System.Text;

namespace AssetParser.SerdeUtils.Enums;

internal class EnumSerdeInfo<E> : ISerdeInfo
    where E : struct, Enum
{
    private readonly string[] _names;
    private readonly Dictionary<string, int> _nameIndexMap = [];
    private readonly ISerdeInfo _info;

    public EnumSerdeInfo(ISerdeInfo underlyingInfo)
    {
        _names = Enum.GetNames<E>();
        for (int i = 0; i < _names.Length; ++i)
        {
            _nameIndexMap[_names[i]] = i;
        }
        _info = underlyingInfo;
    }

    public string Name { get; } = typeof(E).Name;

    public InfoKind Kind => InfoKind.Enum;

    public PrimitiveKind? PrimitiveKind => null;

    public IList<CustomAttributeData> Attributes { get; } = [];

    public int FieldCount => _names.Length;

    public IList<CustomAttributeData> GetFieldAttributes(int index)
    {
        return [];
    }

    public ISerdeInfo GetFieldInfo(int index)
    {
        return _info;
    }

    public ReadOnlySpan<byte> GetFieldName(int index)
    {
        return Encoding.UTF8.GetBytes(_names[index]);
    }

    public string GetFieldStringName(int index)
    {
        return _names[index];
    }

    public int TryGetIndex(ReadOnlySpan<byte> fieldName)
    {
        return _nameIndexMap.GetValueOrDefault(
            Encoding.UTF8.GetString(fieldName), 
            ITypeDeserializer.IndexNotFound
        );
    }
}
