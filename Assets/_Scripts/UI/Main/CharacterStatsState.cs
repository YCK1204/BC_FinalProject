using UnityEngine;

public class CharacterStatsState : UiStateBase
{
    public CharacterStatsState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:stats");
        ui.ShowScreen("stats");
    }

    public override void Exit() { }
}
