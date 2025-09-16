using UnityEngine;

public class FullScreenState : UiStateBase
{
    public FullScreenState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        ui.Animator.SetTrigger("ShowFull");
    }

    public override void Exit() { }
}
