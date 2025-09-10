using Game.Monster;
using UnityEngine;

public class MonsterPatrolState : MonsterBaseState
{
    IMovable movable;

    public MonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Patrol;
    }

    public override void Enter()
    {
        base.Enter();

        movable = _stateMachine.Owner as IMovable;

        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, _stateMachine.Owner.Speed);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        movable?.Move();
    }

    public override void Exit()
    {
        base.Exit();

        movable?.StopMove();
        movable = null;
    }
}
