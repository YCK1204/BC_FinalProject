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

        // ===== Runtime Debug (Inspector에서 확인용) =====
        [Header("Runtime (Read Only)")]
        [SerializeField] private bool showRuntime = true;
        [SerializeField] private bool runtimeIsDashing;
        [SerializeField] private float runtimeSpeedModifier;
        [SerializeField] private float runtimeMoveSpeed;   // Base * Modifier
        [SerializeField] private Vector2 runtimeVelocity;

        public bool IsGrounded()
        {
            if (!GroundCheck) return false;
            return Physics2D.OverlapCircle(GroundCheck.position, GroundRadius, GroundLayer);
        }

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            if (!Force) Force = GetComponent<ForceReceiver>();

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
