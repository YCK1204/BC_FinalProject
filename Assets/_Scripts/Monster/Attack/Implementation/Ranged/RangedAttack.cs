using Game.Monster;
using UnityEngine;

/// <summary>
/// 원거리 공격
/// 기본적으로 투사체를 발사하는 방식? 추후에 다른 방식도 고려해서 만들 듯
/// </summary>
public abstract class RangedAttack : BaseAttack
{
    protected RangedAttack(float damage, float attackRange, Transform tr, MonsterAttack monsterAttack) : base(damage, attackRange, tr, monsterAttack)
    {
    }
}
