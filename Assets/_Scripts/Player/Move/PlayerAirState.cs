using UnityEngine;

namespace Game.Player
{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            StartAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
#if UNITY_2022_3_OR_NEWER
            if (_stateMachine.Player.Rb.linearVelocity.y > 0f)
                StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            else
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
#else
            if (_stateMachine.Player.Rb.velocity.y > 0f)
                StartAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            else
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
#endif
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.AirParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
        }

        public override void Update()
        {
            ReadMoveInput();

            if (_stateMachine.InputActive)
            {
#if ENABLE_INPUT_SYSTEM
                var kb = UnityEngine.InputSystem.Keyboard.current;
                bool jump = kb != null && kb.spaceKey.wasPressedThisFrame;
                bool dash = kb != null && kb.sKey.wasPressedThisFrame;
                bool attack = kb != null && kb.aKey.wasPressedThisFrame;
                //bool wSkill = kb != null && kb.wKey.wasPressedThisFrame;
#else
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool dash = Input.GetKeyDown(KeyCode.S);
            bool attack = Input.GetKeyDown(KeyCode.A);
#endif
                if (attack && !_stateMachine.IsAttacking && _stateMachine.Player.Rb.linearVelocity.y <= 6f)
                {
                    _stateMachine.ComboIndex = 2;
                    _stateMachine.ChangeState(_stateMachine.AirAttackState);
                    return;
                }
                //if (wSkill && _stateMachine.CanUseWSkill() && PlayerManager.Instance != null && !_stateMachine.Player.IsAwakened)
                //{
                //    _stateMachine.ChangeState(_stateMachine.WSkillState);
                //    return;
                //}
                if (jump && _stateMachine.JumpsRemaining > 0)
                {
                    _stateMachine.ChangeState(_stateMachine.JumpState);
                    return;
                }
                if (dash && _stateMachine.CanDash())
                {
                    _stateMachine.DashPressed = true;
                    _stateMachine.ChangeState(_stateMachine.DashState);
                    return;
                }
#if UNITY_2022_3_OR_NEWER

                if (_stateMachine.Player.IsGrounded() && _stateMachine.Player.Rb.linearVelocity.y <= 1f)
                {
                    if (_stateMachine.MovementInput == Vector2.zero)
                        _stateMachine.ChangeState(_stateMachine.IdleState);
                    else
                        _stateMachine.ChangeState(_stateMachine.WalkState);
                }
                else if (_stateMachine.Player.Rb.linearVelocity.y <= 0f)
                {
                    StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                    StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
                }

                //if (_stateMachine.Player.IsGrounded() && _stateMachine.Player.Rb.linearVelocity.y <= 0f)
                //{
                //    StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
                //    if (_stateMachine.MovementInput == Vector2.zero)
                //        _stateMachine.ChangeState(_stateMachine.IdleState);
                //    else
                //        _stateMachine.ChangeState(_stateMachine.WalkState);
                //}
                //else if (_stateMachine.Player.Rb.linearVelocity.y <= 0f)
                //{
                //    if (!_stateMachine.Player.IsGrounded())
                //    {
                //        StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                //        StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
                //    }
                //    else
                //    {
                //        StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
                //        if (_stateMachine.MovementInput == Vector2.zero)
                //            _stateMachine.ChangeState(_stateMachine.IdleState);
                //        else
                //            _stateMachine.ChangeState(_stateMachine.WalkState);
                //    }
                //}
#else
            if (_stateMachine.Player.Rb.velocity.y <= 0f)
            {
                StopAnimation(_stateMachine.Player.AnimationData.JumpParameterHash);
                StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
            }
            if (_stateMachine.Player.Rb.velocity.y <= 0f && _stateMachine.Player.IsGrounded())
            {
                if (_stateMachine.MovementInput == Vector2.zero)
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                else
                    _stateMachine.ChangeState(_stateMachine.WalkState);
            }
#endif
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
