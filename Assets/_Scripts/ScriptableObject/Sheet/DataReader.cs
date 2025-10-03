using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public abstract class DataReaderBase<T> : ScriptableObject
{
    [Header("시트의 주소")][SerializeField] public string associatedSheet = "";
    [Header("스프레드 시트의 시트 이름")][SerializeField] public string associatedWorksheet = "";
    [Header("읽기 시작할 행 번호")][SerializeField] public int START_ROW_LENGTH = 2;
    [Header("읽을 마지막 행 번호")][SerializeField] public int END_ROW_LENGTH = -1;

    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")] public List<T> DataList = new List<T>();
    public abstract void UpdateStats(List<GSTU_Cell> list);
}

#if UNITY_EDITOR
[CustomEditor(typeof(DataReaderBase<>), true)]
public class DataReaderEditor : Editor
{
    protected object targetDataReader;

    void OnEnable()
    {
        targetDataReader = target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("\n\n스프레드 시트 읽어오기");

        if (GUILayout.Button("데이터 읽기(API 호출)"))
        {
            var list = targetDataReader.GetType().GetField("DataList").GetValue(targetDataReader) as IList;
            list.Clear();
            UpdateStats(UpdateMethodOne);
        }
    }

    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        string associatedSheet = (string)targetDataReader.GetType().GetField("associatedSheet").GetValue(targetDataReader);
        string associatedWorksheet = (string)targetDataReader.GetType().GetField("associatedWorksheet").GetValue(targetDataReader);
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethodOne(GstuSpreadSheet ss)
    {
        var start = (int)targetDataReader.GetType().GetField("START_ROW_LENGTH").GetValue(targetDataReader);
        var end = (int)targetDataReader.GetType().GetField("END_ROW_LENGTH").GetValue(targetDataReader);
        var list = (targetDataReader.GetType().GetField("DataList").GetValue(targetDataReader) as IList);
        list.Clear();

        for (int i = start; i <= end; ++i)
        {
            var rowCells = ss.rows[i];
            object[] parameters = new object[] { rowCells };
            targetDataReader.GetType().GetMethod("UpdateStats").Invoke(targetDataReader, parameters);
        }

        EditorUtility.SetDirty(target);
    }
}
#endif