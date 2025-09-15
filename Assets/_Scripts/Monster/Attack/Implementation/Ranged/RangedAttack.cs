using Game.Monster;
using UnityEngine;

/// <summary>
/// 원거리 공격
/// 기본적으로 투사체를 발사하는 방식? 추후에 다른 방식도 고려해서 만들 듯
/// </summary>
public class RangedAttack : IAttackable
{
    float _damage;
    float _attackRange;
    Transform _tr;
    LayerMask _mask;

    public RangedAttack(float damage, float attackRange, Transform tr, LayerMask mask)
    {
        _damage = damage;
        _attackRange = attackRange;
        _tr = tr;
        _mask = mask;
    }

    public void Attack()
    {
        // Todo: 뭔가 투사체 발사
    }
}
