using Game.Monster;
using System;
using UnityEngine;

/// <summary>
/// 근접 공격
/// 추후에 해당 클래스를 근접 상위 클래스로 만들고 더 구체적인 구현을 하위에서 하도록 수정할듯
/// </summary>
public class MeleeAttack : Game.Monster.IAttackable
{
    float _damage;
    float _attackRange;
    Transform _tr;
    LayerMask _mask;

    public MeleeAttack(float damage, float attackRange, Transform tr)
    {
        _damage = damage;
        _attackRange = attackRange;
        _tr = tr;

        _mask = LayerMask.GetMask(Common.Layers.Player);
    }

    public void Attack()
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        Collider2D target = Physics2D.OverlapBox(_tr.position + new Vector3(_attackRange * 0.5f * dir, 0, 0), new Vector2(_attackRange, _attackRange), 0, _mask);

        if(target != null)
        {
            target.GetComponent<IDamageable>()?.TakeDamage((int)_damage);
        }
    }
}
