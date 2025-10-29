using Game.Monster;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    protected float _patternMaxCoolTime = 3f;
    public float PatternMaxCoolTime { get { return _patternMaxCoolTime; } }

    protected int _curSlamCount;
    public int CurSlamCount { get { return _curSlamCount; } }
    protected int _curBreathCount;
    public int CurBreathCount { get { return _curBreathCount; } }

    private float _curBTCheckTime;
    private float _maxBTCheckTime = 0.5f;

    public int CurPhase;

    private BoneReaperBT _curBT;
    public BoneReaperBT BT { get { return _curBT; } }

    private RandomPlatformGenerator _platformGenerator;
    private BossIntroController _introController;
    private BossUI _bossUI;

    protected LayerMask _playerMask;

    public bool IsInvincible = false;

    private WaitForSeconds _waitForHitFlash;

    [SerializeField] private bool[] _isHpLower;

    [Header("Summon Orb")]
    public Dictionary<string, Pool> OrbPool;

    [Header("Slam")]
    public int _curRunningHandsCount = 0;

    [Header("Externals")]
    [SerializeField] public Transform PoolTr;
    [SerializeField] public Orb VerticalOrb;
    [SerializeField] public Orb SpikeOrb;
    [SerializeField] public Material HitFlashMat;

    private void Awake()
    {
        _dataHandler = GetComponent<MonsterDataHandler>();
        _dataHandler.Init();

        _curBT = new BoneReaperBT();
        _curBT.Init(this);

        _waitForHitFlash = new WaitForSeconds(0.1f);
        _bossScale = transform.localScale.x;

        OrbPool = new Dictionary<string, Pool>();

        IsSuperArmor = true;

        if (VerticalOrb != null)
        {
            Pool verticalOrbPool = new Pool();
            verticalOrbPool.Init(10, PoolTr, VerticalOrb.gameObject);
            OrbPool.Add("Vertical", verticalOrbPool);
        }
        if (SpikeOrb != null)
        {
            Pool spikeOrbPool = new Pool();
            spikeOrbPool.Init(2, PoolTr, SpikeOrb.gameObject);
            OrbPool.Add("Spike", spikeOrbPool);
        }

        _head = GetComponentInChildren<BoneReaperHead>();
        BoneReaperHand[] hands = GetComponentsInChildren<BoneReaperHand>();

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

        _head.OnDie += DestroyOnDeath;

        _platformGenerator = GetComponentInChildren<RandomPlatformGenerator>();
        _platformGenerator.Init(this);
        _bossUI = GetComponentInChildren<BossUI>();
        _bossUI.Init(this);
        _introController = GetComponentInChildren<BossIntroController>();
        _introController.Init(this);
    }

    private void OnEnable()
    {
        _patternCoolTime = 1;
        _curSlamCount = 0;
        _curBreathCount = 0;
        _curBTCheckTime = 0;
        CurPhase = 1;
        IsAttacking = false;
        IsDirecting = true;
        IsInvincible = true;

        _isHpLower = new bool[10];

        if(PlayerManager.Instance != null)
        {
            PlayerManager.Instance.Player.OnDied += _bossUI.DisableUI;
            PlayerManager.Instance.Player.OnDied += StopBoss;
            PlayerManager.Instance.Player.OnDied += DestroyBossOnPlayerDied;
        }

        _playerMask = LayerMask.GetMask(Game.Monster.Layers.Player);

        _platformGenerator.StartGenerate();
        _introController.StartIntro();
    }

    private void Update()
    {
        if (IsDirecting) return;

        if(_dataHandler.CurHp > 0 && _curBTCheckTime >= _maxBTCheckTime)
        {
            _curBTCheckTime = 0;
            _curBT.Evaluate();
        }
        _curBTCheckTime += Time.deltaTime;

        if(!IsAttacking)
            _patternCoolTime += Time.deltaTime;

        // 연출 중이 아닐때는 매 프레임마다 무적 해제
        if(!IsDirecting)
            IsInvincible = false;

#if UNITY_EDITOR
        DebugBossDamage();
#endif
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

    public void TakeDamage(float damage, bool hitSfxMute = false)
    {
        if (_dataHandler.CurHp <= 0)
            return;

        _dataHandler.TakeDamage(damage);
#if UNITY_EDITOR
        Debug.Log($"[Damage] {name}이 받은 피해: {damage}");
#endif
        OnHealthchanged?.Invoke();
        CheckFunnel();
        if (_dataHandler.CurHp <= 0)
        {
            Die();
            Manager.Data.playerSOData.ResetData();
        }
        else if(!hitSfxMute)
        {
            Manager.Audio.Play(AudioKey.Player.Hit.P_HIT_UNDEAD_BOSS, transform);
        }

    }

    public override void Die()
    {
        PlayerCharacter.Instance.Kill();
        _head.Die();
        _leftHand.Die();
        _rightHand.Die();
        _platformGenerator.StopGenerate();
        Manager.Audio.Play(AudioKey.Monster.Die.M_DIE_BOSS, transform);
        Manager.Analytics.SendFunnelStep(FunnelStep._Boss, 12);
        Manager.Analytics.SendFunnelStep(FunnelStep._StageC, 14);
        _introController.SetBossCam(true);
        DisplayManager.Instance.PlayClearEffect();
    }

    #region 페이즈 1
    public NodeStatus LaserAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("Laser");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curSlamCount = 0;

        int idx = UnityEngine.Random.Range(0, 2);
        if (idx == 0)
            _leftHand.Laser();
        else
            _rightHand.Laser();

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    /// <summary>
    /// 기획서대로 양손 시간차 공격
    /// </summary>
    /// <returns></returns>
    public NodeStatus TwoHandSlamAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("Slam");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curSlamCount++;
        _curRunningHandsCount += 2;

        if (Vector3.Distance(_target.position, _leftHand.transform.position) <= Vector3.Distance(_target.position, _rightHand.transform.position))
        {
            _leftHand.Slam();
            _rightHand.Slam(3f);
        }
        else
        {
            _rightHand.Slam();
            _leftHand.Slam(3f);
        }

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    /// <summary>
    /// 임의로 만든 한손 공격
    /// </summary>
    /// <returns></returns>
    public NodeStatus OneHandSlamAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("Slam");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curSlamCount++;
        _curRunningHandsCount++;

        if (Vector3.Distance(_target.position, _leftHand.transform.position) <= Vector3.Distance(_target.position, _rightHand.transform.position))
            _leftHand.Slam();
        else
            _rightHand.Slam();

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    bool _attackFlag = false;
    public NodeStatus OtherSlamAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        //if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        if(_attackFlag == false)
        {
            IsAttacking = true;
            _patternCoolTime = 0;
            _curSlamCount++;
            _curRunningHandsCount++;

            if (Vector3.Distance(_target.position, _leftHand.transform.position) <= Vector3.Distance(_target.position, _rightHand.transform.position))
                _leftHand.Slam();
            else
                _rightHand.Slam();

            return NodeStatus.Running;
        }
        else
        {
            _attackFlag = false;
            return NodeStatus.Success;
        }
    }

    public NodeStatus SummonOrbAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("SO");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curBreathCount = 0;

        _head.SummonOrb();

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    public NodeStatus BreathAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("Breath");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curBreathCount++;

        _head.Breath();

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }
    #endregion

    #region 페이즈 2
    public NodeStatus ChangePhase2()
    {
        IsDirecting = true;
        IsInvincible = true;

        CurPhase = 2;

        // 보스 페이즈 브금 변경, 현재 오류있어서 페이드 아웃은 제거
        SetBgmPhase2();
        //Manager.Audio.StopBgm(2, SetBgmPhase2);

        // 포효 애니메이션 실행, 애니메이션이 끝나면 연출 및 무적 해제
        _head.TriggerAnimation(Game.Monster.BoneReaperAnimatorParams.PhaseChange);
        // 능력치 강화
        MonsterData.AddModifier(new StatModifier(StatType.Attack, ModifierType.Add, 10, this));
        _leftHand.PhaseChange();
        _rightHand.PhaseChange();
        _patternMaxCoolTime = 2f;

        // BT 변경
        _curBT.ChangeAttackSelector();

        return NodeStatus.Success;
    }

    private void SetBgmPhase2()
    {
        Manager.Audio.SetBgm(AudioKey.BGM.BGM_BOSS_PHASE2);
    }

    public NodeStatus AdvSlamAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("Slam2");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curSlamCount++;
        _curRunningHandsCount += 2;

        if (Vector3.Distance(_target.position, _leftHand.transform.position) <= Vector3.Distance(_target.position, _rightHand.transform.position))
        {
            _leftHand.Slam();
            _rightHand.Slam(3f);
        }
        else
        {
            _rightHand.Slam();
            _leftHand.Slam(3f);
        }

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    public NodeStatus AdvBreathAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("Breath");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curBreathCount++;

        _head.Breath();

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    public NodeStatus AdvLaserAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("Laser2");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curSlamCount = 0;

        _leftHand.Laser();
        _rightHand.Laser();

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    public NodeStatus AdvSummonOrbAttack()
    {
        if (IsAttacking) return NodeStatus.Running;
        if (_patternCoolTime < PatternMaxCoolTime) return NodeStatus.Failure;

        //Debug.Log("SO");
        IsAttacking = true;
        _patternCoolTime = 0;
        _curBreathCount = 0;

        _head.SummonOrbPhase2();

        return IsAttacking ? NodeStatus.Running : NodeStatus.Success;
    }

    #endregion

    public void HitFlash(SpriteRenderer sr, Coroutine hitEffect, Material originMat)
    {
        if (HitFlashMat == null)
            return;

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

    private void DestroyOnDeath()
    {
        Destroy(gameObject);
    }

    private void DestroyBossOnPlayerDied()
    {
        Invoke("DestroyOnDeath", 2f);
    }

    private void StopBoss()
    {
        IsDirecting = true;
    }

    private void CheckFunnel()
    {
        for(int i = 0; i < 9; i++)
        {
            if (!_isHpLower[i])
            {
                if ((MonsterData.CurHp / MonsterData.Data.MaxHp) * 100 <= 10f * (9-i))
                {
                    //Debug.Log((MonsterData.CurHp / MonsterData.Data.MaxHp) * 10);
                    _isHpLower[i] = true;
                    Manager.Analytics.SendFunnelStep(FunnelStep._Boss, i + 3);
                }
                else
                    break;
            }
        }
    }

    private void DebugBossDamage()
    {
        if(Input.GetKeyDown(KeyCode.Minus))
        {
            _head.TakeDamage(100, gameObject);
        }
        else if(Input.GetKeyDown(KeyCode.Equals))
        {
            _head.TakeDamage(2000, gameObject);
        }
    }
}
