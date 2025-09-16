using UnityEngine;

public class VanishingAttack : MeleeAttack
{
    public VanishingAttack(float damage, float attackRange, Transform tr, MonsterAttack monsterAttack) : base(damage, attackRange, tr, monsterAttack)
    {
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override bool GetCheckAttackable(float margin = 0)
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackMove()
    {
        base.AttackMove();

        // Todo: 순간이동
    }
}
