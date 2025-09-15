using UnityEngine;

namespace GameSystem
{
    public class PlayerDoubleJumpState : PlayerAirState
    {
        public PlayerDoubleJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.Player.ForceReceiver.Jump(_stateMachine.Player.Data.AirData.DoubleJumpForce);
            _stateMachine.JumpsRemaining = Mathf.Max(0, _stateMachine.JumpsRemaining - 1);
            base.Enter();
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
        }

        public override void Update()
        {
            base.Update();
#if UNITY_2022_3_OR_NEWER
            if (_stateMachine.Player.Rb.linearVelocity.y <= 0f)
                _stateMachine.ChangeState(_stateMachine.AirState);
#else
            if (_stateMachine.Player.Rb.velocity.y <= 0f)
                _stateMachine.ChangeState(_stateMachine.AirState);
#endif
        }
    }
}
