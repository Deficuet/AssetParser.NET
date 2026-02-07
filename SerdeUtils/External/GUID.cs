using AssetParser.SerdeUtils.Delegate;
using Serde;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AssetParser.SerdeUtils.External;

[GenerateSerde]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public partial struct UInt32x4Guid : ISerdeDelegate<UInt32x4Guid, Guid>
{
    [SerdeMemberOptions(Rename = "data[0]")]
    public uint data0;
    [SerdeMemberOptions(Rename = "data[1]")]
    public uint data1;
    [SerdeMemberOptions(Rename = "data[2]")]
    public uint data2;
    [SerdeMemberOptions(Rename = "data[3]")]
    public uint data3;

    public static ISerdeInfo GetDelegateSerdeInfo() => s_serdeInfo;
    public static UInt32x4Guid PopulateFrom(Guid obj) => Unsafe.As<Guid, UInt32x4Guid>(ref obj);
    public Guid ConvertTo() => Unsafe.As<UInt32x4Guid, Guid>(ref this);
}

public class GuidProxy : ISerde<Guid>, ISerdeProvider<Guid>
{
    private static readonly GuidProxy s_instance = new();
    static ISerialize<Guid> ISerializeProvider<Guid>.Instance => s_instance;
    static IDeserialize<Guid> IDeserializeProvider<Guid>.Instance => s_instance;

    private static ISerialize<Guid> Ser => SerializeProvider.GetSerialize<Guid, DelegateProxy<Guid, UInt32x4Guid>>();
    private static IDeserialize<Guid> De => DeserializeProvider.GetDeserialize<Guid, DelegateProxy<Guid, UInt32x4Guid>>();

    public ISerdeInfo SerdeInfo => Ser.SerdeInfo;

    void ISerialize<Guid>.Serialize(Guid value, ISerializer serializer)
    {
        Ser.Serialize(value, serializer);
    }

    Guid IDeserialize<Guid>.Deserialize(IDeserializer deserializer)
    {
        if (deserializer.TryReadGuid(out var ret))
        {
            return ret;
        }
        return De.Deserialize(deserializer);
    }
}

public class GuidStringSerProxy : ISerialize<Guid>, ISerializeProvider<Guid>
{
    private static readonly GuidStringSerProxy s_instance = new();
    public static ISerialize<Guid> Instance => s_instance;

    private static readonly ISerdeInfo s_serdeInfo = Serde.SerdeInfo.MakePrimitive(typeof(Guid).FullName!, PrimitiveKind.String);
    public ISerdeInfo SerdeInfo => s_serdeInfo;

    public void Serialize(Guid value, ISerializer serializer)
    {
        serializer.WriteString(value.ToString());
    }
}
