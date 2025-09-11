using UnityEngine;

public class MonsterDieState : MonsterBaseState
{
    public MonsterDieState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Die;
    }

    public override void Enter()
    {
        base.Enter();

        _stateMachine.Owner.Anim.SetTrigger(Common.AnimatorParams.Die);
    }
}
