using Game.Player;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTier
{
    Normal,
    Rare,
    Epic,
    Legendary,
    Corruption,
    Sacred
}

public enum ItemStatType
{
    None,
    Attack,
    SkillAttack,
    AttackSpeed,
    SkillHaste,
    Hp,
    CorruptionDuration,
    CriticalChance,
    CriticalDamage
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

public enum ItemModifierType
{
    Add,
    Minus,
    Multiply,
    Divide
}

[Serializable]
public class ItemStat
{
    [SerializeField]
    ItemStatType itemStatType;
    public ItemStatType ItemStatType { get { return itemStatType; } }
    [SerializeField]
    ItemModifierType itemModifierType;
    public ItemModifierType ItemModifierType { get { return itemModifierType; } }
    [SerializeField]
    ItemTriggerType itemTriggerType;
    public ItemTriggerType ItemTriggerType { get { return itemTriggerType; } }
    [SerializeField]
    float value;
    public float Value { get { return value; } }
}

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObject/Item/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    int itemID;
    public int ItemID { get { return itemID; } }
    [SerializeField]
    ItemTier itemTier;
    public ItemTier ItemTier { get { return itemTier; } }
    static List<Color> TierColors { get; } = new List<Color>()
    {
        Color.white,
        Color.green,
        Color.blue,
        Color.orange,
        Color.purple,
        Color.yellow
    };
    public static Dictionary<ItemStatType, string> ItemStatTypes = new Dictionary<ItemStatType, string>()
    {
        { ItemStatType.Attack, "공격력" },
        { ItemStatType.SkillAttack, "스킬 데미지" },
        { ItemStatType.AttackSpeed, "공격속도" },
        { ItemStatType.SkillHaste, "스킬 쿨타임 감소" },
        { ItemStatType.Hp, "최대체력" },
        { ItemStatType.CorruptionDuration, "타락 지속 시간" },
        { ItemStatType.CriticalDamage, "치명타 피해" },
        { ItemStatType.CriticalChance, "치명타 확률" },
        { ItemStatType.None, "" },
    };
    public Color TierColor { get { return TierColors[(int)itemTier]; } }
    [SerializeField]
    string itemName;
    public string ItemName { get { return itemName; } }
    [SerializeField]
    ItemStat stat1;
    public ItemStat Stat1 { get { return stat1; } }
    [SerializeField]
    ItemStat stat2;
    public ItemStat Stat2 { get { return stat2; } }
    [SerializeField]
    Sprite itemIcon;
    public Sprite ItemIcon { get { return itemIcon; } }
    [SerializeField]
    int itemSetSynergyId;
    public int ItemSetSynergyId { get { return itemSetSynergyId; } }

    public void Set(PlayerCharacter player)
    {
        ItemSetterUtil.ApplyStat(player, stat1.ItemStatType, stat1.Value);
        ItemSetterUtil.ApplyStat(player, stat2.ItemStatType, stat1.Value);
    }
}
