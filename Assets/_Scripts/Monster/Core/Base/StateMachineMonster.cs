using Game.Monster;
using Game.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상태머신을 사용하는 몬스터 클래스
/// </summary>
public abstract class StateMachineMonster : NormalMonster
{
    // 몬스터 상태 머신
    protected MonsterStateMachine _stateMachine;
    public MonsterStateMachine StateMachine { get { return _stateMachine; } }

    protected IAttackable _curAttack;
    protected IMovable _curPatrolMovement;
    protected IMovable _curChaseMovement;

    protected override void Awake()
    {
        base.Awake();

        _stateMachine = new MonsterStateMachine(this);
        OnDiedEnter += DisableEffects;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _stateMachine?.Init();
        Rb.bodyType = RigidbodyType2D.Dynamic;
        Col.isTrigger = false;

        if(PlayerManager.Instance != null)
            PlayerManager.Instance.Player.OnDied += OnPlayerDied;
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

    public IMovable GetChaseMovement()
    {
        return _curChaseMovement;
    }

    public IMovable GetPatrolMovement()
    {
        return _curPatrolMovement;
    }

    // 데미지 적용 메서드
    public override void TakeDamage(float damage, bool hitSfxMute = false)
    {
        // 체력이 0이하면 종료
        // 만약 넉백이 외부에서 구현한다면 그 부분은 호출하는 쪽에서 막을 필요가 있음
        if (_dataHandler.CurHp <= 0)
            return;

        _dataHandler.TakeDamage(damage);
#if UNITY_EDITOR
        Debug.Log($"[Damage] {name}이 받은 피해: {damage}");
#endif
        OnHit?.Invoke();
        DamageIndicator.Instance.GetDamage(transform.position + Vector3.up * 0.5f, damage);

        if (_dataHandler.CurHp <= 0)
        {
            Attack.StopAttack();
            //attacker?.GetComponent<PlayerCharacter>()?.Kill();
            if(PlayerCharacter.Instance != null)
                PlayerCharacter.Instance.Kill();
            _stateMachine.ChangeState(Game.Monster.StateType.Die);
        }
        else
        {
            if(!hitSfxMute)
                Manager.Audio.Play(MonsterData.Data.HitSfx, transform);
            if(!IsSuperArmor)
            {
                _stateMachine.ChangeState(Game.Monster.StateType.Hit);
            }
            else if(Target == null)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }


    private void DisableEffects()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.gameObject.SetActive(false);
        }
    }

    private void OnPlayerDied()
    {
        UnregisterDieEvent();
        Invoke("DestroyMonster", 2f);
    }

    private void DestroyMonster()
    {
        Destroy(gameObject);
    }

    public void UnregisterDieEvent()
    {
        if(PlayerManager.Instance != null)
            PlayerManager.Instance.Player.OnDied -= OnPlayerDied;
    }
}
