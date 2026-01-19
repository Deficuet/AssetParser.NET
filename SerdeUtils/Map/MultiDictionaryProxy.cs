using AssetParser.Collections;
using Serde;

namespace AssetParser.SerdeUtils.Map;

public static class MultiDictionaryProxy
{
    public sealed class Ser<TK, TV, TKProvider, TVProvider>
        : ISerializeProvider<MultiDictionary<TK, TV>>, ISerialize<MultiDictionary<TK, TV>>
        where TK : notnull 
        where TKProvider: ISerializeProvider<TK>
        where TVProvider: ISerializeProvider<TV>
    {
        static ISerialize<MultiDictionary<TK, TV>> ISerializeProvider<MultiDictionary<TK, TV>>.Instance { get; } = 
            new Ser<TK, TV, TKProvider, TVProvider>();

        private readonly static MapPairSerdeInfo<TK, TV> s_pairInfo = new(
            TKProvider.Instance.SerdeInfo,
            TVProvider.Instance.SerdeInfo
        );

        public ISerdeInfo SerdeInfo { get; } = Serde.SerdeInfo.MakeEnumerable(
            typeof(List<KeyValuePair<TK, TV>>).ToString(), s_pairInfo
        );

        private readonly ITypeSerialize<TK> _keySer = TypeSerialize.GetOrBox<TK, TKProvider>();
        private readonly ITypeSerialize<TV> _valSer = TypeSerialize.GetOrBox<TV, TVProvider>();

        void ISerialize<MultiDictionary<TK, TV>>.Serialize(MultiDictionary<TK, TV> value, ISerializer serializer)
        {
            var enumerable = serializer.WriteCollection(SerdeInfo, value.ValueCount);
            foreach (var pair in value)
            {
                foreach (var item in pair.Value)
                {
                    var typeSer = serializer.WriteType(s_pairInfo);
                    _keySer.Serialize(pair.Key, typeSer, s_pairInfo, 0);
                    _valSer.Serialize(item, typeSer, s_pairInfo, 1);
                    typeSer.End(s_pairInfo);
                }
            }
            enumerable.End(SerdeInfo);
        }
    }

    public sealed class De<TK, TV, TKProvider, TVProvider>
        : DeListBase<
            De<TK, TV, TKProvider, TVProvider>, 
            MapPair<TK, TV>,
            MultiDictionary<TK, TV>, 
            MultiDictionary<TK, TV>,
            MapPairProxy.De<TK, TV, TKProvider, TVProvider>
        >
        where TK : notnull
        where TKProvider : IDeserializeProvider<TK>
        where TVProvider : IDeserializeProvider<TV>
    {
        public override ISerdeInfo SerdeInfo { get; } = Serde.SerdeInfo.MakeEnumerable(
            typeof(List<MapPair<TK, TV>>).ToString(),
            new MapPairSerdeInfo<TK, TV>(
                TKProvider.Instance.SerdeInfo,
                TVProvider.Instance.SerdeInfo
            )
        );

        protected override void Add(MultiDictionary<TK, TV> builder, MapPair<TK, TV> item)
        {
            builder.Add(item.first, item.second);
        }

        protected override MultiDictionary<TK, TV> GetBuilder(int? sizeOpt)
        {
            return [];
        }

        protected override MultiDictionary<TK, TV> ToList(MultiDictionary<TK, TV> builder)
        {
            return builder;
        }
    }
}
