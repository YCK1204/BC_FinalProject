using UnityEngine;

public class NormalViewState : UiStateBase
{
    public NormalViewState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        ui.Animator.SetTrigger("ShowNormal");
    }

    public override void Exit() { }
}
