using Game.Player;
using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    ItemProjectileData _data;
    bool _isDestroyed = false;
    int _collisionCount = 0;
    Animator _animator;
    bool _isSetComponent = false;
    void SetPomponent()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); // 나중에 투사체 이미지 따로 저장해야함
        _animator = GetComponent<Animator>();
        _isSetComponent = true;
    }
    public void Init(ItemProjectileData data, PlayerCharacter owner)
    {
        if (!_isSetComponent)
            SetPomponent();
        _data = data;
        transform.position = owner.transform.position;
        _collisionCount = _data.CollisionCount;
        _isDestroyed = false;
        switch (data.ImageType)
        {
            case ImageType.Sprite:
                _spriteRenderer.sprite = data.sprite;
                _animator.enabled = false;
                break;
            case ImageType.Animation:
                _animator.runtimeAnimatorController = data.Animator;
                _animator.enabled = true;
                break;
        }
        StartCoroutine(CoShot());
    }
    IEnumerator CoShot()
    {
        float startTime = Time.time;
        PlayerCharacter player = PlayerCharacter.Instance;
        Vector2 dir = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        while (Time.time - startTime < _data.Duration && !_isDestroyed)
        {
            //방향 지정해야함
            transform.Translate(dir * _data.Speed * Time.deltaTime);
            yield return null;
        }
        End();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case var layer when layer == LayerMask.NameToLayer("Monster"):
                _collisionCount--;
                Debug.Log("HITHITHIT");
                // 데미지 처리
                if (_collisionCount <= 0)
                    End();
                break;
                //case var layer when layer == LayerMask.NameToLayer("Ground"):
                //    End();
                //    break;
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
