using Game.Monster;
using UnityEngine;

public class MonsterCahseState : MonsterBaseState
{
    IMovable _chaseMove;
    private Transform _target;

    public MonsterCahseState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Game.Monster.StateType.Chase;
    }

    public override void Enter()
    {
        base.Enter();

        _stateMachine.Owner.Ondetect?.Invoke();

        _target = _stateMachine.Owner.Target;
        _stateMachine.Owner.LookTarget();

        _chaseMove = _stateMachine.Owner?.GetChaseMovement();

        // 아 이거 마음에 안드는데
        if(_chaseMove is ChaseMove)
        {
            (_chaseMove as ChaseMove)?.SetTarget(_target);
            _stateMachine.Owner.Anim.SetFloat(Game.Monster.AnimatorParams.Speed, _stateMachine.Owner.MonsterData.Speed);
        }
        else if( _chaseMove is VanishMove)
        {
            (_chaseMove as VanishMove)?.SetTarget(_target);
            _stateMachine.Owner.Anim.SetFloat(Game.Monster.AnimatorParams.Speed, 0);
        }


    }

    public override void Update()
    {
        base.Update();

        if (_target != null)
            _stateMachine.Owner.LookTarget();

        if (!CheckDetectRange(false))
            _stateMachine.ChangeState(Game.Monster.StateType.Idle);

        if (_stateMachine.Owner.Attack.Attackable.GetCheckAttackable(_xMargin))
            _stateMachine.ChangeState(Game.Monster.StateType.Attack);
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
