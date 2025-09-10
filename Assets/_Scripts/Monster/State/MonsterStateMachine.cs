using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : Game.Monster.IStateMachine<Monster>
{
    // 해당 스테이트 머신의 소유자
    public Monster Owner { get; private set; }

    // 몬스터의 현재 상태
    protected MonsterBaseState curState;
    // 몬스터의 상태를 담은 딕셔너리
    protected Dictionary<Common.StateType, MonsterBaseState> stateDic;

    protected MonsterStateMachine(Monster owner)
    {
        Owner = owner;
        Init();
    }

    protected virtual void Init()
    {
        stateDic = new Dictionary<Common.StateType, MonsterBaseState>();
        stateDic.Add(Common.StateType.Idle, new MonsterIdleState(this));
        stateDic.Add(Common.StateType.Patrol, new MonsterPatrolState(this));
        stateDic.Add(Common.StateType.Attack, new MonsterAttackState(this));
    }

    public void ChangeState(Common.StateType type)
    {
        if (stateDic.ContainsKey(type))
        {
            curState?.Exit();
            curState = stateDic[type];
            curState?.Enter();
        }
        else
        {
            Debug.LogError("딕셔너리에 존재하지 않는 상태입니다.");
        }
    }

    public void Update()
    {
        curState?.Update();
    }

    public void FixedUpdate()
    {
        curState?.FixedUpdate();
    }
}
