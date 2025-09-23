using UnityEngine;

public class HomingRangedAttack : RangedAttack
{
    public HomingRangedAttack(Transform tr, MonsterAttack monsterAttack) : base(tr, monsterAttack)
    {
    }

    public override bool GetCheckAttackable(float margin = 0)
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        _target = Physics2D.OverlapCircle(_tr.position, _attackRange, _mask);

        return _target != null;
    }
}
