using UnityEngine;

namespace GameSystem
{
    public class PlayerJumpState : PlayerAirState
    {
        public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.Player.ForceReceiver.Jump(_stateMachine.Player.Data.AirData.JumpForce);
            _stateMachine.JumpsRemaining = Mathf.Max(0, _stateMachine.JumpsRemaining - 1);

            _stateMachine.MovementSpeedModifier = 1f;
            StartAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
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
            if (dash && _stateMachine.CanDash())
            {
                _stateMachine.ChangeState(_stateMachine.AirDashState);
                return;
            }
            if (jump && _stateMachine.JumpsRemaining > 0)
            {
                _stateMachine.ChangeState(_stateMachine.DoubleJumpState);
                return;
            }
#if UNITY_2022_3_OR_NEWER
            if (_stateMachine.Player.Rb.linearVelocity.y <= 0f)
            {
                StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
                _stateMachine.ChangeState(_stateMachine.AirState);
                return;
            }
#else
            if (_stateMachine.Player.Rb.velocity.y <= 0f)
            {
                StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
                _stateMachine.ChangeState(_stateMachine.AirState);
                return;
            }
#endif
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
