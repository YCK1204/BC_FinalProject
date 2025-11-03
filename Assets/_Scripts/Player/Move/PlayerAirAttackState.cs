using Destructible2D;
using Game.Monster;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAirAttackState : PlayerBaseState
    {
        private AttackInfoData _attackInfoData;
        private float _timer;
        private List<IDamageable> _hitTargets;

        private float _damageInterval = 0.1f;
        private float _damageTimer;

        public PlayerAirAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.Player.UsingAttack_Start();
            _stateMachine.IsAttacking = true;
            _stateMachine.MovementSpeedModifier = 0f;

            _attackInfoData = _stateMachine.Player.Data.ComboAttackData.GetAttackInfo(2);

            var rb = _stateMachine.Player.Rb;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.down * 30f, ForceMode2D.Impulse);

            _timer = _attackInfoData.AttackDuration;
            _stateMachine.Player.Animator.SetInteger(_stateMachine.Player.AnimationData.ComboParameterHash, 2);
            StartAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            _stateMachine.Player.Animator.Play("Attack_3", 0, 0f);

            Manager.Audio.Play(AudioKey.Player.Skill.P_SKILL_SWORD3, _stateMachine.Player.transform);
            _hitTargets = new List<IDamageable>();

            _damageTimer = 0f;
        }

        public override void Exit()
        {
            _stateMachine.Player.UsingAttackt_End();
            _stateMachine.ComboIndex = 0;
            StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            _stateMachine.IsAttacking = false;
        }

        public override void PhysicsUpdate() { }

        public override void Update()
        {
            _timer -= Time.deltaTime;

            _damageTimer -= Time.deltaTime;
            if (_damageTimer <= 0f)
            {
                TryDealDamage();
                _damageTimer = _damageInterval;
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

            float baseDmg = (d.AttackPower + d.ExtraDamage) * (1 + d.AttackPowerPercent);
            float chance = Mathf.Max(0f, d.CriticalChance) * 0.01f;

            bool hitted = false;
            foreach (var col in cols)
            {
                if (col.transform.IsChildOf(_stateMachine.Player.transform)) continue;

                if (col.gameObject.layer == LayerMask.NameToLayer("Destructible"))
                {
                    var d2dDmg = col.gameObject.transform.parent.GetComponent<D2dDamage>();
                    d2dDmg.Damage++;
                    continue;
                }

                var target = col.GetComponentInParent<IDamageable>();
                if (target != null && !_hitTargets.Contains(target))
                {
                    hitted = true;
                    _hitTargets.Add(target);

                    bool isCrit = Random.value < chance;
                    float mult;

                    if (isCrit)
                        mult = Mathf.Max(0f, d.CriticalDamage) * 0.01f;
                    else
                        mult = 1f;

                    int damage = Mathf.RoundToInt(baseDmg * mult * _attackInfoData.DamageSet);

                    target.TakeDamage(damage);
                    _stateMachine.Player.MarkLastHitCritical(isCrit);
                    if (isCrit) Debug.Log("Critical!!" + target.ToString());

                }
            }
            if (hitted)
                _stateMachine.Player.AttackHit();
        }
    }
}