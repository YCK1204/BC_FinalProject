using Game.Monster;
using System;
using UnityEngine;

/// <summary>
/// 근접 공격
/// 추후에 해당 클래스를 근접 상위 클래스로 만들고 더 구체적인 구현을 하위에서 하도록 수정할듯
/// </summary>
public abstract class MeleeAttack : BaseAttack
{
    protected MeleeAttack(float damage, float attackRange, Transform tr, MonsterAttack monsterAttack) : base(damage, attackRange, tr, monsterAttack)
    {
    }

    protected virtual void AttackMove()
    {
    }
}
