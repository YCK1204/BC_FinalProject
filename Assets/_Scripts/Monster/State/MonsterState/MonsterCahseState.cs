using UnityEngine;

public class MonsterCahseState : MonsterBaseState
{
    public MonsterCahseState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Chase;
    }
}
