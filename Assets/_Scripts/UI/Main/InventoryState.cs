using UnityEngine;

public class InventoryState : UiStateBase
{
    public InventoryState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:inven");
        ui.ShowScreen("inven");
    }

    public override void Exit() { }
}