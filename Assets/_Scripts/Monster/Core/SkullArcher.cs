using Game.Monster;
using UnityEngine;

public class SkullArcher : PatrolStateMonster
{
    IAttackable _curAttack;

    protected override void Awake()
    {
        base.Awake();

        _attack.Init(this);
        _curAttack = new RangedAttack(transform, _attack);

        _attack.Attackable = _curAttack;
    }
}
