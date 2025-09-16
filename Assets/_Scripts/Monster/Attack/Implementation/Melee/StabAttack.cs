using Game.Monster;
using UnityEngine;

public class StabAttack : MeleeAttack
{
    public StabAttack(float damage, float attackRange, Transform tr) : base(damage, attackRange, tr)
    {
    }

    public override void Attack()
    {
        if (GetCheckAttackable())
        {
            _target.GetComponent<IDamageable>()?.TakeDamage((int)_damage);
        }
    }

    public override bool GetCheckAttackable()
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        _target = Physics2D.OverlapBox(_tr.position + new Vector3(_attackRange * 0.5f * dir, 0, 0), new Vector2(_attackRange, 0.5f), 0, _mask);

        return false;
    }
}
