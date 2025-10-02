using Game.Player;
using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    ItemProjectileData _data;
    bool _isDestroyed = false;
    int _collisionCount = 0;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); // 나중에 투사체 이미지 따로 저장해야함
    }
    public void Init(ItemProjectileData data, PlayerCharacter owner)
    {
        _data = data;
        transform.position = owner.transform.position;
        _collisionCount = _data.CollisionCount;
        _isDestroyed = false;
        StartCoroutine(CoShot());
    }
    IEnumerator CoShot()
    {
        float startTime = Time.time;
        while (Time.time - startTime < _data.Duration && !_isDestroyed)
        {
            //방향 지정해야함
            transform.Translate(Vector2.right * _data.Speed * Time.deltaTime);
            yield return null;
        }
        End();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case var layer when layer == LayerMask.NameToLayer("Enemy"):
                _collisionCount--;
                Debug.Log("HITHITHIT");
                // 데미지 처리
                if (_collisionCount <= 0)
                    End();
                break;
            case var layer when layer == LayerMask.NameToLayer("Ground"):
                End();
                break;
        }
    }
    void End()
    {
        if (_isDestroyed) return;
        StopCoroutine(CoShot());
        Manager.Pool.Push<ProjectileController>(gameObject);
        _isDestroyed = true;
    }
}
