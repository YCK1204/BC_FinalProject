using UnityEngine;

namespace GameSystem
{
    public class PlayerJumpState : PlayerAirState
    {
        public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.Player.ForceReceiver.Jump(_stateMachine.JumpForce);
            base.Enter();
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
        }

        public override void Update()
        {
            base.Update();
            if (_stateMachine.Player.Rb.linearVelocity.y <= 0f)
            {
                _stateMachine.ChangeState(_stateMachine.AirState);
            }
        }
    }
}
