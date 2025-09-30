using UnityEngine;

public class HiddenState : UiStateBase
{
    public HiddenState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:out");
        ui.ShowScreen("off");
        ui.Animator.SetInteger("State", 0);
        //Time.timeScale = 1f;
        PlayerManager.Instance.Player.SetPlayerInput(true);
    }

    public override void Exit() { }
}