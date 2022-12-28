

using UnityEngine;

public static class MathExtensions
{
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        
    }
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;

    }
}
