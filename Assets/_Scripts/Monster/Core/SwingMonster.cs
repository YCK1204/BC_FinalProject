using Game.Monster;
using UnityEngine;

public class SwingMonster : StateMachineMonster
{
    protected override void Awake()
    {
        base.Awake();

        _attack.Init(this);
        _curAttack = new SwingAttack(transform, _attack);

        _attack.Attackable = _curAttack;
    }

    protected override void Init()
    {
        base.Init();

        _curPatrolMovement = new PatrolMove(this);
        _curChaseMovement = new ChaseMove(this);
    }
}
