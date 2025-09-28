using System;
using UnityEngine;

public enum SynergyEffectType
{
    Duration,
    Always,
    Creation
}

[Serializable]
public struct ItemSynergyData
{
    public int Id;
    public int Count;
    public string Name;
    public ItemActionType ActionType;
    public float Chance;
    public int SpecialAbilityID;
    public float CoolDown;
    public SynergyEffectType EffectType;
    public int DescID;
    public ItemSynergyData(int id, int count, string name, ItemActionType actionType, float chance, int specialAbilityID, float coolDown, SynergyEffectType effectType, int descID)
    {
        Id = id;
        Count = count;
        Name = name;
        ActionType = actionType;
        Chance = chance;
        SpecialAbilityID = specialAbilityID;
        CoolDown = coolDown;
        EffectType = effectType;
        DescID = descID;
    }
}
