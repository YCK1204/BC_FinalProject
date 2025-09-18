using UnityEngine;

public class MonsterIdleState : MonsterBaseState
{
    private float _moveDelay = 1.0f;
    private float _curMoveDelay = 1.0f;

    private float _detectDelay = 0.2f;
    private float _curDetectDelay = 0.0f;

    public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Game.Monster.StateType.Idle;
    }

    /*
     *  순찰 상태 X -> 계속 대기
     *  순찰 상태 o -> 일정 시간 이후 순찰 상태로 변경
     */

    public override void Enter()
    {
        base.Enter();

        _stateMachine.Owner.ResetTarget();
        _stateMachine.Owner.Anim.SetFloat(Game.Monster.AnimatorParams.Speed, 0);

        _moveDelay = 1.0f;
        _curMoveDelay = 0.0f;

        _detectDelay = 0.2f;
        _curDetectDelay = 0.0f;
    }

    public override void Update()
    {
        base.Update();

        // 일정 시간마다 대상이 탐지 범위에 있는지 확인
        if(_curDetectDelay >= _detectDelay)
        {
            if(CheckDetectRange())
            {
                // 공격 범위 안이라면 공격 상태로 전환
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

        // 순찰 상태 전환이 불가능한 적이라면 종료
        if (!_stateMachine.Owner.MonsterData.CanMove)
            return;

        // 대기 시간이 지나면 순찰상태로 변경
        if(_curMoveDelay >= _moveDelay)
        {
            _stateMachine.ChangeState(Game.Monster.StateType.Patrol);
        }
        _curMoveDelay += Time.deltaTime;
    }
}
