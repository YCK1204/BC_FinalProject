using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 몬스터 상위 클래스
/// </summary>
public abstract class NormalMonster : BaseMonster, Game.Monster.IDamageable
{
    /// <summary>
    /// 만약 단일책임원칙에 따라 스크립트를 분리하면 어떻게 하지?
    /// 몬스터의 데이터 분리 -> 했음
    /// 공격쪽은 이미 분리함
    /// </summary>

    // 몬스터 공격 컴포넌트
    protected MonsterAttack _attack;
    public MonsterAttack Attack { get { return _attack; } }

    protected Rigidbody2D _rb;
    public Rigidbody2D Rb { get { return _rb; } }
    protected Collider2D _col;
    public Collider2D Col { get { return _col; } }
    protected Animator _anim;
    public Animator Anim { get { return _anim; } }
    protected SpriteRenderer _sr;
    public SpriteRenderer Sr { get { return _sr; } }

    protected List<Game.Monster.ISpecialAbillity> _abillityList;

    protected List<Collider2D> _ignoredColliderList;

    public Action Ondetect;
    public Action OnUpdate;
    public Action OnHit;

    protected Material _originMat;
    [SerializeField] protected Material FlashMat;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _anim = GetComponentInChildren<Animator>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _originMat = _sr.material;

        _attack = GetComponentInChildren<MonsterAttack>();
        _dataHandler = Extension.GetOrAddComponent<MonsterDataHandler>(this.gameObject);
        _dataHandler.Owner = this;

        _abillityList = new List<Game.Monster.ISpecialAbillity>();

        if (_ignoredColliderList != null)
            _ignoredColliderList.Clear();
        else
            _ignoredColliderList = new List<Collider2D>();

    }

    protected virtual void Init()
    {
        Game.Monster.ISpecialAbillity[] abillities = GetComponents<Game.Monster.ISpecialAbillity>();
        foreach (Game.Monster.ISpecialAbillity abillity in abillities)
        {
            _abillityList.Add(abillity);
            abillity.Init(this);
        }

    }

    protected virtual void OnEnable()
    {
        _dataHandler.Init();
        OnHit += HitFlash;

        RegisterIgnoreCollider(PlayerManager.Instance?.Player.GetComponent<Collider2D>());

        Init();
    }

    protected virtual void OnDisable()
    {
        ResetTarget();
        OnDied = null;
        Ondetect = null;
        OnUpdate = null;
        OnHit = null;

        _abillityList.Clear();
        _ignoredColliderList.Clear();
    }

    public void HitFlash()
    {
        if (FlashMat == null)
            return;

        _sr.material = FlashMat;
        FlashMat.SetFloat("_FlashAmount", 1f);

        Invoke(nameof(OffHitFlash), 0.1f);
    }

    public void OffHitFlash()
    {
        FlashMat.SetFloat("_FlashAmount", 0f);

        _sr.material = _originMat;
    }

    public abstract void TakeDamage(float damage, bool hitSfxMute = false);

    /// <summary>
    /// 해당 콜라이더를 충돌 무시로 지정하고 리스트에 넣음
    /// </summary>
    /// <param name="collider">충돌을 무시할 콜라이더</param>
    public void RegisterIgnoreCollider(Collider2D collider)
    {
        if (collider != null && !_ignoredColliderList.Contains(collider))
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
        if (collider != null && _ignoredColliderList.Contains(collider))
        {
            Physics2D.IgnoreCollision(Col, collider, false);
            _ignoredColliderList.Remove(collider);
        }
    }
}
