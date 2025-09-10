using Common;
using UnityEngine;

public abstract class MonsterBaseState : Game.Monster.IState
{
    protected MonsterStateMachine _stateMachine;

    public StateType StateType { get; protected set; }

    public MonsterBaseState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
