using UnityEngine;

namespace Game.Player
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            _stateMachine.MovementSpeedModifier = 1f;
            StartAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Update()
        {
            base.Update();
            ReadMoveInput();
            if (_stateMachine.MovementInput.x != 0f)
            {
                _stateMachine.ChangeState(_stateMachine.WalkState);
                return;
            }
            if (!_stateMachine.Player.IsGrounded())
            {
                _stateMachine.ChangeState(_stateMachine.AirState);
                return;
            }
        }

        public override void PhysicsUpdate() { base.PhysicsUpdate(); }
    }
}
