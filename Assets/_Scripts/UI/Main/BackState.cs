using UnityEngine;

public class BackState : UiStateBase
{
    public BackState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        ui.Animator.SetInteger("State", ui.Animator.GetInteger("State") - 1);
    }

    public override void Exit() { }
}