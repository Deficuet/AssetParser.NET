using Serde;

namespace AssetParser.SerdeUtils.Delegate;

public interface ISerdeDelegate<TSelf, T>
{
    static abstract ISerdeInfo GetDelegateSerdeInfo();
    static abstract TSelf PopulateFrom(T obj);
    T ConvertTo();
}
