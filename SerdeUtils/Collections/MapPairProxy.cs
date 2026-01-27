using AssetParser.Collections;
using Serde;

namespace AssetParser.SerdeUtils.Collections;

public static class MapPairProxy
{
    public sealed class Ser<TFirst, TSecond, TFProvider, TSProvider>
        : ISerializeProvider<MapPair<TFirst, TSecond>>, ISerialize<MapPair<TFirst, TSecond>>
        where TFirst : notnull
        where TFProvider : ISerializeProvider<TFirst>
        where TSProvider : ISerializeProvider<TSecond>
    {
        static ISerialize<MapPair<TFirst, TSecond>> ISerializeProvider<MapPair<TFirst, TSecond>>.Instance { get; } = 
            new Ser<TFirst, TSecond, TFProvider, TSProvider>();

        public ISerdeInfo SerdeInfo { get; } = new MapPairSerdeInfo<TFirst, TSecond>(
            TFProvider.Instance.SerdeInfo,
            TSProvider.Instance.SerdeInfo
        );

        private static readonly ITypeSerialize<TFirst> _firstSer = TypeSerialize.GetOrBox<TFirst, TFProvider>();
        private static readonly ITypeSerialize<TSecond> _secondSer = TypeSerialize.GetOrBox<TSecond, TSProvider>();

        void ISerialize<MapPair<TFirst, TSecond>>.Serialize(MapPair<TFirst, TSecond> value, ISerializer serializer)
        {
            var ser = serializer.WriteType(SerdeInfo);
            _firstSer.Serialize(value.first, ser, SerdeInfo, 0);
            _secondSer.Serialize(value.second, ser, SerdeInfo, 1);
            ser.End(SerdeInfo);
        }
    }

    public sealed class De<TFirst, TSecond, TFProvider, TSProvider>
        : IDeserializeProvider<MapPair<TFirst, TSecond>>, IDeserialize<MapPair<TFirst, TSecond>>
        where TFirst : notnull
        where TFProvider : IDeserializeProvider<TFirst>
        where TSProvider : IDeserializeProvider<TSecond>
    {
        static IDeserialize<MapPair<TFirst, TSecond>> IDeserializeProvider<MapPair<TFirst, TSecond>>.Instance { get; } = 
            new De<TFirst, TSecond, TFProvider, TSProvider>();

        public ISerdeInfo SerdeInfo { get; } = new MapPairSerdeInfo<TFirst, TSecond>(
            TFProvider.Instance.SerdeInfo,
            TSProvider.Instance.SerdeInfo
        );

        private static readonly ITypeDeserialize<TFirst> _firstDe = TypeDeserialize.GetOrBox<TFirst, TFProvider>();
        private static readonly ITypeDeserialize<TSecond> _secondDe = TypeDeserialize.GetOrBox<TSecond, TSProvider>();

        MapPair<TFirst, TSecond> IDeserialize<MapPair<TFirst, TSecond>>.Deserialize(IDeserializer deserializer)
        {
            TFirst first = default!;
            TSecond second = default!;
            byte validate = 0;

            using var de = deserializer.ReadType(SerdeInfo);
            while (true)
            {
                var (index, _) = de.TryReadIndexWithName(SerdeInfo);
                if (index == ITypeDeserializer.EndOfType)
                {
                    break;
                }
                switch (index)
                {
                    case 0:
                        DeserializeException.ThrowIfDuplicate(validate, 0, SerdeInfo);
                        first = _firstDe.Deserialize(de, SerdeInfo, 0);
                        validate |= 1 << 0;
                        break;
                    case 1:
                        DeserializeException.ThrowIfDuplicate(validate, 1, SerdeInfo);
                        second = _secondDe.Deserialize(de, SerdeInfo, 1);
                        validate |= 1 << 1;
                        break;
                    case ITypeDeserializer.IndexNotFound:
                        de.SkipValue(SerdeInfo, index);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected index: " + index);
                }
            }
            if ((validate & 0b11) != 0b11)
            {
                throw DeserializeException.UnassignedMember();
            }
            de.End(SerdeInfo);
            return new MapPair<TFirst, TSecond>(first, second);
        }
    }
}
