using UnityEngine;

public class MonsterIdleState : MonsterBaseState
{
    private float _moveDelay = 1.0f;
    private float _curMoveDelay = 1.0f;

    private float _detectDelay = 0.2f;
    private float _curDetectDelay = 0.0f;

    private LayerMask _playerLayer = LayerMask.GetMask(Common.Layers.Player);

    Collider2D _player;

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
        _curMoveDelay = 1.0f;

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
                // 일단 대상으로 지정
                _stateMachine.Owner.SetTarget(_player.transform);

                // 감지가 되고 공격 범위 내라면 공격
                if (CheckAttackRange())
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

        // 순찰 불가능한 적이라면 종료
        if (!_stateMachine.Owner.CanMove)
            return;

        // 대기 시간이 지나면 순찰상태로 변경
        if(_curMoveDelay >= _moveDelay)
        {
            _stateMachine.ChangeState(Common.StateType.Patrol);
        }
        _curMoveDelay += Time.deltaTime;
    }

    private bool CheckDetectRange()
    {
        _player = Physics2D.OverlapCircle(_stateMachine.Owner.transform.position,
                                                        _stateMachine.Owner.DetectRange, _playerLayer);

        // 플레이어가 감지되는데 눈에 보이는 지도 확인
        if (_player != null)
        {
            Vector2 dir = (_player.transform.position - _stateMachine.Owner.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(_stateMachine.Owner.transform.position, dir, _stateMachine.Owner.DetectRange);

            // 발견한 대상 사이에 장애물이 없다면 true
            if (hit.transform == _player.transform)
                return true;
        }

        return false;
    }

    private bool CheckAttackRange()
    {
        // 공격 가능 확인을 위해 null로 초기화
        _player = null;

        // 공격 가능 여부 확인
        _player = Physics2D.OverlapCircle(_stateMachine.Owner.transform.position,
                                                    _stateMachine.Owner.AttackRange, _playerLayer);
        // 공격 가능한 상태라면
        if (_player != null)
            return true;

        return false;
    }
}
