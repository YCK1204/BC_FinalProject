using Game.Monster;
using System.Collections;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    private ProjectileDataHandler _data;
    public ProjectileDataHandler Data {  get { return _data; } }

    protected Rigidbody2D _rb;

    protected Transform _target;
    protected Vector3 _dir;
    
    public void SetData(ProjectileDataHandler data)
    {
        _data = data;
    }

    private void Awake()
    {
        _data = Extension.GetOrAddComponent<ProjectileDataHandler>(this.gameObject);
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ProjectileLife(Data.LifeTime));
    }

    public virtual void Init(Vector3 dir, Transform target = null)
    {
        _target = target;
        _dir = dir;
    }

    private void FixedUpdate()
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
        if(damageable != null && (1 << other.gameObject.layer) != LayerMask.GetMask(Common.Layers.Monster))
        {
            damageable.TakeDamage(_data.Damage);
            DestroyProjectile();
        }
        // 벽이나 땅이면 소멸
        else if((1 << other.gameObject.layer) != LayerMask.GetMask(Common.Layers.Monster))
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
