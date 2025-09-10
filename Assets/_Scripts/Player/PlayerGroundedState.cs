using UnityEngine;

namespace GameSystem
{
    public class PlayerGroundedState : PlayerBaseState
    {
        private float _groundBuffer = 0.1f;
        private float _timer;

        public PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            StopAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.GroundParameterHash);
            _timer = _groundBuffer;
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.GroundParameterHash);
        }

        public override void Update()
        {
            ReadMoveInput();

#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            bool jumpPressed = kb != null && kb.spaceKey.wasPressedThisFrame;
#else
            bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
#endif
            if (jumpPressed && _stateMachine.Player.IsGrounded())
            {
                _stateMachine.ChangeState(_stateMachine.JumpState);
                return;
            }

            if (_stateMachine.MovementInput == Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }

            if (_stateMachine.Player.IsGrounded())
            {
                _timer = _groundBuffer;
            }
            else
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f) _stateMachine.ChangeState(_stateMachine.AirState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
