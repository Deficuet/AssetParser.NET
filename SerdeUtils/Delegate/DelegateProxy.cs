using Serde;

namespace AssetParser.SerdeUtils.Delegate;

public class DelegateProxy<T, TDelegate> : ISerde<T>, ISerdeProvider<T>
    where TDelegate : ISerdeDelegate<TDelegate, T>, ISerializeProvider<TDelegate>, IDeserializeProvider<TDelegate>
{
    private static readonly DelegateProxy<T, TDelegate> s_instance = new();
    static ISerialize<T> ISerializeProvider<T>.Instance => s_instance;

    static IDeserialize<T> IDeserializeProvider<T>.Instance => s_instance;

    public ISerdeInfo SerdeInfo { get; } = TDelegate.GetDelegateSerdeInfo();

    private static readonly ISerialize<TDelegate> s_delegateSer = SerializeProvider.GetSerialize<TDelegate>();

    private static readonly IDeserialize<TDelegate> s_delegateDes = DeserializeProvider.GetDeserialize<TDelegate>();

    public void Serialize(T value, ISerializer serializer)
    {
        var delegateObj = TDelegate.PopulateFrom(value);
        s_delegateSer.Serialize(delegateObj, serializer);
    }

    public T Deserialize(IDeserializer deserializer)
    {
        var delegateObj = s_delegateDes.Deserialize(deserializer);
        return delegateObj.ConvertTo();
    }
}
