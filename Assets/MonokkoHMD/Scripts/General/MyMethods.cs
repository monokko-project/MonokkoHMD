using UnityEngine;

/// <summary>
/// ???????????
/// </summary>
public static class MyMethods
{

    /// <summary>
    /// target?a?b??????(int?)
    /// </summary>
    public static bool IsBetween(int target, int a, int b)
    {
        if (a > b)
        {
            return target <= a && target >= b;
        }
        return target <= b && target >= a;
    }

    /// <summary>
    /// target?a?b??????(float?)
    /// </summary>
    public static bool IsBetween(float target, float a, float b)
    {
        if (a > b)
        {
            return target <= a && target >= b;
        }
        return target <= b && target >= a;
    }

    /// <summary>
    /// target?a?b??????(Vector2?)
    /// </summary>
    public static bool IsBetween(ref Vector2 target, ref Vector2 a, ref Vector2 b)
    {
        return IsBetween(target.x, a.x, b.x) && IsBetween(target.y, a.y, b.y);
    }

    /// <summary>
    /// target?a?b??????(Vector3?)
    /// </summary>
    public static bool IsBetween(ref Vector3 target, ref Vector3 a, ref Vector3 b)
    {
        return IsBetween(target.x, a.x, b.x) && IsBetween(target.y, a.y, b.y) && IsBetween(target.z, a.z, b.z);
    }

    /// <summary>
    /// target?a?b??????(Vector2Int?)
    /// </summary>
    public static bool IsBetween(ref Vector2Int target, ref Vector2Int a, ref Vector2Int b)
    {
        return IsBetween(target.x, a.x, b.x) && IsBetween(target.y, a.y, b.y);
    }

    /// <summary>
    /// target?a?b??????(Vector3Int?)
    /// </summary>
    public static bool IsBetween(ref Vector3Int target, ref Vector3Int a, ref Vector3Int b)
    {
        return IsBetween(target.x, a.x, b.x) && IsBetween(target.y, a.y, b.y) && IsBetween(target.z, a.z, b.z);
    }

    /// <summary>
    /// float???????????????????
    /// </summary>
    public static float RoundFloat(float value, int digit = 4)
    {
        float times = Mathf.Pow(10, (float)digit);
        value *= times;
        value = Mathf.Round(value) / times;
        return value;
    }

    /// <summary>
    /// ?????(?????)?????(float)
    /// </summary>
    public static float Repeat(this float value, float min, float max)
    {
        return Mathf.Repeat(value - min, max - min) + min;
    }

    /// <summary>
    /// ?????(?????)?????(int)
    /// </summary>
    public static int Repeat(this int value, int min, int max)
    {
        return (int)Mathf.Repeat(value - min, max - min) + min;
    }
}