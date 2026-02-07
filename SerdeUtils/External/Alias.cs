using AssetParser.SerdeUtils.Delegate;
using Serde;
using System.Numerics;

namespace AssetParser.SerdeUtils.External;

[GenerateSerde(ForType = typeof(Vector2), With = typeof(DelegateProxy<Vector2, Vector2f>))]
public partial class Vector2Proxy;

[GenerateSerde(ForType = typeof(Vector3), With = typeof(DelegateProxy<Vector3, Vector3f>))]
public partial class Vector3Proxy;

[GenerateSerde(ForType = typeof(Vector4), With = typeof(DelegateProxy<Vector4, Vector4f>))]
public partial class Vector4Proxy;

[GenerateSerde(ForType = typeof(Quaternion), With = typeof(DelegateProxy<Quaternion, Vector4f>))]
public partial class QuaternionProxy;

[GenerateSerde(ForType = typeof(Matrix4x4), With = typeof(DelegateProxy<Matrix4x4, Matrix4x4f>))]
public partial class Matrix4x4Proxy;
