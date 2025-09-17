using UnityEngine;

public abstract class BaseAttack : Game.Monster.IAttackable
{
    protected float _damage;
    protected float _attackRange;
    protected Transform _tr;
    protected LayerMask _mask;
    protected Collider2D _target;

    protected MonsterAttack _monsterAttack;

    public BaseAttack(float damage, float attackRange, Transform tr, MonsterAttack monsterAttack)
    {
        _damage = damage;
        _attackRange = attackRange;
        _tr = tr;

        _mask = LayerMask.GetMask(Common.Layers.Player);
        _monsterAttack = monsterAttack;
    }

    public abstract void Attack();

    /// <summary>
    /// 타겟이 공격 범위에 있는지 확인하는 메서드
    /// </summary>
    /// <returns></returns>
    public abstract bool GetCheckAttackable(float margin = 0);

    // 타겟이 특정 시야각 안에 있는지 확인하는 메서드
    protected bool CheckFov(Transform observer, Transform target, float fov)
    {
        float dot = Vector2.Dot(observer.right * observer.localScale.x,
                        (target.position - observer.position).normalized);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle < fov * 0.5f ? true : false;
    }
}
