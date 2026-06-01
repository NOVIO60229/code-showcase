using UnityEngine;

public abstract class GoogleSheetDataBase : ScriptableObject
{
    public string json;

#if UNITY_EDITOR
    public abstract bool TryConvertJsonToData();
#endif
}