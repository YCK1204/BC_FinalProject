using Game.Monster;
using System;
using UnityEngine;

/// <summary>
/// 근접 공격
/// 추후에 해당 클래스를 근접 상위 클래스로 만들고 더 구체적인 구현을 하위에서 하도록 수정할듯
/// </summary>
public abstract class MeleeAttack : Game.Monster.IAttackable
{
    protected float _damage;
    protected float _attackRange;
    protected Transform _tr;
    protected LayerMask _mask;

    public MeleeAttack(float damage, float attackRange, Transform tr)
    {
        _damage = damage;
        _attackRange = attackRange;
        _tr = tr;

        _mask = LayerMask.GetMask(Common.Layers.Player);
    }

    public virtual void Attack()
    {
        
    }

    // 만약 공격 방식을 반원으로 탐색할 때 필요한 함수
    protected bool CheckFov(Transform observer, Transform target, float fov)
    {
        float dot = Vector2.Dot(observer.right * observer.localScale.x,
                        (target.position - observer.position).normalized);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle < fov * 0.5f ? true : false;
    }
}
