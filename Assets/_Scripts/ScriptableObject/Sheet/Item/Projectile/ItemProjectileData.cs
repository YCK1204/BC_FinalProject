using Game.Player;
using System;
using UnityEngine;

[Serializable]
public struct ItemProjectileData : ItemAbilityEvent
{
    public int Id;
    public float Duration;
    public float Speed;
    public float Width;
    public float Height;
    public int CollisionCount;
    public float Damage;

    public ItemProjectileData(int id, float duration, float speed, float width, float height, int collisionCount, float damage)
    {
        Id = id;
        Duration = duration;
        Speed = speed;
        Width = width;
        Height = height;
        CollisionCount = collisionCount;
        Damage = damage;
    }
    public void OnEvent(PlayerCharacter player)
    {
        var projectile = Manager.Pool.Pop<ProjectileController>();
        projectile.Init(this, player);
    }
}
