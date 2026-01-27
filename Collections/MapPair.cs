using AssetParser.SerdeUtils.Collections;
using Serde;

namespace AssetParser.Collections;

[SerdeTypeOptions(Proxy = typeof(MapPairProxy))]
public readonly struct MapPair<TFirst, TSecond>(TFirst first, TSecond second) : IEquatable<MapPair<TFirst, TSecond>>
{
    public readonly TFirst first = first;
    public readonly TSecond second = second;

    public void Deconstruct(out TFirst first, out TSecond second)
    {
        first = this.first;
        second = this.second;
    }

    public bool Equals(MapPair<TFirst, TSecond> other)
    {
        return EqualityComparer<TFirst>.Default.Equals(first, other.first) &&
               EqualityComparer<TSecond>.Default.Equals(second, other.second);
    }

    public override bool Equals(object? obj)
    {
        return obj is MapPair<TFirst, TSecond> pair && Equals(pair);
    }

    public static bool operator ==(MapPair<TFirst, TSecond> left, MapPair<TFirst, TSecond> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MapPair<TFirst, TSecond> left, MapPair<TFirst, TSecond> right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(first, second);
    }
}
