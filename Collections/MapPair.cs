using AssetParser.SerdeUtils.Collections;
using Serde;

namespace AssetParser.Collections;

[SerdeTypeOptions(Proxy = typeof(MapPairProxy))]
public readonly struct MapPair<TFirst, TSecond>(TFirst first, TSecond second)
{
    public readonly TFirst first = first;
    public readonly TSecond second = second;

    public void Deconstruct(out TFirst first, out TSecond second)
    {
        first = this.first;
        second = this.second;
    }
}
