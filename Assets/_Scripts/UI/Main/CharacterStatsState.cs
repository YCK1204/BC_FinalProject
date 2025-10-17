using UnityEngine;

public class CharacterStatsState : UiStateBase
{
    public CharacterStatsState(UIController controller) : base(controller) { }

    public override void Enter()
    {
        ui.Animator.SetTrigger("ShowFull");
    }

    public override void Exit() { }
}
