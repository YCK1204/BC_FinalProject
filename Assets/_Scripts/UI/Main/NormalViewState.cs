using UnityEngine;

public class NormalViewState : UiStateBase
{
    public NormalViewState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:normal");
        ui.ShowScreen("main");
        ui.Animator.SetInteger("State", 1);
    }

    public override void Exit() { }
}
