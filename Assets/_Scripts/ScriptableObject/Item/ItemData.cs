using Game.Player;
using GoogleSheetsToUnity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemStatType
{
    none,           // 소문자로 변경 (실제 데이터에 맞춤)
    attack,
    skillAttack,
    attackSpeed,
    HP,
    criticalDamage,
    criticalChance,
    skillHaste,
}
public enum ItemTriggerType
{
    OnHit,
    OnAttacked,
    OnHeal,
    OnKill,
    OnStageClear,
    Always
}
public enum ItemGradeType
{
    common,
    uncommon,
    rare,
    legendary,
}

[Serializable]
public class ItemStat
{
    public ItemStatType ItemStatType;
    public ItemTriggerType ItemTriggerType;
    public float Value;
}


[Serializable]
public struct ItemData
{
    static List<Color> TierColors { get; } = new List<Color>()
    {
        Color.white,
        Color.green,
        Color.blue,
        Color.orange,
    };
    //public Color TierColor { get { return TierColors[(int)itemTier]; } }
    public static Dictionary<ItemStatType, string> ItemStatTypes = new Dictionary<ItemStatType, string>()
    {
        { ItemStatType.attack, "공격력" },
        { ItemStatType.attackSpeed, "공격속도" },
        { ItemStatType.HP, "최대체력" },
        { ItemStatType.criticalChance, "치명타 확률" },
        { ItemStatType.criticalDamage, "치명타 피해" },
        { ItemStatType.skillAttack, "스킬 공격력" },
        { ItemStatType.skillHaste, "스킬 가속" },
        { ItemStatType.none, "" },
    };
    public int Id { get; private set; }
    public string ItemName { get; private set; }
    public ItemStat Stat1 { get; private set; }
    public ItemStat Stat2 { get; private set; }
    public string IconRoute { get; private set; }
    public int ItemTalent { get; private set; }
    public int SynergyId { get; private set; }
    public int Desc { get; private set; }
    public ItemGradeType ItemGradeType { get; private set; }
    public void Set(PlayerCharacter player)
    {
        ItemSetterUtil.SetStat(player, Stat1);
        ItemSetterUtil.SetStat(player, Stat2);
    }
    public ItemData(int id, ItemGradeType itemGradeType, string _name, ItemStatType ability_1Type, int ability_1Value, ItemStatType ability_2Type, int ability_2Value, int itemTalentId, int synergyId, string iconRoute, int descriptionId)
    {
        Id = id;
        ItemGradeType = itemGradeType;
        ItemName = _name;
        Stat1 = new ItemStat() { ItemStatType = ability_1Type, ItemTriggerType = ItemTriggerType.Always, Value = ability_1Value };
        Stat2 = new ItemStat() { ItemStatType = ability_2Type, ItemTriggerType = ItemTriggerType.Always, Value = ability_2Value };
        ItemTalent = itemTalentId;
        SynergyId = synergyId;
        IconRoute = iconRoute;
        Desc = descriptionId;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemDataReader))]
public class ItemDataReaderEditor : Editor
{
    ItemDataReader data;

    void OnEnable()
    {
        data = (ItemDataReader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("\n\n스프레드 시트 읽어오기");

        if (GUILayout.Button("데이터 읽기(API 호출)"))
        {
            UpdateStats(UpdateMethodOne);
            data.DataList.Clear();
        }
    }

    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(data.associatedSheet, data.associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethodOne(GstuSpreadSheet ss)
    {
        for (int i = data.START_ROW_LENGTH; i <= data.END_ROW_LENGTH; ++i)
        {
            data.UpdateStats(ss.rows[i], i);
        }

        EditorUtility.SetDirty(target);
    }
}
#endif

