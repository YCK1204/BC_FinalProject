using UnityEngine;

public class MonsterDieState : MonsterBaseState
{
    public MonsterDieState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Die;
    }
}
