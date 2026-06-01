using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Tools/Google Sheet Sync Tool", fileName = "GoogleSheetSyncTool")]
public class GoogleSheetSyncTool : ScriptableObject
{
#if UNITY_EDITOR
    [BoxGroup("設定")]
    [LabelText("GAS Web App URL")]
    [SerializeField] string gasUrl;

    [BoxGroup("設定")]
    [Button("全部同步", ButtonSizes.Large), GUIColor(0.3f, 0.85f, 0.4f)]
    void SyncAll() => _ = SyncAllAsync();

    [BoxGroup("同步目標")]
    [InfoBox("拖入繼承 GoogleSheetDataBase 的 ScriptableObject。\n同步時會以各 Asset 名稱作為 Sheet Tab 名稱查詢。")]
    [SerializeField] List<GoogleSheetDataBase> targets = new();

    async Task SyncAllAsync()
    {
        if (string.IsNullOrWhiteSpace(gasUrl))
        {
            EditorUtility.DisplayDialog("錯誤", "請填入 GAS Web App URL", "OK");
            return;
        }

        if (targets == null || targets.Count == 0)
        {
            EditorUtility.DisplayDialog("錯誤", "同步目標列表是空的", "OK");
            return;
        }

        int success = 0, failed = 0;

        using var client = new HttpClient();
        // client.DefaultRequestHeaders.Add("User-Agent", "UnityEditor");

        for (int i = 0; i < targets.Count; i++)
        {
            var so = targets[i];

            if (so == null)
            {
                Debug.LogWarning("[GoogleSheetSyncTool] 列表中有 null 項目，跳過");
                failed++;
                continue;
            }

            EditorUtility.DisplayProgressBar(
                "Google Sheet 同步中",
                $"正在同步：{so.name}  ({i + 1} / {targets.Count})",
                (float)i / targets.Count);

            try
            {
                string url = $"{gasUrl.TrimEnd('?', '&')}?sheet={Uri.EscapeDataString(so.name)}";
                string json = await client.GetStringAsync(url);

                so.json = json;
                if (so.TryConvertJsonToData())
                {
                    success++;
                }
                else
                {
                    Debug.LogError($"[GoogleSheetSyncTool] JSON 轉換失敗：{so.name}");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GoogleSheetSyncTool] 同步失敗 {so.name}：{e.Message}");
                failed++;
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "同步完成",
            $"成功：{success} 個\n失敗：{failed} 個",
            "OK");
    }
#endif
}
