using UnityEngine;

public abstract class GoogleSheetDataBase : ScriptableObject
{
    public string json;

#if UNITY_EDITOR
    public abstract void ConvertJsonToData();
#endif
}