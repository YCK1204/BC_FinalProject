using Game.Player;
using System;
using UnityEngine;

public enum ItemCreateAreaPosType
{
    Player,
    Collision
}

[Serializable]
public struct ItemAreaData : ItemAbilityEvent
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

    public void OnEvent(PlayerCharacter player)
    {
        var area = Manager.Pool.Pop<AreaController>();
        area.Init(this, player);
    }
}
