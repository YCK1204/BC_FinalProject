// PlayerDashState.cs
using UnityEngine;

namespace Game.Player
{
    public class PlayerDashState : PlayerBaseState
    {
        float _timer;
        Vector2 _dashDir;
        float _prevGravity;

        public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //base.Enter();
            _stateMachine.IsDashing = true;
            _timer = _stateMachine.DashDuration;

            _dashDir = new Vector2(_stateMachine.FacingSign, 0f);

            _prevGravity = _stateMachine.Player.Rb.gravityScale;
            _stateMachine.Player.Rb.gravityScale = 0f;
#if UNITY_2022_3_OR_NEWER
            var v = _stateMachine.Player.Rb.linearVelocity;
            _stateMachine.Player.Rb.linearVelocity = new Vector2(v.x, 0f);
#else
            var v = _stateMachine.Player.Rb.velocity;
            _stateMachine.Player.Rb.velocity = new Vector2(v.x, 0f);
#endif

            _stateMachine.MovementSpeedModifier = _stateMachine.DashSpeedMult;
            StartAnimation(_stateMachine.Player.AnimationData.DashParameterHash);

            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);

            if (_stateMachine.InvincibleDuringDash)
                _stateMachine.Player.SetInvincible(true);

            _stateMachine.Player.SetLayerCollisionIgnore(_stateMachine.Player.Data.DashData.PassThroughLayers, true);
        }

        public override void Exit()
        {
            _stateMachine.Player.StartRound();

            _stateMachine.IsDashing = false;
            _stateMachine.MovementSpeedModifier = 1f;
            _stateMachine.Player.Rb.gravityScale = _prevGravity;
            StopAnimation(_stateMachine.Player.AnimationData.DashParameterHash);

            if (_stateMachine.InvincibleDuringDash)
                _stateMachine.Player.SetInvincible(false);

            _stateMachine.Player.SetLayerCollisionIgnore(_stateMachine.Player.Data.DashData.PassThroughLayers, false);
            _stateMachine.MarkDashedNow();
        }

        public override void Update()
        {
            _stateMachine.MovementInput = _dashDir;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                if (_stateMachine.Player.IsGrounded())
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                else
                    _stateMachine.ChangeState(_stateMachine.AirState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
