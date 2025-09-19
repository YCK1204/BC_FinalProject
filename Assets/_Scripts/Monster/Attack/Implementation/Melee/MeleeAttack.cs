using UnityEngine;

/// <summary>
/// 근접 공격
/// </summary>
public abstract class MeleeAttack : BaseAttack
{
    protected MeleeAttack(Transform tr, MonsterAttack monsterAttack) : base(tr, monsterAttack)
    {
    }
}
