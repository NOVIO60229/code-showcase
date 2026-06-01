using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Data/GameResultData")]
public class GameResultDataSO : GoogleSheetDataBase
{
    [SerializeField]
    List<GameResultData> resultDataList;

    public IReadOnlyList<GameResultData> ResultDataList => resultDataList;

#if UNITY_EDITOR
    [Button]
    public override bool TryConvertJsonToData()
    {
        try
        {
            resultDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GameResultData>>(json);
            EditorUtility.SetDirty(this);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameResultDataSO] JSON 轉換失敗：{e.Message}");
            return false;
        }
    }
#endif
}