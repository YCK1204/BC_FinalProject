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
        _stateMachine.Owner.LookTarget();

        _chaseMove = (_stateMachine.Owner as PatrolMonster)?.GetChaseMovement();

        // 아 이거 마음에 안드는데
        if(_chaseMove is ChaseMove)
        {
            (_chaseMove as ChaseMove)?.SetTarget(_target);
            _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, _stateMachine.Owner.Speed);
        }
        else if( _chaseMove is ShadowStepMove)
        {
            (_chaseMove as ShadowStepMove)?.SetTarget(_target);
            _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, 0);
        }


    }

    public override void Update()
    {
        base.Update();

        if (_target != null)
            _stateMachine.Owner.LookTarget();

        if (!CheckDetectRange())
            _stateMachine.ChangeState(Common.StateType.Idle);

        //if(CheckAttackRange())
        if (_stateMachine.Owner.Attack.Attackable.GetCheckAttackable(_xMargin))
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
