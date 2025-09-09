using UnityEngine;

public class HiddenState : UiStateBase
{
    public HiddenState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        ui.Animator.SetTrigger("Hide");
    }

    public override void Exit() { }
}