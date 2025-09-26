using Game.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    Common,
    Uncommon,
    Rare,
    Legendary,
}

public enum ItemExtraStatType
{
    None,
    PlusAttack,
    Attack,
    PlusSkillAttack,
    SkillAttack,
    AttackSpeed,
    SkillHaste,
    HP,
    CriticalDamage,
    CriticalChance,
    AwakenDuration,
    PlusSpeed,
}

[Serializable]
public class ItemStat
{
    public ItemExtraStatType ItemExtraStatType;
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
    public Color TierColor { get { return TierColors[(int)ItemGrade]; } }
    public static Dictionary<ItemExtraStatType, string> ItemExtraStatTypes = new Dictionary<ItemExtraStatType, string>()
    {
        { ItemExtraStatType.PlusAttack, "추가 공격력" },
        { ItemExtraStatType.Attack, "추가 피해" },
        { ItemExtraStatType.PlusSkillAttack, "추가 스킬 공격력" },
        { ItemExtraStatType.SkillAttack, "스킬 공격력" },
        { ItemExtraStatType.AttackSpeed, "추가 공격 속도" },
        { ItemExtraStatType.SkillHaste, "추가 스킬 가속" },
        { ItemExtraStatType.HP, "추가 체력" },
        { ItemExtraStatType.CriticalDamage, "추가 치명타 피해" },
        { ItemExtraStatType.CriticalChance, "추가 치명타 확률" },
        { ItemExtraStatType.AwakenDuration, "추가 각성 지속 시간" },
        { ItemExtraStatType.PlusSpeed, "추가 이동 속도" },
        { ItemExtraStatType.None, "" },
    };
    public int Id;
    public string ItemName;
    public ItemStat Stat1;
    public ItemStat Stat2;
    public string IconURL;
    public int ItemTalentId;
    public int SynergyId;
    public int DescId;
    public ItemGradeType ItemGrade;
    public void Set(PlayerCharacter player)
    {
        ItemSetterUtil.ApplyStat(player, Stat1);
        ItemSetterUtil.ApplyStat(player, Stat2);
    }
    public ItemData(int id, ItemGradeType itemGradeType, string _name, ItemExtraStatType ability_1Type, int ability_1Value, ItemExtraStatType ability_2Type, int ability_2Value, int itemTalentId, int synergyId, string iconURL, int descriptionId)
    {
        Id = id;
        ItemGrade = itemGradeType;
        ItemName = _name;
        Stat1 = new ItemStat() { ItemExtraStatType = ability_1Type, ItemTriggerType = ItemTriggerType.Always, Value = ability_1Value };
        Stat2 = new ItemStat() { ItemExtraStatType = ability_2Type, ItemTriggerType = ItemTriggerType.Always, Value = ability_2Value };
        ItemTalentId = itemTalentId;
        SynergyId = synergyId;
        IconURL = iconURL;
        DescId = descriptionId;
    }
}



