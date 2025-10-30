using Game.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
public class ItemData
{
    static List<Color> TierColors { get; } = new List<Color>()
    {
        Color.white,
        Color.green,
        Color.blue,
        Color.orange,
    };
    [JsonIgnore]
    public Color TierColor { get { return TierColors[(int)ItemGrade]; } }
    [JsonIgnore]
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
    public int ItemEffectId;
    public int SynergyId;
    public ItemGradeType ItemGrade;
    public ItemEffectData EffectData;
    public string SynergyText;
    public string EffectText;
    public Material Material;
    public void Set(PlayerCharacter player)
    {
        ItemSetterUtil.ApplyStat(player, Stat1.ItemExtraStatType, Stat1.Value);
        ItemSetterUtil.ApplyStat(player, Stat2.ItemExtraStatType, Stat2.Value);

        if (SynergyId != 0)
        {
            Manager.Data.ItemsData.Synergy.TryGetValue(SynergyId, out var synergyData);
            Manager.Data.ItemsData.Effect.TryGetValue(synergyData.ItemEffectID, out var data);
            Manager.Data.ItemsData.Description.TryGetValue(data.DescId, out var descData);
            SynergyText = descData.Korean;
            Debug.Log($"SynergyText: {SynergyText} ");
        }

        if (ItemEffectId == 0)
            return;
        Manager.Data.ItemsData.Effect.TryGetValue(ItemEffectId, out var effectData);
        if (effectData == null)
        {
            Debug.LogError($"ItemEffectId {ItemEffectId} not found");
            return;
        }
        EffectData = effectData;
        EffectData.Set();
        
        Manager.Data.ItemsData.Description.TryGetValue(effectData.DescId, out var desc);
        EffectText = desc.Korean;
    }
    public ItemData(int id, ItemGradeType itemGradeType, string _name, ItemExtraStatType ability_1Type, float ability_1Value, ItemExtraStatType ability_2Type, float ability_2Value, int itemEffectId, int synergyId, string iconURL)
    {
        Id = id;
        ItemGrade = itemGradeType;
        ItemName = _name;
        Stat1 = new ItemStat() { ItemExtraStatType = ability_1Type, ItemTriggerType = ItemTriggerType.Always, Value = ability_1Value };
        Stat2 = new ItemStat() { ItemExtraStatType = ability_2Type, ItemTriggerType = ItemTriggerType.Always, Value = ability_2Value };
        ItemEffectId = itemEffectId;
        SynergyId = synergyId;
        IconURL = iconURL;
        EffectData = default(ItemEffectData);
#if UNITY_EDITOR
        string matName = "Material";
        string toInsert = "";
        switch (ItemGrade)
        {
            case ItemGradeType.Legendary:
                toInsert = "Legendary";
                break;
            default:
                toInsert = "Normal";
                break;
        }
        matName = matName.Insert(0, toInsert);
        Material = Extension.LoadWithAddresssableByGroup<Material>($"{matName}", "Item");
#endif
    }
}