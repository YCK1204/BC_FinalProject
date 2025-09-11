using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Monster : MonoBehaviour
{
    /// <summary>
    /// 만약 단일책임원칙에 따라 스크립트를 분리하면 어떻게 하지?
    /// 몬스터의 데이터 분리
    /// 
    /// </summary>

    // 데이터(추후 분리 예정)
    protected int _curHp = 25;
    public int CurHp { get { return _curHp; } }

    protected float _speed = 3f;
    public float Speed { get { return _speed; } }

    protected float _attackPower = 5f;
    public float AttackPower { get { return _attackPower; } }

    protected float _attackDelay = 1f;
    public float AttackDelay { get { return _attackDelay; } }

    protected float _attackRange = 3f;
    public float AttackRange { get { return _attackRange; } }

    protected float _detectRange = 5f;
    public float DetectRange { get { return _detectRange; } }

    protected bool _canMove = true;
    public bool CanMove { get { return _canMove; } }
    //

    protected MonsterStateMachine _stateMachine;
    public MonsterStateMachine StateMachine { get { return _stateMachine; } }

    public Transform _target;

    protected Rigidbody2D _rb;
    public Rigidbody2D Rb { get { return _rb; } }
    protected Collider2D _col;
    public Collider2D Col { get { return _col; } }
    protected Animator _anim;
    public Animator Anim {  get { return _anim; } }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _anim = GetComponentInChildren<Animator>();

        Init();
    }

    protected virtual void Init()
    {
        _stateMachine = new MonsterStateMachine(this);
    }

    protected virtual void OnEnable()
    {
        _stateMachine.Init();
    }

    protected virtual void Update()
    {
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

    public void LookTarget()
    {
        float d = _target.position.x < transform.position.x ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(d, transform.localScale.y, transform.localScale.z);
    }

    // 에디터에서 탐지 범위와 공격 가능 범위를 표시하는 메서드
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DetectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
