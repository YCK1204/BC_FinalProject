using Game.Monster;
using System.Collections;
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
    private float _maxBTCheckTime = 0.5f;

    private BoneReaperBT _curBT;
    public BoneReaperBT BT { get { return _curBT; } }

    protected LayerMask _playerMask;

    private WaitForSeconds _waitForHitFlash;

    [Header("Externals")]
    [SerializeField] public Orb VerticalOrb;
    [SerializeField] public Orb SpikeOrb;
    [SerializeField] public Material HitFlashMat;

    private void Awake()
    {
        _dataHandler = GetComponent<MonsterDataHandler>();
        _dataHandler.Init();

        _curBT = new BoneReaperBT();
        _curBT.Init(this);

        _head = GetComponentInChildren<BoneReaperHead>();
        BoneReaperHand[] hands = GetComponentsInChildren<BoneReaperHand>();

        _waitForHitFlash = new WaitForSeconds(0.1f);

        foreach (BoneReaperHand hand in hands)
        {
            if(hand.transform.localScale.x < 0)
                _rightHand = hand;
            else
                _leftHand = hand;
        }

        if (_head != null && _leftHand != null && _rightHand != null)
        {
            _head.Init(this);
            _leftHand.Init(this);
            _rightHand.Init(this);
        }
    }

    private void OnEnable()
    {
        _patternCoolTime = 3;
        _curSlamCount = 0;
        _curBreathCount = 0;
        _curBTCheckTime = 0;
        IsAttacking = false;

        _playerMask = LayerMask.GetMask(Game.Monster.Layers.Player);
    }

    private void Update()
    {
        if(_dataHandler.CurHp > 0 && _curBTCheckTime >= _maxBTCheckTime)
        {
            _curBTCheckTime = 0;
            _curBT.Evaluate();
        }
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
            Debug.Log("Find!");
            _target = player.transform;
            return NodeStatus.Success;
        }

        Debug.Log("Not Found...");
        return NodeStatus.Failure;
    }

    public void TakeDamage(int damage)
    {
        if (_dataHandler.CurHp <= 0)
            return;

        _dataHandler.TakeDamage(damage);
        if (_dataHandler.CurHp <= 0)
        {
            // 사망 처리
            _head.Die();
            _leftHand.Die();
            _rightHand.Die();
        }

    }

    public NodeStatus LaserAttack()
    {
        if (IsAttacking) return NodeStatus.Running;

        Debug.Log("Laser");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curSlamCount = 0;

        int idx = Random.Range(0, 2);
        if (idx == 0)
            _leftHand.Laser();
        else
            _rightHand.Laser();

        return NodeStatus.Success;
    }

    public NodeStatus SlamAttack()
    {
        if (IsAttacking) return NodeStatus.Running;

        Debug.Log("Slam");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curSlamCount++;


        if (Vector3.Distance(_target.position, _leftHand.transform.position) <= Vector3.Distance(_target.position, _rightHand.transform.position))
            _leftHand.Slam();
        else
            _rightHand.Slam();

        return NodeStatus.Success;
    }

    public NodeStatus SummonOrbAttack()
    {
        if (IsAttacking) return NodeStatus.Running;

        Debug.Log("SO");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curBreathCount = 0;

        _head.SummonOrb();

        return NodeStatus.Success;
    }

    public NodeStatus BreathAttack()
    {
        if (IsAttacking) return NodeStatus.Running;

        Debug.Log("Breath");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curBreathCount++;

        _head.Breath();

        return NodeStatus.Success;
    }

    public void HitFlash(SpriteRenderer sr, Coroutine hitEffect)
    {
        if (HitFlashMat == null)
            return;

        Material originMat = sr.material;
        sr.material = HitFlashMat;
        HitFlashMat.SetFloat("_FlashAmount", 1f);

        if(hitEffect != null)
            StopCoroutine(hitEffect);
        hitEffect = StartCoroutine(OffHitFlash(sr, originMat));
    }

    public IEnumerator OffHitFlash(SpriteRenderer sr, Material originMat)
    {
        yield return _waitForHitFlash;
        HitFlashMat.SetFloat("_FlashAmount", 0f);

        sr.material = originMat;
    }
}
