using System;
using UnityEngine;

[Serializable]
public struct ItemBuffData
{
    public int Id;
    public float Duration;
    public int MaxCount;
    public ItemExtraStatType Ability1;
    public float Ability1Value;
    public ItemExtraStatType Ability2;
    public float Ability2Value;
    public string IconURL;
    public string Name;
    public int DescriptionId;
    public ItemBuffData(int id, float duration, int maxCount, ItemExtraStatType ability1, float ability1Value, ItemExtraStatType ability2, float ability2Value, string iconURL, string name, int descriptionId)
    {
        Id = id;
        Duration = duration;
        MaxCount = maxCount;
        Ability1 = ability1;
        Ability1Value = ability1Value;
        Ability2 = ability2;
        Ability2Value = ability2Value;
        IconURL = iconURL;
        Name = name;
        DescriptionId = descriptionId;
    }
}
