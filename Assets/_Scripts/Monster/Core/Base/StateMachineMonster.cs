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

    protected override void Awake()
    {
        base.Awake();

        _stateMachine = new MonsterStateMachine(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _stateMachine?.Init();
        Rb.bodyType = RigidbodyType2D.Dynamic;
        Col.isTrigger = false;
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

    // 데미지 적용 메서드
    public override void TakeDamage(float damage, GameObject attacker = null)
    {
        // 체력이 0이하면 종료
        // 만약 넉백이 외부에서 구현한다면 그 부분은 호출하는 쪽에서 막을 필요가 있음
        if (_dataHandler.CurHp <= 0)
            return;

        _dataHandler.TakeDamage(damage);
        OnHit?.Invoke();

        if (_dataHandler.CurHp <= 0)
        {
            Attack.StopAttack();
            //attacker?.GetComponent<PlayerCharacter>()?.Kill();
            PlayerCharacter.Instance.Kill();
            _stateMachine.ChangeState(Game.Monster.StateType.Die);
        }
        else if(!IsSuperArmor)
        {
            _stateMachine.ChangeState(Game.Monster.StateType.Hit);
        }
    }

}
