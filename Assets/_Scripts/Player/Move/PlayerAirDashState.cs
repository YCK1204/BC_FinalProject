using UnityEngine;

namespace GameSystem
{
    public class PlayerAirDashState : PlayerBaseState
    {
        float _timer;
        Vector2 _dashDir;

        public PlayerAirDashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.IsDashing = true;
            _timer = _stateMachine.DashDuration;

            _dashDir = new Vector2(_stateMachine.FacingSign, 0f);

            _stateMachine.MovementSpeedModifier = _stateMachine.DashSpeedMult;
            StartAnimation(_stateMachine.Player.AnimationData.DashParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);

            if (_stateMachine.InvincibleDuringDash)
                _stateMachine.Player.SetInvincible(true);
        }

        public override void Exit()
        {
            _stateMachine.IsDashing = false;
            _stateMachine.MovementSpeedModifier = 1f;
            StopAnimation(_stateMachine.Player.AnimationData.DashParameterHash);

            if (_stateMachine.InvincibleDuringDash)
                _stateMachine.Player.SetInvincible(false);

            _stateMachine.MarkDashedNow();
        }

        public override void Update()
        {
            _stateMachine.MovementInput = _dashDir;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                if (_stateMachine.Player.IsGrounded())
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                else
                    _stateMachine.ChangeState(_stateMachine.AirState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
