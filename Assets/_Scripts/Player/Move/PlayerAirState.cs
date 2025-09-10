using UnityEngine;

namespace GameSystem
{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.MovementSpeedModifier = 1f;
            StartAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            if (_stateMachine.Player.Rb.linearVelocity.y > 0f)
            {
                StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            }
            else
            {
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            }
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
        }

        public override void Update()
        {
            ReadMoveInput();

            if (_stateMachine.Player.Rb.linearVelocity.y <= 0f)
            {
                StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            }

            if (_stateMachine.Player.Rb.linearVelocity.y <= 0f && _stateMachine.Player.IsGrounded())
            {
                if (_stateMachine.MovementInput == Vector2.zero)
                {
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                }
                else
                {
                    _stateMachine.ChangeState(_stateMachine.WalkState);
                }
            }
        }
    }
}
