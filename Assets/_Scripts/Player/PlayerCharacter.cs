using DG.Tweening;
using Game.Monster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Player
{
    public class PlayerCharacter : MonoBehaviour, IDamageable
    {
        public Inventory Inventory = new Inventory();
        public Rigidbody2D Rb { get; private set; }
        public Animator Animator { get; private set; }
        public static PlayerCharacter Instance { get; private set; }

        public bool OnUI;

        public List<InteractableController> Interactables = new List<InteractableController>();
        public SpriteRenderer SpriteRenderer;

        public MaterialInitializer PlayerMaterial;

        public CameraShake camShake;

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

        public PlayerStateMachine StateMachine { get; private set; }

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
        //각성게이지
        public float CurrentAwakening => currentAwakening;
        public bool IsAwakened { get; private set; }

        [SerializeField] private GameObject awakeningEffect;
        [SerializeField] private GameObject awakeningEffect_field;

        [SerializeField] private GameObject _ShadowSlashProjectilePrefab;

        [SerializeField] public string Awkanim;

        [Header("Combat Debug")]
        [SerializeField] private bool lastHitCritical;
        public bool LastHitCritical => lastHitCritical;
        public void MarkLastHitCritical(bool on) { lastHitCritical = on; }

        //현재 체력
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

        [field: SerializeField] public Skill ShadowSlashSkill { get; private set; }
        [field: SerializeField] public Skill DoubleStrikeSkill { get; private set; }



        private void Awake()
        {
            Instance = this;
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponentInChildren<Animator>();
            if (!Force) Force = GetComponent<ForceReceiver>();
            _originalData = DataSerialized.Clone();

            currentHP = Data.Stats.MaxHP;

            StateMachine = new PlayerStateMachine(this);
            StateMachine.ChangeState(StateMachine.IdleState);

            _impulseSource = GetComponent<CinemachineImpulseSource>();
            DoubleStrikeSkill = GetComponent<DoubleStrike>();
            ShadowSlashSkill = GetComponent<ShadowSlash>();

            ShadowSlashSkill?.Initialize(this);
            DoubleStrikeSkill?.Initialize(this);

            if (SceneManager.GetActiveScene().name != "Intro")
                gameObject.SetActive(false);
        }
        public void SetData(PlayerData data)
        {
            _originalData = data;
        }
        public string GetPlayerJsonData()
        {
            var mapData = MapManager.Instance.GetSaveData();
            var jsonData = new PlayerJsonData()
            {
                CurMap = mapData.CurrentMapIndex,
                ClearedMaps = mapData.ClearedMapIndices,
                Hp = currentHP,
                Awaken = currentAwakening,
                PlayTime = mapData.PlayTime,
            };

            return JsonConvert.SerializeObject(jsonData);
        }
        public void LoadPlayerFromJson(string jsonData)
        {
            var json = JObject.Parse(jsonData);
            currentHP = json.Value<float>("hp");
            currentAwakening = json.Value<float>("awaken");
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
            AwakeningEvent?.Invoke(currentAwakening, Data.awakening.MaxAwakeningGauge);
            Debug.Log("플레이어 데이터 로드 완료");
        }
        public void ApplyData(PlayerSaveData data)
        {
            this.currentHP = data.CurrentHP;
            this.currentAwakening = data.CurrentAwakening;

            HpEvent?.Invoke(this.currentHP, Data.Stats.MaxHP);
            AwakeningEvent?.Invoke(this.currentAwakening, Data.awakening.MaxAwakeningGauge);
            Debug.Log("데이터 저장 동기화");
        }

        #region Callback

        public event Action OnKill;
        public event Action OnUsingSkill;
        public event Action OnUsingAttackStart;
        public event Action OnUsingAttackEnd;
        public event Action OnAttackHit;
        public event Action OnStartRound;
        public event Action OnDashEnd;
        public event Action OnDied;

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

        public bool CanAwaken() => currentAwakening >= Data.awakening.MaxAwakeningGauge && !IsAwakened;

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



        public void TakeDamage(float amount, bool hitSfxMute = false)
        {
            if (Invincible || IsDead) return;
            currentHP = Mathf.Max(0f, currentHP - Mathf.Max(0f, amount));
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);

            Debug.Log($"피해량체크- {amount} 남은체력- {currentHP}");

            camShake.Shake(2f, 2f, 0.15f);
            _impulseSource.GenerateImpulse();
            if (currentHP <= 0f) Die(); else StartCoroutine(HitColor());
            ;
        }

        public void TakeDamage(int damage)
        {
            if (Invincible || IsDead) return;
            currentHP = Mathf.Max(0f, currentHP - Mathf.Max(0, damage));
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);

            Debug.Log($"피해량체크- {damage} 남은체력- {currentHP}");

            camShake.Shake(2f, 2f, 0.15f);
            _impulseSource.GenerateImpulse();
            if (currentHP <= 0f) Die(); else StartCoroutine(HitColor());
            ;
        }

        private IEnumerator HitColor()
        {

            float a = 1f;
            float b = 0.1f;
            StateMachine.ChangeState(StateMachine.HurtState);

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
            OnDied?.Invoke();

            currentHP = 0f;
            HpEvent?.Invoke(currentHP, Data.Stats.MaxHP);
            StateMachine.ChangeState(StateMachine.DieState);

            gameObject.layer = LayerMask.NameToLayer("Player_die");

            deadControl.DieSet();

            MapManager.Instance.SetTimerActive(false);
            PlayerManager.Instance.CooldownD.ShowCooldown();
        }

        public void Resurrection()
        {
            //OnDied?.Invoke();

            // todo data reset
            Manager.Game.MonsterCount = 0;

            DataSerialized = _originalData.Clone();

            OnDied = null;
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
            StateMachine.ChangeState(StateMachine.IdleState);

            currentAwakening = 0;
            AwakeningEvent?.Invoke(currentAwakening, Data.awakening.MaxAwakeningGauge);

            gameObject.layer = LayerMask.NameToLayer("Player");
            PlayerMaterial.SetDefaultMaterial();

            Inventory.Reset();

            Debug.Log("리셋");
            //ShadowSlashSkill?.Initialize(this);
            //DoubleStrikeSkill?.Initialize(this);
        }

        public void GainAwakeningGauge()
        {
            if (IsAwakened || IsDead || PlayerManager.Instance == null) return;
            var awakeningData = Data.awakening;
            currentAwakening = Mathf.Min(awakeningData.MaxAwakeningGauge, currentAwakening + awakeningData.AwakeningOnHit);

            AwakeningEvent?.Invoke(currentAwakening, awakeningData.MaxAwakeningGauge);

            if (currentAwakening >= awakeningData.MaxAwakeningGauge)
            {
                PlayerManager.Instance.CooldownD.HideCooldown();
            }
        }

        public void ActivateAwakening()
        {
            if (IsAwakened) return;

            PlayerManager.Instance.CooldownD.ShowCooldown();
            var awakeningData = Data.awakening;

            Debug.Log("각성!");
            IsAwakened = true;
            currentAwakening = awakeningData.MaxAwakeningGauge;
            float totalDuration = Data.awakening.Duration;
            Data.CombatData.AttackRange = 1.6f;
            Data.DashData.Cooldown = 0.5f;
            Data.GroundData.BaseSpeed = Data.GroundData.BaseSpeed+1;

            Animator.runtimeAnimatorController = awakenedAnimator;

            awakeningEffect.SetActive(true);

            PlayerManager.Instance.CooldownQ.ShowCooldown();
            PlayerManager.Instance.CooldownW.ShowCooldown();

            StartCoroutine(AwakeningTimer(totalDuration));
        }

        public IEnumerator OnAwken(float a)
        {
            yield return new WaitForSeconds(a);
            awakeningEffect_field.SetActive(true);
        }

        private IEnumerator AwakeningTimer(float duration)
        {
            float time = 0f;
            float start = currentAwakening;

            while (time < duration)
            {
                time += Time.deltaTime;
                currentAwakening = Mathf.Lerp(start, 0f, time / duration);

                AwakeningEvent?.Invoke(currentAwakening, Data.awakening.MaxAwakeningGauge);

                yield return null;
            }

            currentAwakening = 0f;
            IsAwakened = false;
            Data.CombatData.AttackRange = 1.1f;
            Data.GroundData.BaseSpeed = Data.GroundData.BaseSpeed - 1f;
            Data.DashData.Cooldown = 2f;

            Animator.runtimeAnimatorController = normalAnimator;

            awakeningEffect.SetActive(false);
            PlayerManager.Instance.CooldownQ.HideCooldown();
            PlayerManager.Instance.CooldownW.HideCooldown();

            AwakeningEvent?.Invoke(currentAwakening, Data.awakening.MaxAwakeningGauge);
            StartCoroutine(OnAwken(0f));

            Debug.Log("각성종료");
        }

        public void ShootShadow()
        {
            Debug.Log("추가공격");

            if (_ShadowSlashProjectilePrefab == null)
            {
                return;
            }

            Vector3 spawnPos = transform.position;
            float projectileOffset = 0.5f;
            spawnPos.x += StateMachine.FacingSign * projectileOffset;
            Quaternion rotation = StateMachine.FacingSign > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

            GameObject projectileGO = PlayerPool.Instance.GetFromPool(_ShadowSlashProjectilePrefab, spawnPos, rotation);

            if (projectileGO == null)
            {
                return;
            }

            ShadowSlashProjectile projectile = projectileGO.GetComponent<ShadowSlashProjectile>();
            if (projectile != null)
            {
                projectile.Launch(this);
            }
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
                StateMachine.MovementInput = b.normalized;

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            SetPlayerInput(true);
            StateMachine.MovementInput = Vector2.zero;
            Animator.SetBool(AnimationData.WalkParameterHash, false);
            Animator.SetBool(AnimationData.IdleParameterHash, true);
        }

        public void SetPlayerInput(bool isEnable)
        {
            StateMachine.InputActive = isEnable;

            if (!isEnable)
            {
                StateMachine.MovementInput = Vector2.zero;
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
            StateMachine.Tick();
            UpdateRuntimeDebug();

            var kb = UnityEngine.InputSystem.Keyboard.current;
            bool f = kb != null && kb.fKey.wasPressedThisFrame;
            if (f)
            {
                if (Interactables.Count == 0)
                    return;
                Interactables[Interactables.Count - 1].OnInteract();
            }

            //if (kb != null && kb.rKey.wasPressedThisFrame)
            //{
            //    Die();
            //}

            if (kb != null && kb.iKey.wasPressedThisFrame)
            {
                DebugInventoryContents();
            }
        }

        public void DebugInventoryContents()
        {

            if (Inventory == null || Inventory.Items.Count == 0)
            {
                Debug.Log("아이템 빔");
                return;
            }

            foreach (var itemEntry in Inventory.Items)
            {
                ItemData item = itemEntry.Value;
                Debug.Log($"-아이템 ID: {item.Id}, 이름: {item.ItemName}");
                Debug.Log($"-등급: {item.ItemGrade}");
                // 스탯 1 정보
                if (item.Stat1.ItemExtraStatType != ItemExtraStatType.None && item.Stat1.Value != 0)
                {
                    Debug.Log($"-스탯1: {ItemData.ItemExtraStatTypes[item.Stat1.ItemExtraStatType]} ({item.Stat1.Value})");
                }
                // 스탯 2 정보
                if (item.Stat2.ItemExtraStatType != ItemExtraStatType.None && item.Stat2.Value != 0)
                {
                    Debug.Log($"스탯2: {ItemData.ItemExtraStatTypes[item.Stat2.ItemExtraStatType]} ({item.Stat2.Value})");
                }
                Debug.Log($"-URL: {item.IconURL}");
                Debug.Log($"-특성: {item.ItemEffectId}, -시너지: {item.SynergyId}");
            }
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick();
            UpdateRuntimeDebug();
        }

        void UpdateRuntimeDebug()
        {
            if (!showRuntime) return;
            runtimeIsDashing = StateMachine.IsDashing;
            runtimeSpeedModifier = StateMachine.MovementSpeedModifier;
            runtimeMoveSpeed = StateMachine.MovementSpeed * StateMachine.MovementSpeedModifier;
#if UNITY_2022_3_OR_NEWER
            runtimeVelocity = Rb.linearVelocity;
#else
            runtimeVelocity = Rb.velocity;
#endif
        }
    }
}
