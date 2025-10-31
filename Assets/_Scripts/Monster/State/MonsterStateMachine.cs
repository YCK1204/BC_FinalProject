using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : Game.Monster.IStateMachine<StateMachineMonster>
{
    // 해당 스테이트 머신의 소유자
    public StateMachineMonster Owner { get; private set; }

    // 몬스터의 현재 상태
    protected MonsterBaseState curState;
    protected MonsterBaseState prevState;
    // 몬스터의 상태를 담은 딕셔너리
    protected Dictionary<Game.Monster.StateType, MonsterBaseState> stateDic;

    public MonsterStateMachine(StateMachineMonster owner)
    {
        Owner = owner;
        Init();
    }

    public virtual void Init()
    {
        stateDic = new Dictionary<Game.Monster.StateType, MonsterBaseState>();
        stateDic.Add(Game.Monster.StateType.Idle, new MonsterIdleState(this));
        stateDic.Add(Game.Monster.StateType.Patrol, new MonsterPatrolState(this));
        stateDic.Add(Game.Monster.StateType.Attack, new MonsterAttackState(this));
        stateDic.Add(Game.Monster.StateType.Chase, new MonsterCahseState(this));
        stateDic.Add(Game.Monster.StateType.Hit, new MonsterHitState(this));
        stateDic.Add(Game.Monster.StateType.Die, new MonsterDieState(this));

        prevState = null;
        curState = stateDic[Game.Monster.StateType.Idle];
        curState.Enter();
    }

    public void ChangeState(Game.Monster.StateType type)
    {
        // 현재 상태와 같은 상태로 변경 시도하면 종료
        if (curState.StateType == type && curState.StateType != Game.Monster.StateType.Hit)
            return;

        // 변경하려는 스테이트가 딕셔너리에 존재해야만 변경
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

    public void ChangePrevState()
    {
        // 이전 상태가 null이거나 현재 상태와 동일하다면 종료
        if (prevState == null || prevState.StateType == curState.StateType)
            return;

        // 음 이러면 이전 상태는 한개 밖에 저장이 안되네? 큐로 가지고 있어야 하나?
        curState?.Exit();
        curState = prevState;
        prevState = null;
        curState?.Enter();
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeState(Game.Monster.StateType.Die);
            return;
        }
#endif
        curState?.Update();
    }

    public void FixedUpdate()
    {
        curState?.FixedUpdate();
    }
}
