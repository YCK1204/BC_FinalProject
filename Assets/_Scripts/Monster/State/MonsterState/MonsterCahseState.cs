using Game.Monster;
using UnityEngine;

public class MonsterCahseState : MonsterBaseState
{
    IMovable _chaseMove;
    private Transform _target;

    public MonsterCahseState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Chase;
    }

    public override void Enter()
    {
        base.Enter();

        _target = _stateMachine.Owner.Target;

        _chaseMove = (_stateMachine.Owner as PatrolMonster)?.GetChaseMovement();
        (_chaseMove as ChaseMove)?.SetTarget(_target);

        _stateMachine.Owner.LookTarget();
        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, _stateMachine.Owner.Speed);

    }

    public override void Update()
    {
        base.Update();

        if (_target != null)
            _stateMachine.Owner.LookTarget();

        if (!CheckDetectRange())
            _stateMachine.ChangeState(Common.StateType.Idle);

        //if(CheckAttackRange())
        if (_stateMachine.Owner.Attack.Attackable.GetCheckAttackable())
            _stateMachine.ChangeState(Common.StateType.Attack);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        _chaseMove?.Move();
    }

    public override void Exit()
    {
        base.Exit();

        _chaseMove?.StopMove();
        _chaseMove = null;
    }
}
