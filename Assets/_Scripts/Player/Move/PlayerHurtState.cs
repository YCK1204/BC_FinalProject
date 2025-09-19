using UnityEngine;

namespace Game.Player
{
    public class PlayerHurtState : PlayerBaseState
    {
        float _timer;

        public PlayerHurtState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _timer = _stateMachine.Player.Data.HurtData.Duration;
            _stateMachine.IsAttacking = false;
            _stateMachine.IsDashing = false;
            _stateMachine.MovementSpeedModifier = 0f;
            StartAnimation(_stateMachine.Player.AnimationData.HurtParameterHash);
            if (_stateMachine.Player.Data.HurtData.InvincibleDuringHurt)
                _stateMachine.Player.SetInvincible(true);
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.HurtParameterHash);
            _stateMachine.MovementSpeedModifier = 1f;
            if (_stateMachine.Player.Data.HurtData.InvincibleDuringHurt)
                _stateMachine.Player.SetInvincible(false);
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                if (_stateMachine.Player.IsGrounded())
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                else
                    _stateMachine.ChangeState(_stateMachine.AirState);
            }
        }

        public override void PhysicsUpdate() { }
    }
}
