using Serde;

namespace AssetParser.SerdeUtils.Delegate;

public interface ISerdeDelegate<TSelf, T>
{
    abstract static ISerdeInfo GetDelegateSerdeInfo();
    abstract static TSelf PopulateFrom(T obj);
    abstract T ConvertTo();
}
