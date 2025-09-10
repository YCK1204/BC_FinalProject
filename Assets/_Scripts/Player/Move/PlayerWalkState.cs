using UnityEngine;

namespace GameSystem
{
    public class PlayerWalkState : PlayerGroundedState
    {
        public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.MovementSpeedModifier = _stateMachine.Player.Data.GroundData.WalkSpeedModifier;
            base.Enter();
            StartAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        }

        public override void Update()
        {
            base.Update();
            if (_stateMachine.MovementInput == Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
            }
        }
    }
}
