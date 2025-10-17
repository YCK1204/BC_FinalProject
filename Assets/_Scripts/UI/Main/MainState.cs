using UnityEngine;

public class MainState : UiStateBase
{
    public MainState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:normal");
        ui.ShowScreen("main");
        ui.Animator.SetInteger("State", 1);
        //Time.timeScale = 0;
        PlayerManager.Instance.Player.SetPlayerInput(false);
        PlayerManager.Instance.Player.Animator.Play("Phon");
    }

    public override void Exit() { }
}
