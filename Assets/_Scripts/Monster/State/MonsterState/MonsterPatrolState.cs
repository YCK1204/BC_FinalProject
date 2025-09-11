using Game.Monster;
using UnityEngine;

public class MonsterPatrolState : MonsterBaseState
{
    IMovable movable;

    float _maxPatrolTime;
    float _curPatrolTime;

    public MonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Patrol;
    }

    public override void Enter()
    {
        base.Enter();

        movable = new PatrolMove(_stateMachine.Owner.Speed,
                                _stateMachine.Owner.transform,
                                _stateMachine.Owner.Rb,
                                _stateMachine.Owner.Col);

        (_stateMachine.Owner as PatrolMonster)?.SetMovement(movable);

        int dir = Random.Range(0, 2) % 2 == 0 ? 1 : -1;
        _stateMachine.Owner.transform.localScale = new Vector3(dir * _stateMachine.Owner.transform.localScale.x,
                                                               _stateMachine.Owner.transform.localScale.y,
                                                               _stateMachine.Owner.transform.localScale.z);
        
        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, _stateMachine.Owner.Speed);

        _maxPatrolTime = Random.Range(2f, 5f);
        _curPatrolTime = 0f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_curPatrolTime >= _maxPatrolTime)
            _stateMachine.ChangeState(Common.StateType.Idle);

        movable?.Move();

        _curPatrolTime += Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();

        movable?.StopMove();

        movable = null;
    }
}
