using UnityEngine;

namespace GameSystem
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.MovementSpeedModifier = 0f;
            base.Enter();
            StartAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Update()
        {
            base.Update();
            if (_stateMachine.MovementInput != Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.WalkState);
            }
        }
    }
}
