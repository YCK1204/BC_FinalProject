using Game.Monster;
using Game.Player;
using System.Collections;
using UnityEngine;

public class HomingProjectile : BaseProjectile
{
    private Coroutine _rotateCoroutine;
    private bool _startMove = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        _startMove = false;
        transform.localEulerAngles = Vector3.zero;
        Manager.Audio.Play(AudioKey.Monster.Projectile.M_PROJ_UNDEAD_MAGE_CREATE, transform);
    }

    protected override void FixedUpdate()
    {
        if (_startMove)
        {
            base.FixedUpdate();
        }
    }

    public override void Init(Vector3 dir, GameObject attacker, Transform target = null, float attackPower = 1)
    {
        base.Init(dir, attacker, target, attackPower);

    }

    protected override void Move()
    {
        _rb.linearVelocity = transform.up * DataHandler.Data.Speed;
    }

    private IEnumerator Rotate()
    {
        while (_target != null)
        {
            Vector2 dir = ((Vector2)_target.position - _rb.position).normalized;

            // 목표 회전값
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // 현재 회전에서 목표 회전으로 부드럽게 회전
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                DataHandler.Data.RotateSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    public void StartMove()
    {
        _anim.SetTrigger("StartMove");
        _startMove = true;
        Manager.Audio.Play(AudioKey.Monster.Projectile.M_PROJ_UNDEAD_MAGE_SHOT, transform);

        if (_target != null && _rotateCoroutine == null)
        {
            transform.localRotation = _dir.x > 0 ? Quaternion.Euler(0,0,-90f) : Quaternion.Euler(0, 0, 90f);
            _rotateCoroutine = StartCoroutine(Rotate());
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 움직이지 않는 상태에선 피격 판정X
        if (!_startMove)
            return;

        IDamageable damageable = other.gameObject.GetComponentInChildren<IDamageable>();
        // 플레이어면 데미지
        if ((1 << other.gameObject.layer) == LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            if (damageable == null) return;
            // 무적 체크
            PlayerCharacter pc = other.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            _rb.linearVelocity = Vector2.zero;
            _startMove = false;

            Vector2 knockBackDir = new Vector2(transform.position.x > other.transform.position.x ? -1 : 1, 0);
            knockBackDir.Normalize();

            // 수치를 어떻게 조정해야하지?
            Rigidbody2D targetRb = other.GetComponent<Rigidbody2D>();
            targetRb.linearVelocity = Vector2.zero;
            targetRb.AddForce(knockBackDir * 200);

            damageable?.TakeDamage(DataHandler.Damage, _attacker);
            if (_anim != null)
                _anim.SetTrigger("Explode");
            else
                DestroyProjectile();
        }
        // 벽이나 땅이면 소멸
        // 일단 플레이어에 damageable이 없어서 조건 추가함
        else if ((1 << other.gameObject.layer) == LayerMask.GetMask(Game.Monster.Layers.Ground))
        {
            _rb.linearVelocity = Vector2.zero;
            _startMove = false;
            if (_anim != null)
                _anim.SetTrigger("Explode");
            else
                DestroyProjectile();
        }
    }

    protected override void DestroyProjectile()
    {
        base.DestroyProjectile();
    }
}
