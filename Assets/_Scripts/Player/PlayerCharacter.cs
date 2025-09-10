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
        }

        private void FixedUpdate()
        {
            _machine?.FixedTick();
        }
    }
}
