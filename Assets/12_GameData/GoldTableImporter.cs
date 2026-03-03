#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class SOGoldTableImporter
{
    [MenuItem("Tools/Import/Gold Table CSV")]
    public static void Import()
    {
        // 1. CSV 선택
        string strPath = EditorUtility.OpenFilePanel("Select Gold Table CSV", "", "csv");
        if (string.IsNullOrEmpty(strPath))
            return;

        string[] strLines = File.ReadAllLines(strPath);
        if (strLines.Length <= 1)
        {
            Debug.LogError("CSV is empty or has no data rows");
            return;
        }

        // 2. 기존 SO 있으면 로드, 없으면 생성
        const string strAssetPath = "Assets/12_GameData/SOGoldTable.asset";
        SOGoldTable SOTable = AssetDatabase.LoadAssetAtPath<SOGoldTable>(strAssetPath);

        if (SOTable == null)
        {
            SOTable = ScriptableObject.CreateInstance<SOGoldTable>();
            AssetDatabase.CreateAsset(SOTable, strAssetPath);
        }

        // 3. 초기화
        SOTable.listGoldRange.Clear();

        // 4. CSV 파싱
        for (int i = 1; i < strLines.Length; ++i) // 0번은 헤더
        {
            string line = strLines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] strCols = line.Split(',');
            if (strCols.Length < 4)
            {
                Debug.LogWarning($"Invalid row at line {i + 1}");
                continue;
            }

            SOGoldTable.GoldRange pGoldRange = new SOGoldTable.GoldRange();
            pGoldRange.MinLevel = int.Parse(strCols[0]);
            pGoldRange.MaxLevel = int.Parse(strCols[1]);
            pGoldRange.MinGold = int.Parse(strCols[2]);
            pGoldRange.MaxGold = int.Parse(strCols[3]);

            SOTable.listGoldRange.Add(pGoldRange);
        }

        // 5. MinLevel 기준 정렬 
        SOTable.listGoldRange.Sort(
            (a, b) => a.MinLevel.CompareTo(b.MinLevel)
        );

        // 6. 저장
        EditorUtility.SetDirty(SOTable);
        AssetDatabase.SaveAssets();

        Debug.Log("Gold Table CSV import completed");
    }
}
#endif