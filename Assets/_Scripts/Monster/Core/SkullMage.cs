using Game.Monster;
using UnityEngine;

public class SkullMage : PatrolStateMonster
{
    IAttackable _curAttack;

    protected override void Awake()
    {
        base.Awake();

        _attack.Init(this);
        _curAttack = new HomingRangedAttack(transform, _attack);

        _attack.Attackable = _curAttack;
    }
}
