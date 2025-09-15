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
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
#if UNITY_2022_3_OR_NEWER
            if (_stateMachine.Player.Rb.linearVelocity.y > 0f)
                StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            else
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
#else
            if (_stateMachine.Player.Rb.velocity.y > 0f)
                StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            else
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
#endif
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

#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            bool jump = kb != null && kb.spaceKey.wasPressedThisFrame;
            bool dash = kb != null && kb.sKey.wasPressedThisFrame;
#else
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool dash = Input.GetKeyDown(KeyCode.S);
#endif
            if (jump && _stateMachine.JumpsRemaining > 0)
            {
                _stateMachine.ChangeState(_stateMachine.DoubleJumpState);
                return;
            }
            if (dash && _stateMachine.CanDash())
            {
                _stateMachine.DashPressed = true;
                _stateMachine.ChangeState(_stateMachine.AirDashState);
                return;
            }

#if UNITY_2022_3_OR_NEWER
            if (_stateMachine.Player.Rb.linearVelocity.y <= 0f)
            {
                StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            }
            if (_stateMachine.Player.Rb.linearVelocity.y <= 0f && _stateMachine.Player.IsGrounded())
            {
                if (_stateMachine.MovementInput == Vector2.zero)
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                else
                    _stateMachine.ChangeState(_stateMachine.WalkState);
            }
#else
            if (_stateMachine.Player.Rb.velocity.y <= 0f)
            {
                StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            }
            if (_stateMachine.Player.Rb.velocity.y <= 0f && _stateMachine.Player.IsGrounded())
            {
                if (_stateMachine.MovementInput == Vector2.zero)
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                else
                    _stateMachine.ChangeState(_stateMachine.WalkState);
            }
#endif
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
