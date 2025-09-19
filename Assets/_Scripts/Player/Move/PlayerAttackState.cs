//using UnityEngine;
//using Game.Monster;

//namespace Game.Player
//{
//    public class PlayerAttackState : PlayerBaseState
//    {
//        float _timer;
//        bool _hasHit;

//        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

//        public override void Enter()
//        {
//            _stateMachine.IsAttacking = true;
//            _stateMachine.MovementSpeedModifier = 0f;
//            _timer = _stateMachine.Player.Data.CombatData.AttackDuration;
//            StartAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);

//            var v = _stateMachine.Player.Rb.linearVelocity;
//            _stateMachine.Player.Rb.linearVelocity = new Vector2(0f, v.y);

//            _hasHit = false;
//            DoHit();
//        }

//        public override void Exit()
//        {
//            _stateMachine.IsAttacking = false;
//            _stateMachine.MovementSpeedModifier = 1f;
//            StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
//        }

//        public override void Update()
//        {
//            _timer -= Time.deltaTime;
//            if (_timer <= 0f)
//            {
//                if (_stateMachine.Player.IsGrounded())
//                {
//                    if (_stateMachine.MovementInput == Vector2.zero)
//                        _stateMachine.ChangeState(_stateMachine.IdleState);
//                    else
//                        _stateMachine.ChangeState(_stateMachine.WalkState);
//                }
//                else
//                {
//                    _stateMachine.ChangeState(_stateMachine.AirState);
//                }
//            }
//        }

//        void DoHit()
//        {
//            if (_hasHit) return;
//            _hasHit = true;

//            var d = _stateMachine.Player.Data.CombatData;
//            float r = d.AttackRange;
//            var pos = (Vector2)_stateMachine.Player.transform.position
//                      + new Vector2(_stateMachine.FacingSign * r * 0.5f, 0f);

//            var cols = Physics2D.OverlapCircleAll(pos, r);

//            int hitCount = 0;
//            var visited = new System.Collections.Generic.HashSet<object>();

//            float baseDmg = d.AttackPower + d.ExtraDamage;
//            float chance = Mathf.Max(0f, d.CriticalChancePercent) * 0.01f;
//            bool isCrit = Random.value < chance;
//            float mult = isCrit ? (1f + Mathf.Max(0f, d.CriticalDamagePercent) * 0.01f) : 1f;
//            int damage = Mathf.RoundToInt(baseDmg * mult);

//            for (int i = 0; i < cols.Length; i++)
//            {
//                if (cols[i].transform.IsChildOf(_stateMachine.Player.transform)) continue;

//                var target = cols[i].GetComponentInParent<IDamageable>();
//                if (target != null && visited.Add(target))
//                {
//                    target.TakeDamage(damage);
//                    hitCount++;
//                }
//            }

//            _stateMachine.Player.MarkLastHitCritical(isCrit);
//            if (hitCount > 0)
//            {
//                if (isCrit) Debug.Log("Critical");
//                _stateMachine.Player.ReportNormalAttackHit();
//            }
//        }

//        public override void PhysicsUpdate() { }
//    }
//}
