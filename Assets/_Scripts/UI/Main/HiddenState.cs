using UnityEngine;

public class HiddenState : UiStateBase
{
    public HiddenState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:out");
        ui.ShowScreen("off");
        ui.Animator.SetInteger("State", 0);
    }

    public override void Exit() { }
}