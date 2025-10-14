namespace Game.Player
{
    public class PlayerWalkState : PlayerGroundedState
    {
        public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            _stateMachine.MovementSpeedModifier = _stateMachine.Player.Data.GroundData.WalkSpeedModifier;
            _stateMachine.Player.Animator.Play("walk", 0, 0f);
            StartAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
        }

        public override void Update()
        {
            base.Update();
            ReadMoveInput();
            if (_stateMachine.MovementInput.x == 0f)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
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
