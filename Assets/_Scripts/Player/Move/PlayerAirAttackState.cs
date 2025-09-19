using UnityEngine;
using Game.Monster;
using System.Collections.Generic;

namespace Game.Player
{
    public class PlayerAirAttackState : PlayerBaseState
    {
        private AttackInfoData _attackInfoData;
        private float _timer;
        private bool _hasDealtDamage;
        private List<IDamageable> _hitTargets;

        public PlayerAirAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.IsAttacking = true;
            _stateMachine.MovementSpeedModifier = 0f;

            _attackInfoData = _stateMachine.Player.Data.ComboAttackData.GetAttackInfo(2);

            var rb = _stateMachine.Player.Rb;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);

            _timer = _attackInfoData.AttackDuration;
            _stateMachine.Player.Animator.SetInteger(_stateMachine.Player.AnimationData.ComboParameterHash, 2);
            StartAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);

            _hasDealtDamage = false;
            _hitTargets = new List<IDamageable>();
        }

        public override void Exit()
        {
            Debug.Log("공중공격끝");
            _stateMachine.ComboIndex = 0;
            StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            _stateMachine.IsAttacking = false;
        }

        public override void PhysicsUpdate() { }

        public override void Update()
        {
            _timer -= Time.deltaTime;

            float timePassed = _attackInfoData.AttackDuration - _timer;
            if (!_hasDealtDamage && timePassed >= _attackInfoData.HitTiming)
            {
                _hasDealtDamage = true;
                TryDealDamage();
            }

            if (_timer <= 0f)
            {
                _stateMachine.ChangeState(_stateMachine.AirState);
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
            float mult = isCrit ? (1f + Mathf.Max(0f, d.CriticalDamage) * 0.01f) : 1f;
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