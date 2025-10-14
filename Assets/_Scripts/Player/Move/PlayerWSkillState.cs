using UnityEngine;

namespace Game.Player
{
    public class PlayerWSkillState : PlayerSkillState
    {
        public PlayerWSkillState(PlayerStateMachine stateMachine)
            : base(stateMachine, stateMachine.Player.Data.SkillData.WSkillCastingDuration) { }

        public override void Enter()
        {
            base.Enter();
            PlayerManager.Instance.CooldownW.StartCooldown(_stateMachine.Player.Data.SkillData.WSkillCooldown);
            Debug.Log("W 스킬사용");

            _stateMachine.Player.DoubleStrikeSkill.Execute();
        }

        public override void Exit()
        {
            base.Exit();
            _stateMachine.MarkWSkillUsed();
            Debug.Log("W 스킬종료");
        }
    }
}