using Game.Player;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
public enum ItemEffectCreatePosType
{
    Player,
    Collision,
    NearestEnemy,
    WithInRangeEnemy,
}

[Serializable]
public struct ItemAreaData : ItemAbilityEvent
{
    public int Id;
    public float AnmationDuration;
    public ItemEffectCreatePosType CreatePosType;
    public float Radius;
    public float Damage;
    public int AttackCount;

    public ItemAreaData(int id, float anmationDuration, ItemEffectCreatePosType createPosType, float radius, float damage, int attackCount)
    {
        Id = id;
        AnmationDuration = anmationDuration;
        CreatePosType = createPosType;
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
