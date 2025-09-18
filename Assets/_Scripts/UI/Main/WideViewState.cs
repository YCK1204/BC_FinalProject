using UnityEngine;

public class WideViewState : UiStateBase
{
    public WideViewState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:wide");
        ui.ShowScreen("inven");
        ui.Animator.SetInteger("State", 2);
    }

    public override void Exit() { }
}