using System;
using UnityEngine;

public enum ItemCreateAreaPosType
{
    Player,
    Collision
}

[Serializable]
public struct ItemAreaData
{
    public int Id;
    public float AnmationDuration;
    public ItemCreateAreaPosType CreateAreaPosType;
    public float Radius;
    public float Damage;
    public int AttackCount;

    public ItemAreaData(int id, float anmationDuration, ItemCreateAreaPosType createAreaPosType, float radius, float damage, int attackCount)
    {
        Id = id;
        AnmationDuration = anmationDuration;
        CreateAreaPosType = createAreaPosType;
        Radius = radius;
        Damage = damage;
        AttackCount = attackCount;
    }
}
