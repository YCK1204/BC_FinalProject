using UnityEngine;

public abstract class BaseAttack : Game.Monster.IAttackable
{
    protected float _damage { get { return _monsterAttack.AttackPower; } }
    protected float _attackRange { get { return _monsterAttack.AttackRange; } }
    protected Transform _tr;
    protected LayerMask _mask;
    protected Collider2D _target;

    protected MonsterAttack _monsterAttack;

    public BaseAttack(Transform tr, MonsterAttack monsterAttack)
    {
        _monsterAttack = monsterAttack;
        _tr = tr;

        _mask = LayerMask.GetMask(Game.Monster.Layers.Player);
    }

    public virtual void Attack()
    {
        Manager.Audio.Play(_monsterAttack.Owner.MonsterData.Data.AttackSfx, _tr);
    }

    public virtual void StopAttack()
    {

    }

    /// <summary>
    /// 타겟이 공격 범위에 있는지 확인하는 메서드
    /// </summary>
    /// <returns></returns>
    public abstract bool GetCheckAttackable(float margin = 0);

    public virtual void Init() { }

    // 타겟이 특정 시야각 안에 있는지 확인하는 메서드
    protected bool CheckFov(Transform observer, Transform target, float fov)
    {
        float dot = Vector2.Dot(observer.right * observer.localScale.x,
                        (target.position - observer.position).normalized);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle < fov * 0.5f ? true : false;
    }
}
