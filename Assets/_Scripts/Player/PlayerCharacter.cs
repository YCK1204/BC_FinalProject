using UnityEngine;

namespace GameSystem
{
    public class PlayerCharacter : MonoBehaviour
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

        public float CurrentHP => currentHP;
        public bool IsDead => currentHP <= 0f;

        public bool IsGrounded()
        {
            if (!GroundCheck) return false;
            return Physics2D.OverlapCircle(GroundCheck.position, GroundRadius, GroundLayer);
        }

        public void TakeDamage(float amount)
        {
            if (Invincible || IsDead) return;
            currentHP = Mathf.Max(0f, currentHP - Mathf.Max(0f, amount));
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            currentHP = Mathf.Min(Data.Stats.MaxHP, currentHP + Mathf.Max(0f, amount));
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
            _machine?.Tick();
            UpdateRuntimeDebug();
        }

        private void FixedUpdate()
        {
            _machine?.FixedTick();
            UpdateRuntimeDebug();
        }

        void UpdateRuntimeDebug()
        {
            if (!showRuntime || _machine == null) return;
            runtimeIsDashing = _machine.IsDashing;
            runtimeSpeedModifier = _machine.MovementSpeedModifier;
            runtimeMoveSpeed = _machine.MovementSpeed * _machine.MovementSpeedModifier;
#if UNITY_2022_3_OR_NEWER
            runtimeVelocity = Rb.linearVelocity;
#else
            runtimeVelocity      = Rb.velocity;
#endif
        }
    }
}
