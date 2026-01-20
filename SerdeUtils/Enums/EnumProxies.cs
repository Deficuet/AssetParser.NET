using Serde;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssetParser.SerdeUtils.Enums;

public sealed class EnumValueProxy<E, N, NProvider>: ISerdeProvider<E>, ISerde<E>
    where E : struct, Enum
    where N : unmanaged, IConvertible
    where NProvider: ISerdeProvider<NProvider, NProvider, N>, ISerde<N>
{
    private static readonly Type s_enumType = typeof(E);

    static EnumValueProxy()
    {
        var underlyingType = Enum.GetUnderlyingType(s_enumType);
        if (typeof(N) != underlyingType)
        {
            throw new InvalidOperationException(
                $"Type {typeof(N).FullName} is not the underlying type of {s_enumType.FullName} ({underlyingType.FullName})"
            );
        }
    }

    private static readonly EnumValueProxy<E, N, NProvider> s_instance = new();
    static ISerialize<E> ISerializeProvider<E>.Instance => s_instance;
    static IDeserialize<E> IDeserializeProvider<E>.Instance => s_instance;

    private static readonly E[] s_values = Enum.GetValues(s_enumType) as E[]
        ?? throw new InvalidOperationException($"Failed to get enum values for enum type {s_enumType.FullName}");
    
    private static readonly ISerialize<N> s_underlyingSer = SerializeProvider.GetSerialize<N, NProvider>();
    private static readonly IDeserialize<N> s_underlyingDe = DeserializeProvider.GetDeserialize<N, NProvider>();

    public ISerdeInfo SerdeInfo { get; } = new EnumSerdeInfo(s_enumType, SerdeInfoProvider.GetDeserializeInfo<N, NProvider>());

    public void Serialize(E value, ISerializer serializer)
    {
        var underlying = Unsafe.As<E, N>(ref value);
        s_underlyingSer.Serialize(underlying, serializer);
    }

    public E Deserialize(IDeserializer deserializer)
    {
        var (index, enumName) = deserializer.ReadEnumIndex(SerdeInfo);
        if (index == ITypeDeserializer.NonApplicable)
        {
            var value = s_underlyingDe.Deserialize(deserializer);
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
    static ISerialize<E> ISerializeProvider<E>.Instance { get; } = new EnumStringSerProxy<E>();
    public ISerdeInfo SerdeInfo => throw new NotImplementedException();

    public void Serialize(E value, ISerializer serializer)
    {
        var name = Enum.GetName(s_enumType, value)
            ?? throw new InvalidEnumArgumentException($"Failed to get name for enum value {value} of type {s_enumType.FullName}");
        serializer.WriteString(name);
    }
}
