using Serde;
using System.Reflection;
using System.Text;

namespace AssetParser.SerdeUtils.Enums;

internal class EnumSerdeInfo : ISerdeInfo
{
    private readonly string _fullName;
    private readonly string[] _names;
    private readonly Dictionary<string, int> _nameIndexMap = [];
    private readonly ISerdeInfo _info;

    public EnumSerdeInfo(Type enumType, ISerdeInfo underlyingInfo)
    {
        _fullName = enumType.FullName ?? enumType.Name;
        _names = Enum.GetNames(enumType);
        for (int i = 0; i < _names.Length; ++i)
        {
            _nameIndexMap[_names[i]] = i;
        }
        _info = underlyingInfo;
    }

    public string Name => _fullName;

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
