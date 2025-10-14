using Game.Monster;
using UnityEngine;

public class MonsterPatrolState : MonsterBaseState
{
    IMovable _patrolMove;

    float _maxPatrolTime;
    float _curPatrolTime;

    private float _detectDelay = 0.2f;
    private float _curDetectDelay = 0.0f;

    public MonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Game.Monster.StateType.Patrol;
    }

    public override void Enter()
    {
        base.Enter();

        // 이동 관련 초기화
        _patrolMove = _stateMachine.Owner?.GetPatrolMovement();

        Vector3 scale = _stateMachine.Owner.transform.localScale;
        int dir = Random.Range(0, 2) % 2 == 0 ? 1 : -1;
        _stateMachine.Owner.transform.localScale = new Vector3(dir * scale.x, scale.y, scale.z);
        
        _stateMachine.Owner.Anim.SetFloat(Game.Monster.AnimatorParams.Speed, _stateMachine.Owner.MonsterData.Speed);

        _maxPatrolTime = Random.Range(2f, 5f);
        _curPatrolTime = 0f;

        // 탐지 관련 초기화
        _detectDelay = 0.2f;
        _curDetectDelay = 0.0f;
    }

    public override void Update()
    {
        base.Update();

        // 일정 시간마다 대상이 탐지 범위에 있는지 확인
        if (_curDetectDelay >= _detectDelay)
        {
            if (CheckDetectRange())
            {
                // 감지가 되고 공격 범위 내라면 공격
                if (_stateMachine.Owner.Attack.Attackable.GetCheckAttackable(_xMargin))
                {
                    _stateMachine.ChangeState(Game.Monster.StateType.Attack);
                }
                // 감지는 됬는데 공격 사정거리 밖이라면 추격
                else
                {
                    _stateMachine.ChangeState(Game.Monster.StateType.Chase);
                }
            }
        }
        _curDetectDelay += Time.deltaTime;

        // 일정 시간동안 순찰을 하고 대기 상태로 전환
        if (_curPatrolTime >= _maxPatrolTime)
            _stateMachine.ChangeState(Game.Monster.StateType.Idle);
        _curPatrolTime += Time.deltaTime;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        _patrolMove?.Move();
    }

    public override void Exit()
    {
        base.Exit();

        _patrolMove?.StopMove();

        _patrolMove = null;
    }
}
