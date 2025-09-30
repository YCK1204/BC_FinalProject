using Game.Monster;
using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum  ItemConditionType
{
    Awaken,
    Always,
    Normal
}

public enum ItemActionType
{
    Always,
    Kill,
    UsingSkill,
    UsingAttack,
    AttackHit,
    StartRound,
    DashEnd,
    OnSynergy
}

[Serializable]
public struct ItemTalentData
{
    public int Id;
    public ItemConditionType ConditionType;
    public ItemActionType itemActionType;
    public float Chance;
    public int SpecialAbilityId;
    public float CoolDown;

    public ItemTalentData(int Id, ItemConditionType conditionType, ItemActionType actionType, float chance, int specialAbilityId, float coolDown)
    {
        this.Id = Id;
        ConditionType = conditionType;
        itemActionType = actionType;
        Chance = chance;
        CoolDown = coolDown;
        SpecialAbilityId = specialAbilityId;
    }
}
