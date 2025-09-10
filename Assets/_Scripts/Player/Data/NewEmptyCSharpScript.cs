using UnityEngine;

namespace GameSystem
{
    public partial class Player : MonoBehaviour
    {
        public Rigidbody2D Rb { get; private set; }
        public Animator Animator { get; private set; }
        public AnimationData AnimationData => _animationData;
        public PlayerData Data => _data;
        public ForceReceiver ForceReceiver => _force;

        [SerializeField] private AnimationData _animationData;
        [SerializeField] private PlayerData _data;
        [SerializeField] private Transform GroundCheck;
        [SerializeField] private float GroundRadius = 0.15f;
        [SerializeField] private LayerMask GroundLayer;
        [SerializeField] private ForceReceiver _force;

        private void Reset()
        {
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            _force = GetComponent<ForceReceiver>();
        }

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
        }

        public bool IsGrounded()
        {
            if (!GroundCheck) return false;
            return Physics2D.OverlapCircle(GroundCheck.position, GroundRadius, GroundLayer);
        }
    }
}
