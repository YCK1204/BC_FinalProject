using UnityEngine;

namespace Game.Player
{
    public class PlayerDieState : PlayerBaseState
    {
        public PlayerDieState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.IsAttacking = false;
            _stateMachine.IsDashing = false;
            _stateMachine.MovementSpeedModifier = 0f;

            Manager.Data.playerSOData.ResetData();
#if UNITY_2022_3_OR_NEWER
            _stateMachine.Player.Rb.linearVelocity = Vector2.zero;
#else
            _stateMachine.Player.Rb.velocity = Vector2.zero;
#endif
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.DashParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.HurtParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.DieParameterHash);
            _stateMachine.Player.Animator.Play("Die", 0, 0f);
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.DieParameterHash);
        }

        public override void Update() { }
        public override void PhysicsUpdate() 
        {
#if UNITY_2022_3_OR_NEWER
            if (_stateMachine.Player.Rb.linearVelocity != Vector2.zero)
            {
                _stateMachine.Player.Rb.linearVelocity = Vector2.zero;
            }
#else
            if (_stateMachine.Player.Rb.velocity != Vector2.zero)
            {
                _stateMachine.Player.Rb.velocity = Vector2.zero;
            }
#endif
        }
    }
}
