using AssetParser.SerdeUtils.Delegate;
using Serde;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AssetParser.SerdeUtils.External;


[GenerateSerde]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public partial struct Vector2f : ISerdeDelegate<Vector2f, Vector2>
{
    public float x;
    public float y;

    public static ISerdeInfo GetDelegateSerdeInfo() => s_serdeInfo;
    public static Vector2f PopulateFrom(Vector2 obj) => Unsafe.As<Vector2, Vector2f>(ref obj);
    public Vector2 ConvertTo() => Unsafe.As<Vector2f, Vector2>(ref this);
}

[GenerateSerde]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public partial struct Vector3f : ISerdeDelegate<Vector3f, Vector3>
{
    public float x;
    public float y;
    public float z;

    public static ISerdeInfo GetDelegateSerdeInfo() => s_serdeInfo;
    public static Vector3f PopulateFrom(Vector3 obj) => Unsafe.As<Vector3, Vector3f>(ref obj);
    public Vector3 ConvertTo() => Unsafe.As<Vector3f, Vector3>(ref this);
}

[GenerateSerde]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public partial struct Vector4f : ISerdeDelegate<Vector4f, Vector4>, ISerdeDelegate<Vector4f, Quaternion>
{
    public float x;
    public float y;
    public float z;
    public float w;

    public static ISerdeInfo GetDelegateSerdeInfo() => s_serdeInfo;
    static Vector4f ISerdeDelegate<Vector4f, Vector4>.PopulateFrom(Vector4 obj) => Unsafe.As<Vector4, Vector4f>(ref obj);
    static Vector4f ISerdeDelegate<Vector4f, Quaternion>.PopulateFrom(Quaternion obj) => Unsafe.As<Quaternion, Vector4f>(ref obj);
    Vector4 ISerdeDelegate<Vector4f, Vector4>.ConvertTo() => Unsafe.As<Vector4f, Vector4>(ref this);
    Quaternion ISerdeDelegate<Vector4f, Quaternion>.ConvertTo() => Unsafe.As<Vector4f, Quaternion>(ref this);
}

[GenerateSerde]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public partial struct Matrix4x4f : ISerdeDelegate<Matrix4x4f, Matrix4x4>
{
    public float e00;
    public float e01;
    public float e02;
    public float e03;
    public float e10;
    public float e11;
    public float e12;
    public float e13;
    public float e20;
    public float e21;
    public float e22;
    public float e23;
    public float e30;
    public float e31;
    public float e32;
    public float e33;

    public static ISerdeInfo GetDelegateSerdeInfo() => s_serdeInfo;
    public static Matrix4x4f PopulateFrom(Matrix4x4 obj) => Unsafe.As<Matrix4x4, Matrix4x4f>(ref obj);
    public Matrix4x4 ConvertTo() => Unsafe.As<Matrix4x4f, Matrix4x4>(ref this);
}
