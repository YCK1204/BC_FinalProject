using UnityEngine;

public class SettingsState : UiStateBase
{
    public SettingsState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("ui:setting");
        ui.Animator.SetBool("Setting", true);
    }

    public override void Exit() { }
}
