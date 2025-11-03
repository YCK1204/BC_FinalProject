using UnityEngine;

public class HiddenState : UiStateBase
{
    public HiddenState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:out");
        ui.ShowScreen("off");
        ui.Animator.SetInteger("State", 0);
        PlayerManager.Instance.Player.SetPlayerInput(true);
        PlayerManager.Instance.Player.Animator.Play("Idle");
    }

    public override void Exit() { }
}