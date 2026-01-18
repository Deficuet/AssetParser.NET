using Serde;

namespace AssetParser.SerdeUtils.Enums;

public class EnumProxy<E, N, NProvider>: ISerdeProvider<E>, ISerde<E>
    where E : struct, Enum
    where NProvider: ISerdeProvider<NProvider, NProvider, N>, ISerde<N>
{
    private static readonly EnumProxy<E, N, NProvider> s_instance = new();
    static ISerialize<E> ISerializeProvider<E>.Instance => s_instance;
    static IDeserialize<E> IDeserializeProvider<E>.Instance => s_instance;

    private static readonly E[] s_values = Enum.GetValues<E>();
    private static readonly ITypeDeserialize<N> s_underlyingDe = TypeDeserialize.GetOrBox<N, NProvider>();

    public ISerdeInfo SerdeInfo { get; } = new EnumSerdeInfo<E>(SerdeInfoProvider.GetDeserializeInfo<N, NProvider>());

    public void Serialize(E value, ISerializer serializer)
    {
        var name = Enum.GetName(value) 
            ?? throw new InvalidOperationException(
                $"Cannot serialize unnamed enum value ({value}) of enum <{SerdeInfo.Name}>"
            );
        serializer.WriteString(name);
    }

    public E Deserialize(IDeserializer deserializer)
    {
        using var de = deserializer.ReadType(SerdeInfo);
        var (index, errorName) = de.TryReadIndexWithName(SerdeInfo);
        if (index == ITypeDeserializer.IndexNotFound)
        {
            throw DeserializeException.UnknownMember(errorName!, SerdeInfo);
        }
        if (index == ITypeDeserializer.EndOfType)
        {
            return (E)(object)s_underlyingDe.Deserialize(de, SerdeInfo, index)!;
        }
        return s_values[index];
    }
}
