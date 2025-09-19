using Game.Monster;
using System;
using UnityEngine;

namespace GameSystem
{
    public class PlayerCharacter : MonoBehaviour, IDamageable
    {
        public Rigidbody2D Rb { get; private set; }
        public Animator Animator { get; private set; }

        [SerializeField] private AnimationData AnimationDataSerialized;
        [SerializeField] private PlayerData DataSerialized;
        [SerializeField] private Transform GroundCheck;
        [SerializeField] private float GroundRadius = 0.15f;
        [SerializeField] private LayerMask GroundLayer;
        [SerializeField] private ForceReceiver Force;

        public AnimationData AnimationData => AnimationDataSerialized;
        public PlayerData Data => DataSerialized;
        public ForceReceiver ForceReceiver => Force;

        private PlayerStateMachine _machine;

        public bool Invincible { get; private set; }
        public void SetInvincible(bool on) { Invincible = on; }

        [Header("Runtime (Read Only)")]
        [SerializeField] private bool showRuntime = true;
        [SerializeField] private bool runtimeIsDashing;
        [SerializeField] private float runtimeSpeedModifier;
        [SerializeField] private float runtimeMoveSpeed;
        [SerializeField] private Vector2 runtimeVelocity;
        [SerializeField] private float currentHP;

        [Header("Corruption")]
        [SerializeField] private int corruptionGauge;
        [SerializeField] private int maxCorruptionGauge = 200;
        [SerializeField] private int corruptionGainPerHit = 5;
        public int CorruptionGauge => corruptionGauge;

        [Header("Combat Debug")]
        [SerializeField] private bool lastHitCritical;
        public bool LastHitCritical => lastHitCritical;
        public void MarkLastHitCritical(bool on) { lastHitCritical = on; }

        public float CurrentHP => currentHP;
        public bool IsDead => currentHP <= 0f;

        public event Action<float, float> HpEvent;

        public bool IsGrounded()
        {
            if (!GroundCheck) return false;
            return Physics2D.OverlapCircle(GroundCheck.position, GroundRadius, GroundLayer);
        }

        public void ReportNormalAttackHit()
        {
            if (corruptionGauge >= maxCorruptionGauge) return;
            corruptionGauge = Mathf.Min(maxCorruptionGauge, corruptionGauge + Mathf.Max(0, corruptionGainPerHit));
        }

        public void ResetCorruptionGauge() { corruptionGauge = 0; }

        public void TakeDamage(float amount)
        {
            if (Invincible || IsDead) return;
            currentHP = Mathf.Max(0f, currentHP - Mathf.Max(0f, amount));
            if (currentHP <= 0f) Die(); else EnterHurtByFacing();
        }

        public void TakeDamage(int damage)
        {
            if (Invincible || IsDead) return;
            currentHP = Mathf.Max(0f, currentHP - Mathf.Max(0, damage));
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
            if (currentHP <= 0f) Die(); else EnterHurtByFacing();
        }

        void EnterHurtByFacing()
        {
            float dir = -Mathf.Sign(transform.localScale.x);
            var hd = Data.HurtData;
            var kb = new Vector2(dir * hd.KnockbackX, hd.KnockbackY);
            ForceReceiver.Knockback(kb);
            _machine.ChangeState(_machine.HurtState);
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            currentHP = Mathf.Min(Data.Stats.MaxHP, currentHP + Mathf.Max(0f, amount));
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
        }

        public void Die()
        {
            currentHP = 0f;
            ResetCorruptionGauge();
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
            _machine.ChangeState(_machine.DieState);
        }

        public void SetLayerCollisionIgnore(LayerMask mask, bool ignore)
        {
            int playerLayer = gameObject.layer;
            int m = mask.value;
            for (int i = 0; i < 32; i++)
            {
                if ((m & (1 << i)) != 0)
                    Physics2D.IgnoreLayerCollision(playerLayer, i, ignore);
            }
        }

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            if (!Force) Force = GetComponent<ForceReceiver>();
            currentHP = Data.Stats.MaxHP;

            _machine = new PlayerStateMachine(this);
            _machine.ChangeState(_machine.IdleState);
        }

        private void Update()
        {
            _machine.Tick();
            UpdateRuntimeDebug();
        }

        private void FixedUpdate()
        {
            _machine.FixedTick();
            UpdateRuntimeDebug();
        }

        void UpdateRuntimeDebug()
        {
            if (!showRuntime) return;
            runtimeIsDashing = _machine.IsDashing;
            runtimeSpeedModifier = _machine.MovementSpeedModifier;
            runtimeMoveSpeed = _machine.MovementSpeed * _machine.MovementSpeedModifier;
#if UNITY_2022_3_OR_NEWER
            runtimeVelocity = Rb.linearVelocity;
#else
            runtimeVelocity = Rb.velocity;
#endif
        }
    }
}
