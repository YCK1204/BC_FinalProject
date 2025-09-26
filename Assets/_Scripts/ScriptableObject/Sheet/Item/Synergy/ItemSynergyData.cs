using System;
using UnityEngine;

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
    public int DescID;
    public ItemSynergyData(int id, int count, string name, ItemActionType actionType, float chance, int specialAbilityID, float coolDown, int descID)
    {
        Id = id;
        Count = count;
        Name = name;
        ActionType = actionType;
        Chance = chance;
        SpecialAbilityID = specialAbilityID;
        CoolDown = coolDown;
        DescID = descID;
    }
}
