using UnityEngine;

[CreateAssetMenu]
public class MyAppSetting : ScriptableObject
{
    public PakageType pakageType;

    // Pakage
    public enum PakageType
    {
        Exe_Product,
        Exe_Trial,
        Apk_Product,
        Apk_Trial
    }

    // SetCursor(cursorTexs[(int)CursorType])
    public Texture2D[] cursorTexs;
    public enum CursorType
    {
        Default,
        OnPointing
    }
}