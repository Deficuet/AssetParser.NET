using AssetParser.SerdeUtils.Map;
using Serde;

namespace AssetParser.Collections;

[SerdeTypeOptions(Proxy = typeof(MultiDictionaryProxy))]
public class MultiDictionary<K, V>: Dictionary<K, List<V>>
    where K: notnull
{
    private int _valueCount;

    public int ValueCount => _valueCount;

    public new int Count => _valueCount;

    public int KeyCount => base.Count;

    public MultiDictionary(): base() { }

    public MultiDictionary(IEqualityComparer<K>? comparer): base(comparer) { }

    public void Add(K key, V value)
    {
        if (TryGetValue(key, out var list))
        {
            list.Add(value);
        }
        else
        {
            var newList = new List<V>()
            {
                value
            };
            base[key] = newList;
        }
        _valueCount++;
    }

    public void AddRange(K key, IEnumerable<V> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (TryGetValue(key, out var list))
        {
            foreach (var v in values)
            {
                list.Add(v);
            }
        }
        else
        {
            base[key] = [.. values];
        }
        _valueCount += values.Count();
    }

    public bool Remove(K key, V value)
    {
        if (!TryGetValue(key, out var list))
        {
            return false;
        }
        bool removed = list.Remove(value);
        if (!removed)
        {
            return false;
        }
        _valueCount--;
        if (list.Count == 0)
        {
            base.Remove(key);
        }
        return true;
    }

    public new bool Remove(K key)
    {
        if (!TryGetValue(key, out var list))
        {
            return false;
        }
        _valueCount -= list.Count;
        return base.Remove(key);
    }

    public new void Clear()
    {
        base.Clear();
        _valueCount = 0;
    }

    public IReadOnlyList<V> GetValues(K key)
    {
        if (TryGetValue(key, out var list))
        {
            return [.. list];
        }
        return [];
    }

    public bool TryGetValues(K key, out List<V>? values)
    {
        return TryGetValue(key, out values);
    }

    public IEnumerable<V> EnumerateValues()
    {
        return base.Values.SelectMany(item => item);
    }
}
