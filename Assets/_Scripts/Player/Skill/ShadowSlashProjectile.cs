using Game.Monster;
using Game.Player;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSlashProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _distance = 8f;
    [SerializeField] private float _damageMult = 1.5f;
    [SerializeField] private float _lifeTime = 5f;

    private Rigidbody2D _rb;
    private Vector2 _startPos;
    private float _damage;
    private readonly List<IDamageable> _hitTargets = new List<IDamageable>();

    private EffectReturn _effectReturn;
    private PlayerCharacter _owner;

    [SerializeField] private GameObject _hitPrefab;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _effectReturn = GetComponent<EffectReturn>();
    }

    private void OnEnable()
    {
        _hitTargets.Clear();
        _owner = null;

        if (_effectReturn != null)
        {
            _effectReturn.AutoReturnTime = _lifeTime;
        }
    }
    public void Launch(PlayerCharacter owner)
    {
        _owner = owner;
        _startPos = transform.position;
        _damage = (owner.Data.CombatData.SkillAttck * ( 1 + owner.Data.CombatData.SkillAttckPercent)) * _damageMult;
        _rb.linearVelocity = transform.right * _speed;
    }

    private void FixedUpdate()
    {
        float traveled = Vector2.Distance(_startPos, transform.position);

        if (traveled >= _distance)
        {
            _effectReturn.Return();
            return;
        }

        float currentSpeed = Mathf.Lerp(_speed, 0f, traveled / _distance);
        _rb.linearVelocity = transform.right * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            if (_owner != null && other.gameObject == _owner.gameObject)
            {
                return;
            }
            if (!_hitTargets.Contains(target))
            {
                if (target is BaseMonster baseMonsterTarget && baseMonsterTarget.MonsterData.CurHp <= 0)
                {
                    return;
                }

                Transform targetTransform = ((MonoBehaviour)target).transform;

                _owner.StateMachine.Player.GainAwakeningGauge();
                target.TakeDamage((int)_damage);
                _hitTargets.Add(target);

                float randomZ = Random.Range(0f, 360f);
                Quaternion randomRot = Quaternion.Euler(0f, 0f, randomZ);
                Vector3 spawnPos = targetTransform.position;

                PlayerPool.Instance.GetFromPool(_hitPrefab, spawnPos, randomRot);
            }
        }
    }
}