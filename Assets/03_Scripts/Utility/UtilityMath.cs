using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectOperation
{
    None,
    Add,
    Subtract,
    Multiply,
    Divide,
}

public interface INumericOps<T>
{
    T Add(T a, T b);
    T Sub(T a, T b);
    T Mul(T a, T b);
    T Div(T a, T b);
}

public readonly struct IntOps : INumericOps<int>
{
    public int Add(int a, int b) => a + b;
    public int Sub(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => b != 0 ? a / b : a; // 0 나눗셈 보호
}

public readonly struct FloatOps : INumericOps<float>
{
    public float Add(float a, float b) => a + b;
    public float Sub(float a, float b) => a - b;
    public float Mul(float a, float b) => a * b;
    public float Div(float a, float b) => Mathf.Abs(b) > 1e-6f ? a / b : a; // 0 나눗셈 보호
}

public readonly struct Vec4Ops : INumericOps<Vector4>
{
    public Vector4 Add(Vector4 a, Vector4 b) => a + b;
    public Vector4 Sub(Vector4 a, Vector4 b) => a - b;
    public Vector4 Mul(Vector4 a, Vector4 b) => new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
    public Vector4 Div(Vector4 a, Vector4 b) => new Vector4(
        Mathf.Abs(b.x) > 1e-6f ? a.x / b.x : a.x,
        Mathf.Abs(b.y) > 1e-6f ? a.y / b.y : a.y,
        Mathf.Abs(b.z) > 1e-6f ? a.z / b.z : a.z,
        Mathf.Abs(b.w) > 1e-6f ? a.w / b.w : a.w
    );
};

public class UtilityMath
{
    public static T ApplyToValue<T, OPSTION>(T a, T b, EffectOperation eOperOption)
        where OPSTION : struct, INumericOps<T>
    {
        OPSTION tOps = default;

        switch (eOperOption)
        {
            case EffectOperation.Add: return tOps.Add(a, b);
            case EffectOperation.Subtract: return tOps.Sub(a, b);
            case EffectOperation.Multiply: return tOps.Mul(a, b);
            case EffectOperation.Divide: return tOps.Div(a, b);
            default: return a;
        }
    }
   
}
