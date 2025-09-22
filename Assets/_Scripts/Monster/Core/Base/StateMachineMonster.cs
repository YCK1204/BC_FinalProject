using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상태머신을 사용하는 몬스터 클래스
/// </summary>
public abstract class StateMachineMonster : BaseMonster
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
    public override void TakeDamage(int damage)
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

}
