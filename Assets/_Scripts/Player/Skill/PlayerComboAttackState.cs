using UnityEngine;
using Game.Monster;
using System.Collections.Generic;

namespace Game.Player
{
    public class PlayerComboAttackState : PlayerBaseState
    {
        private AttackInfoData _attackInfoData;
        private float _timer;
        private bool _force;
        private bool _damage;
        private List<IDamageable> _hitTargets;

        public PlayerComboAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.IsAttacking = true;
            _stateMachine.MovementSpeedModifier = 0f;
            _stateMachine.Player.Rb.linearVelocity = new Vector2(0, _stateMachine.Player.Rb.linearVelocity.y);

            int comboIndex = _stateMachine.ComboIndex;
            _attackInfoData = _stateMachine.Player.Data.ComboAttackData.GetAttackInfo(comboIndex);

            if (_attackInfoData == null)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }

            _stateMachine.Player.ForceReceiver.AddImpulse(new Vector2(_stateMachine.FacingSign * _attackInfoData.Force, 0));

            _timer = _attackInfoData.AttackDuration;

            _stateMachine.Player.Animator.SetInteger(_stateMachine.Player.AnimationData.ComboParameterHash, comboIndex);
            StartAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);

            _stateMachine.ContinueCombo = false;
            _force = false;
            _damage = false;
            _hitTargets = new List<IDamageable>();
        }

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            _stateMachine.IsAttacking = false;
        }

        public override void PhysicsUpdate() { }


        public override void Update()
        {
            _timer -= Time.deltaTime;

            //대시캔슬
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

            float comboStart = _attackInfoData.AttackDuration * _attackInfoData.ComboTime;
            float timePassed = _attackInfoData.AttackDuration - _timer;

            if (timePassed >= comboStart)
            {
                if (kb != null && kb.aKey.wasPressedThisFrame)
                {
                    _stateMachine.ContinueCombo = true;
                    Debug.Log("콤보");
                }
            }

            if (!_force && timePassed >= _attackInfoData.ForceTime)
            {
                _force = true;
                _stateMachine.Player.Rb.linearVelocity = new Vector2(0, _stateMachine.Player.Rb.linearVelocity.y);
            }

            if (!_damage && timePassed >= _attackInfoData.HitTiming)
            {
                _damage = true;
                TryDealDamage();
            }

            if (_timer <= 0f)
            {
                if (_stateMachine.ContinueCombo && _attackInfoData.ComboStateIndex != -1)
                {
                    _stateMachine.ComboIndex = _attackInfoData.ComboStateIndex;
                    _stateMachine.ChangeState(_stateMachine.ComboAttackState);
                }
                else
                {
                    _stateMachine.ComboIndex = 0;
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                }
            }
        }

        private void TryDealDamage()
        {
            var d = _stateMachine.Player.Data.CombatData;
            float r = d.AttackRange;
            var pos = (Vector2)_stateMachine.Player.transform.position + new Vector2(_stateMachine.FacingSign * r * 0.5f, 0f);
            var cols = Physics2D.OverlapCircleAll(pos, r);

            float baseDmg = d.AttackPower + d.ExtraDamage;
            float chance = Mathf.Max(0f, d.CriticalChance) * 0.01f;
            bool isCrit = Random.value < chance;
            float mult;

            if (isCrit)
                mult = 1f + Mathf.Max(0f, d.CriticalDamage) * 0.01f;
            else
                mult = 1f;

            int damage = Mathf.RoundToInt(baseDmg * mult * _attackInfoData.DamageSet);

            foreach (var col in cols)
            {
                if (col.transform.IsChildOf(_stateMachine.Player.transform)) continue;

                var target = col.GetComponentInParent<IDamageable>();
                if (target != null && !_hitTargets.Contains(target))
                {
                    _hitTargets.Add(target);
                    target.TakeDamage(damage);
                    _stateMachine.Player.MarkLastHitCritical(isCrit);
                    if (isCrit) Debug.Log("Critical!");
                }
            }
        }
    }
}