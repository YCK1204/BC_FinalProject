using UnityEngine;

/// <summary>
/// 근접 공격
/// </summary>
public abstract class MeleeAttack : BaseAttack
{
    protected MeleeAttack(float damage, float attackRange, Transform tr, MonsterAttack monsterAttack) : base(damage, attackRange, tr, monsterAttack)
    {
    }
}
