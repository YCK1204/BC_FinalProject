using UnityEngine;

namespace Game.Player
{
    public class PlayerJumpState : PlayerAirState
    {
        public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        GameObject _JumpEffectPrefab;

        public override void Enter()
        {
            float jumpForce;
            Vector3 pos = _stateMachine.Player.transform.position;

            if (_stateMachine.JumpsRemaining == _stateMachine.MaxJumps)
            {
                jumpForce = _stateMachine.Player.Data.AirData.JumpForce;
                _JumpEffectPrefab = _stateMachine.Player.Data.AirData.JumpEffectPrefab1;
                pos.y -= 0.45f;
            }
            else
            {
                jumpForce = _stateMachine.Player.Data.AirData.DoubleJumpForce;
                _JumpEffectPrefab = _stateMachine.Player.Data.AirData.JumpEffectPrefab2;
                pos.y -= 0.5f;
            }

            _stateMachine.Player.ForceReceiver.Jump(jumpForce);
            _stateMachine.JumpsRemaining = Mathf.Max(0, _stateMachine.JumpsRemaining - 1);

            _stateMachine.MovementSpeedModifier = 1f;
            StartAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            _stateMachine.Player.Animator.Play("jump", 0, 0f);

            if (_JumpEffectPrefab != null)
                PlayerPool.Instance.GetFromPool(_JumpEffectPrefab, pos, Quaternion.Euler(0f, 0f, 0f));


            }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
        }

        public override void Update()
        {
            if (_stateMachine.IsAttacking || _stateMachine.IsDashing)
            {
                return;
            }

#if UNITY_2022_3_OR_NEWER
            if (_stateMachine.Player.Rb.linearVelocity.y <= 13f)
            {
                _stateMachine.ChangeState(_stateMachine.AirState);
                return;
            }
#else
            if (_stateMachine.Player.Rb.velocity.y <= 0f)
            {
                _stateMachine.ChangeState(_stateMachine.AirState);
                return;
            }
#endif

            ReadMoveInput();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}