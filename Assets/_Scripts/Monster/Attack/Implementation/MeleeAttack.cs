using Game.Monster;
using System;
using UnityEngine;

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
        Debug.Log(_tr.position + new Vector3(_attackRange * 0.5f * dir, 0, 0));
        if(target != null)
        {
            target.GetComponent<IDamageable>()?.TakeDamage((int)_damage);
        }
    }
}
