using Serde;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssetParser.SerdeUtils.Enums;

public sealed class EnumValueProxy<E, N, NProvider>: ISerdeProvider<E>, ISerde<E>
    where E : struct, Enum
    where N : unmanaged, IConvertible
    where NProvider: ISerdeProvider<NProvider, NProvider, N>, ISerde<N>
{
    private static readonly EnumValueProxy<E, N, NProvider> s_instance = new();
    static ISerialize<E> ISerializeProvider<E>.Instance => s_instance;
    static IDeserialize<E> IDeserializeProvider<E>.Instance => s_instance;

    private static readonly E[] s_values;
    
    private static readonly ISerialize<N> s_valueSer = SerializeProvider.GetSerialize<N, NProvider>();
    private static readonly IDeserialize<N> s_valueDe = DeserializeProvider.GetDeserialize<N, NProvider>();

    private static readonly EnumSerdeInfo s_serdeInfo;

    static EnumValueProxy()
    {
        var enumType = typeof(E);
        var underlyingType = Enum.GetUnderlyingType(enumType);
        var valueType = typeof(N);
        if (valueType != underlyingType)
        {
            throw new InvalidOperationException(
                $"Type {valueType.FullName} is not the underlying type of {enumType.FullName} ({underlyingType.FullName})"
            );
        }
        s_values = Enum.GetValues(enumType) as E[]
            ?? throw new InvalidOperationException($"Failed to get enum values for enum type {enumType.FullName}");
        s_serdeInfo = new EnumSerdeInfo(enumType, NProvider.Instance.SerdeInfo);
    }

    public ISerdeInfo SerdeInfo => s_serdeInfo;

    public void Serialize(E value, ISerializer serializer)
    {
        var underlying = Unsafe.As<E, N>(ref value);
        s_valueSer.Serialize(underlying, serializer);
    }

    public E Deserialize(IDeserializer deserializer)
    {
        var (index, enumName) = deserializer.ReadEnumIndex(SerdeInfo);
        if (index == ITypeDeserializer.NonApplicable)
        {
            var value = s_valueDe.Deserialize(deserializer);
            return Unsafe.As<N, E>(ref value);
        }
        if (index == ITypeDeserializer.IndexNotFound)
        {
            throw DeserializeException.UnknownMember(enumName ?? "<null>", SerdeInfo);
        }
        return s_values[index];
    }
}

public sealed class EnumStringSerProxy<E>: ISerializeProvider<E>, ISerialize<E>
    where E : struct, Enum
{
    private static readonly Type s_enumType = typeof(E);
    private static readonly EnumStringSerProxy<E> s_instance = new();
    static ISerialize<E> ISerializeProvider<E>.Instance => s_instance;

    private static readonly EnumSerdeInfo s_serdeInfo = new(s_enumType, StringProxy.SerdeInfo);
    public ISerdeInfo SerdeInfo => s_serdeInfo;

    public void Serialize(E value, ISerializer serializer)
    {
        var name = Enum.GetName(s_enumType, value)
            ?? throw new InvalidEnumArgumentException($"Failed to get name for enum value {value} of type {s_enumType.FullName}");
        serializer.WriteString(name);
    }
}
