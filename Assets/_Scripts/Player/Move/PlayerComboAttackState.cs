using Destructible2D;
using DG.Tweening;
using Game.Monster;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerComboAttackState : PlayerBaseState
    {
        private AttackInfoData _attackInfoData;
        private PlayerCombatData _attackcombatData;

        private float _timer;
        private bool _force;
        private bool _damage;
        private List<IDamageable> _hitTargets;


        public PlayerComboAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.Player.UsingAttack_Start();

            _stateMachine.IsAttacking = true;
            _stateMachine.MovementSpeedModifier = 0f;
            _stateMachine.Player.Rb.linearVelocity = new Vector2(0, _stateMachine.Player.Rb.linearVelocity.y);

            int comboIndex = _stateMachine.ComboIndex;
            _attackInfoData = _stateMachine.Player.Data.ComboAttackData.GetAttackInfo(comboIndex);
            _attackcombatData = _stateMachine.Player.Data.CombatData;

            if (_attackInfoData == null)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }

            _stateMachine.Player.Animator.speed = _attackcombatData.AttackSpeed;

            _stateMachine.Player.ForceReceiver.AddImpulse(new Vector2(_stateMachine.FacingSign * _attackInfoData.Force, 0));

            _timer = _attackInfoData.AttackDuration / _attackcombatData.AttackSpeed;

            _stateMachine.Player.Animator.SetInteger(_stateMachine.Player.AnimationData.ComboParameterHash, comboIndex);
            StartAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);

            _stateMachine.Player.Animator.Play(_attackInfoData.AnimName, 0, 0f);

            _stateMachine.ContinueCombo = false;
            _force = false;
            _damage = false;
            _hitTargets = new List<IDamageable>();
        }

        public override void Exit()
        {
            _stateMachine.Player.UsingAttackt_End();

            StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            _stateMachine.IsAttacking = false;

            _stateMachine.Player.Animator.speed = 1f;
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

            float timePass = (_attackInfoData.AttackDuration / _attackcombatData.AttackSpeed) - _timer;

            float comboStartTime = _attackInfoData.AttackDuration * _attackInfoData.ComboTime / _attackcombatData.AttackSpeed;
            float forceTime = _attackInfoData.ForceTime / _attackcombatData.AttackSpeed;
            float hitTime = _attackInfoData.HitTiming / _attackcombatData.AttackSpeed;


            if (timePass >= comboStartTime)
            {
                if (kb != null && kb.aKey.wasPressedThisFrame)
                {
                    _stateMachine.ContinueCombo = true;
                    //Debug.Log("콤보");
                }
            }

            if (!_force && timePass >= forceTime)
            {
                _force = true;
                if (_stateMachine.Player.IsGroundInFront(0.5f))
                {
                    _stateMachine.Player.Rb.linearVelocity = new Vector2(0, _stateMachine.Player.Rb.linearVelocity.y) * _attackcombatData.AttackSpeed;
                }
            }

            if (!_stateMachine.Player.IsGroundInFront(0.5f))
            {
                //Debug.Log("정지");

                var rb = _stateMachine.Player.Rb;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            if (!_damage && timePass >= hitTime)
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
            bool hitted = false;
            foreach (var col in cols)
            {
                if (col == null) continue;
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
                        mult = 1f + Mathf.Max(0f, d.CriticalDamage) * 0.01f;
                    else
                        mult = 1f;

                    int damage = Mathf.RoundToInt(baseDmg * mult * _attackInfoData.DamageSet);

                    _stateMachine.Player.GainAwakeningGauge();

                    target.TakeDamage(damage);
                    _stateMachine.Player.MarkLastHitCritical(isCrit);
                    if (isCrit) Debug.Log("Critical!!" + target.ToString());

                    //Rigidbody2D targetRb = col.attachedRigidbody;
                    //if (targetRb != null)
                    //{
                    //    float power = _attackInfoData.KnockbackPower;
                    //    Vector2 knockDir = new Vector2(_stateMachine.FacingSign, 0f).normalized;

                    //    targetRb.linearVelocity = knockDir * power;
                    //}
                }
            }

            if (hitted)
                _stateMachine.Player.AttackHit();
        }
    }
}