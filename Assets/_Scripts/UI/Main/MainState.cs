using UnityEngine;
using static AudioKey;

public class MainState : UiStateBase
{
    public MainState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:normal");
        ui.ShowScreen("main");
        PlayerManager.Instance.Player.SetPlayerInput(false);
    }

    public override void Exit() { }
}
