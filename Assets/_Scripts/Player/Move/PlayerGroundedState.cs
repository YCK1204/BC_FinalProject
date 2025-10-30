using UnityEngine;

namespace Game.Player
{
    public abstract class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.JumpsRemaining = _stateMachine.MaxJumps;
        }

        public override void Update()
        {
            if (_stateMachine.InputActive)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                bool jump = kb != null && kb.spaceKey.wasPressedThisFrame;
                bool dash = kb != null && kb.sKey.wasPressedThisFrame;
                bool attack = kb != null && kb.aKey.wasPressedThisFrame;
                bool qSkill = kb != null && kb.qKey.wasPressedThisFrame;
                bool wSkill = kb != null && kb.wKey.wasPressedThisFrame;
                bool awaken = kb != null && kb.dKey.wasPressedThisFrame;

                _stateMachine.DashPressed = dash;

                if (awaken && _stateMachine.Player.CanAwaken() && PlayerManager.Instance != null)
                {
                    _stateMachine.ChangeState(_stateMachine.AwakeningState);
                    return;
                }
                if (qSkill && _stateMachine.CanUseQSkill() && PlayerManager.Instance != null && !_stateMachine.Player.IsAwakened)
                {
                    _stateMachine.ChangeState(_stateMachine.QSkillState);
                    return;
                }
                if (wSkill && _stateMachine.CanUseWSkill() && PlayerManager.Instance != null && !_stateMachine.Player.IsAwakened)
                {
                    _stateMachine.ChangeState(_stateMachine.WSkillState);
                    return;
                }

                if (!_stateMachine.IsAttacking && _stateMachine.Player.IsGrounded())
                {
                    if (attack || _stateMachine.AttackInputBuffered)
                    {
                        if (_stateMachine.AttackInputBuffered)
                        {
                            _stateMachine.ComboIndex = 0;
                            _stateMachine.AttackInputBuffered = false;
                        }
                        else
                        {
                            _stateMachine.ComboIndex = 0;
                        }
                        _stateMachine.ChangeState(_stateMachine.ComboAttackState);
                        return;
                    }
                }

                if (jump && _stateMachine.Player.IsGrounded() && _stateMachine.JumpsRemaining > 0)
                {
                    _stateMachine.ChangeState(_stateMachine.JumpState);
                    return;
                }
                if (dash && _stateMachine.CanDash())
                {
                    _stateMachine.ChangeState(_stateMachine.DashState);
                    return;
                }
            }
        }

        public override void PhysicsUpdate() { base.PhysicsUpdate(); }
    }
}