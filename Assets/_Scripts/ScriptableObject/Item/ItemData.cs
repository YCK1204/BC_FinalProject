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
    AttackPower,
    AttackSpeed,
    MaxHp,
    MoveSpeed,
    Corruption,
    ExtraDamage
}

public enum ItemTriggerType
{
    OnHit,
    OnAttacked,
    OnHeal,
    OnKill,
    OnStageClear
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

public enum ItemSetEffectType
{
    CthughaFlame,      // 크투가의 살아있는 불꽃
    CthulhuWrath,      // 크툴루의 분노  
    YigCurse,          // 이그의 저주
    HasturWind,        // 하스터의 살을 에는 바람
    MentalBreakdown,   // 정신 붕괴
    IronWill           // 불굴의 의지
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
    [SerializeField, TextArea]
    string itemDescription;
    public string ItemDescription { get { return itemDescription; } }
    [SerializeField]
    ItemSetEffectType itemSetEffectType;
    public ItemSetEffectType ItemSetEffectType { get { return itemSetEffectType; } }
}
