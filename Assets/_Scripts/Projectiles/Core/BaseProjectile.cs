using Game.Monster;
using System.Collections;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    private ProjectileDataHandler _dataHandler;
    public ProjectileDataHandler DataHandler {  get { return _dataHandler; } }

    protected Rigidbody2D _rb;
    protected SpriteRenderer _sr;
    protected Animator _anim;

    protected Transform _target;
    protected Vector3 _dir;

    private void Awake()
    {
        _dataHandler = GetComponent<ProjectileDataHandler>();
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        _rb.linearVelocityX = 0;
        StartCoroutine(ProjectileLife(DataHandler.Data.LifeTime));
    }

    public virtual void Init(Vector3 dir, Transform target = null, float attackPower = 0f)
    {
        _target = target;
        _dir = dir;
        _dataHandler.Damage = attackPower;
        _sr.flipX = dir.x < 0;
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected abstract void Move();

    // 일단 레이어 정보가 정확하지 않아 임시로 설정 추후 수정 필요
    // Todo: 레이어 정보 변경하기
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        // 플레이어면 데미지
        if((1 << other.gameObject.layer) == LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            Vector2 knockBackDir = new Vector2(_rb.linearVelocityX < 0 ? -1 : 1, 1);
            knockBackDir.Normalize();

            Debug.Log(knockBackDir);
            // 수치를 어떻게 조정해야하지?
            Rigidbody2D targetRb = _target.GetComponent<Rigidbody2D>();
            targetRb.linearVelocity = Vector2.zero;
            targetRb.AddForce(knockBackDir * 400);

            damageable?.TakeDamage((int)DataHandler.Damage);
            DestroyProjectile();
        }
        // 벽이나 땅이면 소멸
        // 일단 플레이어에 damageable이 없어서 조건 추가함
        else if((1 << other.gameObject.layer) == LayerMask.GetMask(Game.Monster.Layers.Ground) ||
                 (1 << other.gameObject.layer) == LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            DestroyProjectile();
        }
    }

    // Todo: 풀로 반환하기
    protected virtual void DestroyProjectile()
    {
        StopAllCoroutines();
        // 일단 디스트로이로 제거
        // 추후 풀로 반환
        Destroy(gameObject);
    }

    // 일정 시간이 지나면 자동으로 제거
    protected IEnumerator ProjectileLife(float lifetime)
    {
        float curTime = 0;
        while(curTime <= lifetime)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        DestroyProjectile() ;
    }
}
