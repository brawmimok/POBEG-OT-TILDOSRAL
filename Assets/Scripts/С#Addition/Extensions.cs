using UnityEngine;

public static class VectorUtils
{
    public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(
            Mathf.Clamp(value.x, min.x, max.x),
            Mathf.Clamp(value.y, min.y, max.y)
        );
    }
    public static Vector2 Abs(this Vector2 value)
    {
        return new Vector2(
            Mathf.Abs(value.x),
            Mathf.Abs(value.y)
        );
    }
}