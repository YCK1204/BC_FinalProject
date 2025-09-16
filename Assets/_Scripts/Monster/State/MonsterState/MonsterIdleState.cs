using UnityEngine;

public class MonsterIdleState : MonsterBaseState
{
    private float _moveDelay = 1.0f;
    private float _curMoveDelay = 1.0f;

    private float _detectDelay = 0.2f;
    private float _curDetectDelay = 0.0f;

    public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Idle;
    }

    /*
     *  순찰 상태 X -> 계속 대기
     *  순찰 상태 o -> 일정 시간 이후 순찰 상태로 변경
     */

    public override void Enter()
    {
        base.Enter();

        _stateMachine.Owner.ResetTarget();
        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, 0);

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
                // 감지가 되고 공격 범위 내라면 공격
                // 이거 차라리 원뿔 모양 감지보다는 각 공격 방식에 맞는 방식으로 변경하기? -> 그래서 일단 바꿔 봄
                //if (CheckAttackRange())
                if (_stateMachine.Owner.Attack.Attackable.GetCheckAttackable(_xMargin))
                {
                    _stateMachine.ChangeState(Common.StateType.Attack);
                }
                // 감지는 됬는데 공격 사정거리 밖이라면 추격
                else
                {
                    _stateMachine.ChangeState(Common.StateType.Chase);
                }
            }
        }
        _curDetectDelay += Time.deltaTime;

        // 순찰 상태 전환이 불가능한 적이라면 종료
        if (!_stateMachine.Owner.CanMove)
            return;

        // 대기 시간이 지나면 순찰상태로 변경
        if(_curMoveDelay >= _moveDelay)
        {
            _stateMachine.ChangeState(Common.StateType.Patrol);
        }
        _curMoveDelay += Time.deltaTime;
    }
}
