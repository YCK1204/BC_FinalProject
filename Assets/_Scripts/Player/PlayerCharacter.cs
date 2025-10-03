using DG.Tweening;
using Game.Monster;
using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

namespace Game.Player
{
    public class PlayerCharacter : MonoBehaviour, IDamageable
    {
        public Rigidbody2D Rb { get; private set; }
        public Animator Animator { get; private set; }
        public static PlayerCharacter Instance { get; private set; }

        public bool OnTrait;

        public SpriteRenderer SpriteRenderer;

        public MaterialInitializer PlayerMaterial;

        [SerializeField] private CameraShake camShake;

        [SerializeField] private AnimationData AnimationDataSerialized;
        [SerializeField] private PlayerData DataSerialized;
        [SerializeField] private Transform GroundCheck;
        [SerializeField] private float GroundRadius = 0.15f;
        [SerializeField] private LayerMask GroundLayer;
        [SerializeField] private ForceReceiver Force;

        public AnimationData AnimationData => AnimationDataSerialized;
        public PlayerData Data => DataSerialized;

        private PlayerData _originalData;
        public ForceReceiver ForceReceiver => Force;

        private PlayerStateMachine _machine;

        private CinemachineImpulseSource _impulseSource;

        [SerializeField] private RuntimeAnimatorController normalAnimator;
        [SerializeField] private RuntimeAnimatorController awakenedAnimator;

        
        public bool Invincible { get; private set; }
        public void SetInvincible(bool on) { Invincible = on; }

        [Header("Runtime (Read Only)")]
        [SerializeField] private bool showRuntime = true;
        [SerializeField] private bool runtimeIsDashing;
        [SerializeField] private float runtimeSpeedModifier;
        [SerializeField] private float runtimeMoveSpeed;
        [SerializeField] private Vector2 runtimeVelocity;
        [SerializeField] private float currentHP;

        [Header("Awakening")]
        [SerializeField] private float currentAwakening;
        public float CurrentAwakening => currentAwakening;
        public bool IsAwakened { get; private set; }

        [SerializeField] private GameObject awakeningEffect;

        [Header("Combat Debug")]
        [SerializeField] private bool lastHitCritical;
        public bool LastHitCritical => lastHitCritical;
        public void MarkLastHitCritical(bool on) { lastHitCritical = on; }

        public float CurrentHP
        {
            get
            {
                return currentHP; 
            }
            set
            {
                if (currentHP <= 0f) return;
                currentHP = Mathf.Max(0, value);
                HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
                if (currentHP <= 0f)
                    Die();
            }
        }

        public bool IsDead => currentHP <= 0f;
        [SerializeField] private DeadControl deadControl;


        public event Action<float, float> HpEvent;
        public event Action<float, float> AwakeningEvent;



        private void Awake()
        {
            Instance = this;
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponentInChildren<Animator>();
            if (!Force) Force = GetComponent<ForceReceiver>();
            _originalData = DataSerialized.Clone();

            currentHP = Data.Stats.MaxHP;

            _machine = new PlayerStateMachine(this);
            _machine.ChangeState(_machine.IdleState);

            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        #region Callback
        public event Action OnKill;
        public event Action OnUsingSkill;
        public event Action OnUsingAttackStart;
        public event Action OnUsingAttackEnd;
        public event Action OnAttackHit;
        public event Action OnStartRound;
        public event Action OnDashEnd;

        public void Kill()
        {
            OnKill?.Invoke();
        }
        public void UsingSkill()
        {
            OnUsingSkill?.Invoke();
        }
        public void UsingAttack_Start()
        {
            OnUsingAttackStart?.Invoke();
        }
        public void UsingAttackt_End()
        {
            OnUsingAttackEnd?.Invoke();
        }
        public void AttackHit()
        {
            OnAttackHit?.Invoke();
        }
        public void StartRound()
        {
            OnStartRound?.Invoke();
        }
        public void DashEnd()
        {
            OnDashEnd?.Invoke();
        }

        #endregion

        #region Player

        public bool IsGrounded()
        {
            if (!GroundCheck)
            {
                return false;
            }

            Vector2 center = GroundCheck.position;
            Vector2 left = new Vector2(center.x - 0.21f, center.y);
            Vector2 right = new Vector2(center.x + 0.21f, center.y);

            RaycastHit2D hitCenter = Physics2D.Raycast(center, Vector2.down, GroundRadius, GroundLayer);
            RaycastHit2D hitLeft = Physics2D.Raycast(left, Vector2.down, GroundRadius, GroundLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(right, Vector2.down, GroundRadius, GroundLayer);

            // 레이 표시
            //Debug.DrawRay(center, Vector2.down * GroundRadius, hitCenter.collider != null ? Color.green : Color.red);
            //Debug.DrawRay(left, Vector2.down * GroundRadius, hitLeft.collider != null ? Color.green : Color.red);
            //Debug.DrawRay(right, Vector2.down * GroundRadius, hitRight.collider != null ? Color.green : Color.red);

            return hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;
        }

        public bool IsGroundInFront(float forward)
        {
            if (!GroundCheck)
            {
                return false;
            }

            float facingDirection = Mathf.Sign(transform.localScale.x);
            Vector2 origin = (Vector2)GroundCheck.position + new Vector2(facingDirection * 0.5f, 0f);
            Vector2 direction = new Vector2(facingDirection, -1f).normalized;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, forward, GroundLayer);

            Debug.DrawRay(origin, direction * forward, hit.collider != null ? Color.green : Color.red);

            return hit.collider != null;
        }



        public void TakeDamage(float amount, GameObject attacker = null)
        {
            if (Invincible || IsDead) return;
            currentHP = Mathf.Max(0f, currentHP - Mathf.Max(0f, amount));
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);

            Debug.Log($"피해량체크- {amount} 남은체력- {currentHP}");

            camShake.Shake(1f, 1f, 0.2f);
            _impulseSource.GenerateImpulse();
            if (currentHP <= 0f) Die(); else StartCoroutine(HitColor());
            ;
        }

        //public void TakeDamage(int damage)
        //{
        //    if (Invincible || IsDead) return;
        //    currentHP = Mathf.Max(0f, currentHP - Mathf.Max(0, damage));
        //    HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);

        //    Debug.Log($"피해량체크- {damage} 남은체력- {currentHP}");

        //    camShake.Shake(1f, 1f, 0.2f);
        //    _impulseSource.GenerateImpulse();
        //    if (currentHP <= 0f) Die(); else StartCoroutine(HitColor());
        //    ;
        //}

        private IEnumerator HitColor()
        {

            float a = 1f;
            float b = 0.1f;
            _machine.ChangeState(_machine.HurtState);

            Color color = SpriteRenderer.color;
            SetInvincible(true);

            float time = 0f;
            while (time < a)
            {
                SpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                yield return new WaitForSeconds(b);
                time += b;

                SpriteRenderer.color = color;
                yield return new WaitForSeconds(b);
                time += b;
            }

            SpriteRenderer.color = color;
            SetInvincible(false);
        }

        void EnterHurtByFacing()
        {
            //float dir = -Mathf.Sign(transform.localScale.x);
            //var hd = Data.HurtData;
            //var kb = new Vector2(dir * hd.KnockbackX, hd.KnockbackY);
            //ForceReceiver.Knockback(kb);
            //_machine.ChangeState(_machine.HurtState);
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
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
            _machine.ChangeState(_machine.DieState);

            gameObject.layer = LayerMask.NameToLayer("Default");

            deadControl.DieSet();
        }

        public void Resurrection()
        {
            DataSerialized = _originalData.Clone();

            OnKill = null;
            OnUsingSkill = null;
            OnUsingAttackStart = null;
            OnUsingAttackEnd = null;
            OnAttackHit = null;
            OnStartRound = null;
            OnDashEnd = null;

            Animator.runtimeAnimatorController = normalAnimator;

            currentHP = Data.Stats.MaxHP;
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
            _machine.ChangeState(_machine.IdleState);

            gameObject.layer = LayerMask.NameToLayer("Player");
            PlayerMaterial.SetDefaultMaterial();
        }

        public void GainAwakeningGauge()
        {
            if (IsAwakened || IsDead) return;
            var awakeningData = Data.awakening;
            currentAwakening = Mathf.Min(awakeningData.maxAwakeningGauge, currentAwakening + awakeningData.awakeningOnHit);

            AwakeningEvent?.Invoke(currentAwakening, awakeningData.maxAwakeningGauge);

            if (currentAwakening >= awakeningData.maxAwakeningGauge)
            {
                EnterAwakening();
            }
        }

        private void EnterAwakening()
        {
            if (IsAwakened) return;

            var awakeningData = Data.awakening;

            Debug.Log("각성!");
            IsAwakened = true;
            currentAwakening = awakeningData.maxAwakeningGauge;
            float totalDuration = Data.awakening.duration;
            Data.CombatData.AttackRange = 1.6f;

            Animator.runtimeAnimatorController = awakenedAnimator;

            awakeningEffect.SetActive(true);

            StartCoroutine(AwakeningTimer(totalDuration));
        }

        private IEnumerator AwakeningTimer(float duration)
        {
            float time = 0f;
            float start = currentAwakening;

            while (time < duration)
            {
                time += Time.deltaTime;
                currentAwakening = Mathf.Lerp(start, 0f, time / duration);

                AwakeningEvent?.Invoke(currentAwakening, Data.awakening.maxAwakeningGauge);

                yield return null;
            }

            currentAwakening = 0f;
            IsAwakened = false;
            Data.CombatData.AttackRange = 1.1f;

            Animator.runtimeAnimatorController = normalAnimator;

            awakeningEffect.SetActive(false);

            AwakeningEvent?.Invoke(currentAwakening, Data.awakening.maxAwakeningGauge);
            Debug.Log("각성종료");
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

        public void AutoMove(float a, Vector2 b)
        {
            StartCoroutine(AutoMoveCrt(a, b));
        }

        private IEnumerator AutoMoveCrt(float a, Vector2 b)
        {
            Animator.SetBool(AnimationData.WalkParameterHash, true);
            Animator.SetBool(AnimationData.IdleParameterHash, false);
            SetPlayerInput(false);

            float timeElapsed = 0f;
            while (timeElapsed < a)
            {
                _machine.MovementInput = b.normalized;

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            SetPlayerInput(true);
            _machine.MovementInput = Vector2.zero;
            Animator.SetBool(AnimationData.WalkParameterHash, false);
            Animator.SetBool(AnimationData.IdleParameterHash, true);
        }

        public void SetPlayerInput(bool isEnable)
        {
            _machine.InputActive = isEnable;

            if (!isEnable)
            {
                _machine.MovementInput = Vector2.zero;
            }
        }

        #endregion

        //private void OnDrawGizmosSelected()
        //{
        //    if (Data == null) return;

        //    float r = Data.CombatData.AttackRange;
        //    float facing = Mathf.Sign(transform.localScale.x);

        //    Vector2 pos = (Vector2)transform.position + new Vector2(facing * r * 0.5f, 0f);

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(pos, r);
        //}

        private void Update()
        {
            _machine.Tick();
            UpdateRuntimeDebug();

            var kb = UnityEngine.InputSystem.Keyboard.current;
            bool g = kb != null && kb.gKey.wasPressedThisFrame;
            if (g)
                Manager.Item.AddItem(this);

            if (kb != null && kb.rKey.wasPressedThisFrame)
            {
                Die();
            }
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
