using Game.Monster;
using UnityEngine;

public class BoneReaper : BossMonster
{
    // 각 파츠들에 대한 정보
    private BoneReaperHead _head;
    public BoneReaperHead Head { get { return _head; } }
    private BoneReaperHand _leftHand;
    public BoneReaperHand LeftHand { get { return _leftHand; } }
    private BoneReaperHand _rightHand;
    public BoneReaperHand RightHand { get { return _rightHand; } }

    protected float _patternCoolTime = 0f;
    public float PatternCoolTime { get { return _patternCoolTime; } }
    public readonly float PatternMaxCoolTime = 5f;

    protected int _curSlamCount;
    public int CurSlamCount { get { return _curSlamCount; } }
    protected int _curBreathCount;
    public int CurBreathCount { get { return _curBreathCount; } }

    private float _curBTCheckTime;
    private float _maxBTCheckTime = 0.2f;

    private BoneReaperBT _curBT;
    public BoneReaperBT BT { get { return _curBT; } }

    protected LayerMask _playerMask;

    public bool IsAttacking;

    private void Awake()
    {
        _dataHandler = GetComponent<MonsterDataHandler>();

        _curBT = new BoneReaperBT();
        _curBT.Init(this);

        if (_head != null && _leftHand != null && _rightHand != null)
        {
            _head.Init(this);
            _leftHand.Init(this);
            _rightHand.Init(this);
        }
    }

    private void OnEnable()
    {
        _patternCoolTime = 0;
        _curSlamCount = 0;
        _curBreathCount = 0;
        _curBTCheckTime = 0;
        IsAttacking = false;

        _playerMask = LayerMask.GetMask(Game.Monster.Layers.Player);
    }

    private void Update()
    {
        if(_dataHandler.CurHp > 0 && _curBTCheckTime >= _maxBTCheckTime)
            _curBT.Evaluate();
        _curBTCheckTime += Time.deltaTime;

        if(!IsAttacking)
            _patternCoolTime += Time.deltaTime;
    }

    public NodeStatus FindTarget()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position,
                                                MonsterData.DetectRange, _playerMask);
        if(player != null)
        {
            _target = player.transform;
            return NodeStatus.Success;
        }

        return NodeStatus.Failure;
    }

    public void TakeDamage(int damage)
    {
        if (_dataHandler.CurHp <= 0)
            return;

        _dataHandler.TakeDamage(damage);
        if (_dataHandler.CurHp <= 0)
        {
            
        }

    }
}
