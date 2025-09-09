using UnityEngine;

public class WideViewState : UiStateBase
{
    public WideViewState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        ui.Animator.SetTrigger("ShowWide");
    }

    public override void Exit() { }
}