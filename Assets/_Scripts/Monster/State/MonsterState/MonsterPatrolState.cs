using Game.Monster;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MonsterPatrolState : MonsterBaseState
{
    IMovable _patrolMove;

    float _maxPatrolTime;
    float _curPatrolTime;

    private float _detectDelay = 0.2f;
    private float _curDetectDelay = 0.0f;

    private LayerMask _playerLayer;
    private LayerMask _obstacleLayer;

    Collider2D _player;

    public MonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Patrol;
    }

    public override void Enter()
    {
        base.Enter();

        // 이동 관련 초기화
        _patrolMove = new PatrolMove(_stateMachine.Owner.Speed,
                                _stateMachine.Owner.transform,
                                _stateMachine.Owner.Rb,
                                _stateMachine.Owner.Col);

        (_stateMachine.Owner as PatrolMonster)?.SetMovement(_patrolMove);

        int dir = Random.Range(0, 2) % 2 == 0 ? 1 : -1;
        _stateMachine.Owner.transform.localScale = new Vector3(dir * _stateMachine.Owner.transform.localScale.x,
                                                               _stateMachine.Owner.transform.localScale.y,
                                                               _stateMachine.Owner.transform.localScale.z);
        
        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, _stateMachine.Owner.Speed);

        _maxPatrolTime = Random.Range(2f, 5f);
        _curPatrolTime = 0f;

        // 탐지 관련 초기화
        _detectDelay = 0.2f;
        _curDetectDelay = 0.0f;

        _playerLayer = LayerMask.GetMask(Common.Layers.Player);
        _obstacleLayer = ~LayerMask.GetMask(Common.Layers.Monster);
    }

    public override void Update()
    {
        base.Update();

        // 일정 시간마다 대상이 탐지 범위에 있는지 확인
        if (_curDetectDelay >= _detectDelay)
        {
            if (CheckDetectRange())
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

        // 일정 시간동안 순찰을 하고 대기 상태로 전환
        if (_curPatrolTime >= _maxPatrolTime)
            _stateMachine.ChangeState(Common.StateType.Idle);
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

    private bool CheckDetectRange()
    {
        _player = Physics2D.OverlapCircle(_stateMachine.Owner.transform.position,
                                                        _stateMachine.Owner.DetectRange, _playerLayer);

        // 플레이어가 감지 되었을 때, 시야각 내에 있지 않으면 false
        if (_player != null)
        {
            if (!CheckFov(_stateMachine.Owner.transform, _player.transform, 90))
                return false;
        }

        // 플레이어가 감지되고 거리 내라면, 눈에 보이는 지 확인
        if (_player != null && Vector3.Distance(_stateMachine.Owner.transform.position, _player.transform.position) <= _stateMachine.Owner.DetectRange)
        {
            Vector2 dir = (_player.transform.position - _stateMachine.Owner.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(_stateMachine.Owner.transform.position, dir, _stateMachine.Owner.DetectRange, _obstacleLayer);
            Debug.DrawRay(_stateMachine.Owner.transform.position, dir * _stateMachine.Owner.DetectRange, Color.blue);

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

        // 플레이어가 감지 되었을 때, 시야각 내에 있지 않으면 false
        if (_player != null)
        {
            if (!CheckFov(_stateMachine.Owner.transform, _player.transform, 90))
                return false;
        }

        // 공격 가능한 상태고 공격 범위 내라면
        if (_player != null && Vector3.Distance(_stateMachine.Owner.transform.position, _player.transform.position) <= _stateMachine.Owner.AttackRange)
            return true;

        return false;
    }

    private bool CheckFov(Transform observer, Transform target, float fov)
    {
        float dot = Vector2.Dot(observer.right * observer.localScale.x,
                        (target.position - observer.position).normalized);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle < fov * 0.5f ? true : false;
    }
}
