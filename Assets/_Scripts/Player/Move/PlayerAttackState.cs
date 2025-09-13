// PlayerAttackState.cs
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class PlayerAttackState : PlayerGroundedState
    {
        float _timer;
        HashSet<Collider2D> _hit = new HashSet<Collider2D>();

        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.IsAttacking = true;
            _stateMachine.MovementSpeedModifier = 0f;
            _timer = _stateMachine.Player.Data.CombatData.AttackDuration;
            _hit.Clear();
            StartAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Exit()
        {
            _stateMachine.IsAttacking = false;
            StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            _stateMachine.MovementSpeedModifier = 1f;
        }

        public override void Update()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            bool dash = kb != null && kb.sKey.wasPressedThisFrame;
#else
            bool dash = Input.GetKeyDown(KeyCode.S);
#endif
            if (dash && _stateMachine.CanDash())
            {
                _stateMachine.ChangeState(_stateMachine.DashState);
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            Vector2 origin = _stateMachine.Player.transform.position;
            float r = _stateMachine.Player.Data.CombatData.AttackRange;
            Vector2 facing = _stateMachine.FacingSign > 0 ? Vector2.right : Vector2.left;

            var cols = Physics2D.OverlapCircleAll(origin, r);
            float damage = _stateMachine.Player.Data.CombatData.AttackPower;

            for (int i = 0; i < cols.Length; i++)
            {
                var c = cols[i];
                if (c.attachedRigidbody == _stateMachine.Player.Rb) continue;

                Vector2 to = (Vector2)c.bounds.center - origin;
                if (to.sqrMagnitude <= 0.0001f) continue;
                if (Vector2.Dot(to.normalized, facing) <= 0f) continue;

                if (_hit.Add(c))
                {
                    c.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
