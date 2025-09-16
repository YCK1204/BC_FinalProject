using UnityEngine;

namespace GameSystem
{
    public abstract class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.JumpsRemaining = _stateMachine.MaxJumps;
        }

        public override void Update()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            bool jump = kb != null && kb.spaceKey.wasPressedThisFrame;
            bool dash = kb != null && kb.sKey.wasPressedThisFrame;
            bool attack = kb != null && kb.aKey.wasPressedThisFrame;
#else
            bool jump   = Input.GetKeyDown(KeyCode.Space);
            bool dash   = Input.GetKeyDown(KeyCode.S);
            bool attack = Input.GetKeyDown(KeyCode.A);
#endif
            _stateMachine.DashPressed = dash;

            if (attack && !_stateMachine.IsAttacking && _stateMachine.Player.IsGrounded())
            {
                _stateMachine.ChangeState(_stateMachine.AttackState);
                return;
            }
            if (jump && _stateMachine.Player.IsGrounded() && _stateMachine.JumpsRemaining > 0)
            {
                _stateMachine.ChangeState(_stateMachine.JumpState);
                return;
            }
            if (dash && _stateMachine.CanDash())
            {
                _stateMachine.ChangeState(_stateMachine.DashState);
                return;
            }
        }

        public override void PhysicsUpdate() { base.PhysicsUpdate(); }
    }
}
