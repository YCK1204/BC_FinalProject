using UnityEngine;

public abstract class MonsterBaseState : Game.Monster.IState
{
    protected MonsterStateMachine _stateMachine;

    public Game.Monster.StateType StateType { get; protected set; }

    private LayerMask _playerLayer;
    private LayerMask _obstacleLayer;
    private LayerMask _monsterLayer;

    protected float _curCheckTime;
    protected float _maxCheckTime;

    // 이부분 몬스터 종류에 따라 설정해야 할 듯? ex) 근접(90), 원거리(360)
    protected float _detectFov = 90f;

    // 공격 범위 탐색에서 보정하는 값
    protected float _xMargin = 0.3f;

    public MonsterBaseState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

        _playerLayer = LayerMask.GetMask(Game.Monster.Layers.Player);
        _obstacleLayer = (LayerMask.GetMask(Game.Monster.Layers.Ground) | LayerMask.GetMask(Game.Monster.Layers.Player));
        _monsterLayer = LayerMask.GetMask(Game.Monster.Layers.Monster);

        _curCheckTime = 0;
        _maxCheckTime = 0.2f;
        //CheckColliders();
    }

    public virtual void Enter() { /*Debug.Log(StateType);*/ }
    public virtual void Exit() { }
    public virtual void Update()
    {
        if(_curCheckTime >=  _maxCheckTime)
        {
            _curCheckTime = 0;
            CheckColliders();
        }
        _curCheckTime += Time.deltaTime;
    }
    public virtual void FixedUpdate() { }


    // 탐지 범위내에 대상이 있는 지 확인하는 메서드
    // 추가적으로 fov체크 여부를 인자로 전달받음
    protected bool CheckDetectRange(bool isCheckFov = true)
    {
        Collider2D player = Physics2D.OverlapCircle(_stateMachine.Owner.transform.position,
                                                        _stateMachine.Owner.MonsterData.DetectRange, _playerLayer);

        // 플레이어가 감지 되었을 때, 시야각 내에 있지 않으면 false
        if (player != null && isCheckFov)
        {
            if (!CheckFov(_stateMachine.Owner.transform, player.transform, _detectFov))
                return false;
        }
 
        // 플레이어가 감지되고 거리 내라면, 눈에 보이는 지 확인
        if (player != null && Vector3.Distance(_stateMachine.Owner.transform.position, player.transform.position) <= _stateMachine.Owner.MonsterData.DetectRange)
        {
            Vector2 dir = (player.transform.position - _stateMachine.Owner.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(_stateMachine.Owner.transform.position, dir, _stateMachine.Owner.MonsterData.DetectRange, _obstacleLayer);
            Debug.DrawRay(_stateMachine.Owner.transform.position, dir * _stateMachine.Owner.MonsterData.DetectRange, Color.blue);

            // 발견한 대상 사이에 장애물이 없다면 true
            if (hit.transform == player.transform)
            {
                _stateMachine.Owner.SetTarget(player.transform);
                return true;
            }
        }

        return false;
    }

    // 공격 범위내에 대상이 있는 지 확인하는 메서드
    protected void CheckColliders()
    {
        // 공격 가능 여부 확인
        Collider2D[] ignoreCollider = Physics2D.OverlapCircleAll(_stateMachine.Owner.transform.position,
                                                    _stateMachine.Owner.MonsterData.DetectRange, _playerLayer);

        foreach (Collider2D collider in ignoreCollider)
        {
            _stateMachine.Owner.RegisterIgnoreCollider(collider);
        }
    }

    /// <summary>
    /// 특정 관측자 입장에서 대상이 fov내에 들어오는지 확인하는 메서드
    /// </summary>
    /// <param name="observer">관측자</param>
    /// <param name="target">관측 대상</param>
    /// <param name="fov">시야각</param>
    /// <returns></returns>
    protected bool CheckFov(Transform observer, Transform target, float fov)
    {
        float dot = Vector2.Dot(observer.right * (observer.localScale.x < 0 ? -1 : 1),
                        (target.position - observer.position).normalized);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        return angle < fov * 0.5f ? true : false;
    }
}
