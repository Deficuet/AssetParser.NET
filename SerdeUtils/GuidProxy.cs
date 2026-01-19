using Serde;
using System.Buffers;

namespace AssetParser.SerdeUtils;

[GenerateSerde(ForType = typeof(Guid), With = typeof(GuidProxy))]
public partial class GuidProxy : ISerdeProvider<Guid>, ISerde<Guid>
{
    private static readonly GuidProxy s_instance = new();
    static ISerialize<Guid> ISerializeProvider<Guid>.Instance { get; } = s_instance;

    static IDeserialize<Guid> IDeserializeProvider<Guid>.Instance { get; } = s_instance;

    public ISerdeInfo SerdeInfo => ByteArrayProxy.SerdeInfo;

    public void Serialize(Guid value, ISerializer serializer)
    {
        serializer.WriteBytes(value.ToByteArray());
    }

    public Guid Deserialize(IDeserializer deserializer)
    {
        var writer = new ArrayBufferWriter<byte>(16);
        deserializer.ReadBytes(writer);
        return new Guid(writer.GetSpan(16));
    }
}
