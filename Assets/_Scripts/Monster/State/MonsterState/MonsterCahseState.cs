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

        _chaseMove = new ChaseMove(_stateMachine.Owner.Speed,
                                    _stateMachine.Owner.transform,
                                    _stateMachine.Owner.Rb,
                                    _stateMachine.Owner.Col,
                                    _target);

        (_stateMachine.Owner as PatrolMonster)?.SetMovement(_chaseMove);

        _stateMachine.Owner.LookTarget();
        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, _stateMachine.Owner.Speed);

    }

    public override void Update()
    {
        base.Update();

        if (Vector3.Distance(_stateMachine.Owner.transform.position, _target.position) >= _stateMachine.Owner.DetectRange)
            _stateMachine.ChangeState(Common.StateType.Idle);

        if(Vector3.Distance(_stateMachine.Owner.transform.position, _target.position) <= _stateMachine.Owner.AttackRange)
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
