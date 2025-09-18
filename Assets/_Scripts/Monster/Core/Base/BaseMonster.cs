using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 최상위 클래스
/// </summary>
public abstract class BaseMonster : MonoBehaviour, Game.Monster.IDamageable
{
    /// <summary>
    /// 만약 단일책임원칙에 따라 스크립트를 분리하면 어떻게 하지?
    /// 몬스터의 데이터 분리 -> 했음
    /// 공격쪽은 이미 분리함
    /// </summary>

    // 몬스터 데이터 관리 컴포넌트
    [SerializeField] protected MonsterDataHandler _dataHandler;
    public MonsterDataHandler MonsterData {  get { return _dataHandler; } }

    // 몬스터 공격 컴포넌트
    protected MonsterAttack _attack;
    public MonsterAttack Attack { get { return _attack; } }

    // 몬스터 상태 머신
    protected MonsterStateMachine _stateMachine;
    public MonsterStateMachine StateMachine { get { return _stateMachine; } }

    private Transform _target;
    public Transform Target { get { return _target; } }

    protected Rigidbody2D _rb;
    public Rigidbody2D Rb { get { return _rb; } }
    protected Collider2D _col;
    public Collider2D Col { get { return _col; } }
    protected Animator _anim;
    public Animator Anim {  get { return _anim; } }
    protected SpriteRenderer _sr;
    public SpriteRenderer Sr { get { return _sr; } }

    protected List<Game.Monster.ISpecialAbillity> _abillityList;

    protected List<Collider2D> _ignoredColliderList;

    public Action OnDied;
    public Action OnUpdate;
    public Action OnHit;

    // 슈퍼아머 상태 확인 변수
    public bool IsSuperArmor = false;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _anim = GetComponentInChildren<Animator>();
        _sr = GetComponentInChildren<SpriteRenderer>();

        _attack = GetComponentInChildren<MonsterAttack>();
        _dataHandler = Extension.GetOrAddComponent<MonsterDataHandler>(this.gameObject);
        _dataHandler.Owner = this;
        _dataHandler.SetStatModifier(new StatModifier(1,1,1,1));
        _dataHandler.Init();

        Init();
    }

    protected virtual void Init()
    {
        _abillityList = new List<Game.Monster.ISpecialAbillity>();
        Game.Monster.ISpecialAbillity[] abillities = GetComponents<Game.Monster.ISpecialAbillity>();
        foreach (Game.Monster.ISpecialAbillity abillity in abillities)
        {
            _abillityList.Add(abillity);
            abillity.Init(this);
        }

        if(_ignoredColliderList != null)
            _ignoredColliderList.Clear();
        else
            _ignoredColliderList = new List<Collider2D>();

        _stateMachine = new MonsterStateMachine(this);

    }

    protected virtual void OnEnable()
    {
        _stateMachine.Init();
        _stateMachine.Owner.Rb.bodyType = RigidbodyType2D.Dynamic;
        _stateMachine.Owner.Col.isTrigger = false;
    }

    protected virtual void Update()
    {
        OnUpdate?.Invoke();
        _stateMachine?.Update();
    }

    protected virtual void FixedUpdate()
    {
        _stateMachine?.FixedUpdate();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void ResetTarget()
    {
        _target = null;
    }

    // 타겟을 바라보는 메서드
    public void LookTarget()
    {
        float d = _target.position.x < transform.position.x ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(d, transform.localScale.y, transform.localScale.z);
    }

    // 데미지 적용 메서드
    public virtual void TakeDamage(int damage)
    {
        _dataHandler.TakeDamage(damage);
        OnHit?.Invoke();

        if (_dataHandler.CurHp <= 0)
        {
            _stateMachine.ChangeState(Game.Monster.StateType.Die);
        }
        else if(!IsSuperArmor)
        {
            _stateMachine.ChangeState(Game.Monster.StateType.Hit);
        }
    }

    // 몬스터가 사망할 때 호출하는 메서드
    public void Die()
    {
        OnDied?.Invoke();
        // Todo: 오브젝트 풀로 리턴
        Destroy(gameObject);
    }

    /// <summary>
    /// 해당 콜라이더를 충돌 무시로 지정하고 리스트에 넣음
    /// </summary>
    /// <param name="collider">충돌을 무시할 콜라이더</param>
    public void RegisterIgnoreCollider(Collider2D collider)
    {
        if(!_ignoredColliderList.Contains(collider))
        {
            Physics2D.IgnoreCollision(Col, collider, true);
            _ignoredColliderList.Add(collider);
        }
    }

    /// <summary>
    /// 해당 콜라이더를 충돌 가능하게 만들고 리스트에서 제거함
    /// </summary>
    /// <param name="collider">충돌 가능하게 만들 콜라이더</param>
    public void DeleteIgnoreCollider(Collider2D collider)
    {
        if(_ignoredColliderList.Contains(collider))
        {
            Physics2D.IgnoreCollision(Col, collider, false);
            _ignoredColliderList.Remove(collider);
        }
    }

#if UNITY_EDITOR
    // 에디터에서 탐지 범위와 공격 가능 범위를 표시하는 메서드
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _dataHandler.DetectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _dataHandler.AttackRange);
    }
#endif

}
