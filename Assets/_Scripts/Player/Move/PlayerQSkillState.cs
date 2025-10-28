using UnityEngine;

namespace Game.Player
{
    public class PlayerQSkillState : PlayerSkillState
    {
        public PlayerQSkillState(PlayerStateMachine stateMachine)
            : base(stateMachine, stateMachine.Player.Data.SkillData.QSkillCastingDuration) { }

        public override void Enter()
        {
            base.Enter();
            PlayerManager.Instance.CooldownQ.StartCooldown(_stateMachine.Player.Data.SkillData.GetQSkillCooldown(_stateMachine.Player.Data.CombatData.SkillHaste));
            Debug.Log("Q 스킬사용");

            _stateMachine.Player.ShadowSlashSkill.Execute();
        }

        public override void Exit()
        {
            base.Exit();
            _stateMachine.MarkQSkillUsed();
            Debug.Log("Q 스킬종료");
        }
    }
}