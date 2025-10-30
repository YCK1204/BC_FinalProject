using Game.Monster;
using Game.Player;
using System;
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
    Vector2 _direction = Vector2.zero;
    CircleCollider2D _circleCollider2D;
    BoxCollider2D _boxCollider2D;
    void SetComponent()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _isSetComponent = true;
    }
    public void Init(ItemProjectileData data, PlayerCharacter owner)
    {
        if (!_isSetComponent)
            SetComponent();
        _data = data;
        transform.position = owner.transform.position;
        _collisionCount = _data.CollisionCount;
        _isDestroyed = false;

        switch (data.ImageType)
        {
            case ImageType.Sprite:
                _spriteRenderer.sprite = data.Sprite;
                _animator.enabled = false;
                _direction = owner.transform.localScale.x < 0 ? Vector2.left : Vector2.right;
                StartCoroutine(CoShotForward(false));
                break;
            case ImageType.Animation:
                _animator.runtimeAnimatorController = data.Animator;
                _animator.enabled = true;

                var clips = _animator.runtimeAnimatorController.animationClips;
                foreach (var clip in clips)
                {
                    var events = clip.events;
                    foreach (var e in events)
                    {
                        if (string.IsNullOrEmpty(e.functionName))
                        {
                            e.functionName = "End";
                        }
                    }
                    clip.events = events;
                }
                switch (data.MoveType)
                {
                    case ProjectileMoveType.Forward:
                        _direction = owner.transform.localScale.x < 0 ? Vector2.left : Vector2.right;
                        StartCoroutine(CoShotForward(true));
                        break;
                    case ProjectileMoveType.Tracking:
                        //var target = transform.FindNearestObject<BaseMonster>(_data.range, LayerMask.GetMask("Monster"));
                        //StartCoroutine(CoShotTracking(target));
                        break;
                    case ProjectileMoveType.Oblque:
                        StartCoroutine(CoShotObique());
                        break;
                }
                break;
        }
        _boxCollider2D.size = new Vector2(_data.Width, _data.Height);
        Manager.Audio.Play(data.AudioKey, transform);


        //if (data.ColliderType == ColliderType.Circle)
        //{
        //    _circleCollider2D.enabled = true;
        //    _boxCollider2D.enabled = false;
        //    _circleCollider2D.radius = data.ColliderRadius;
        //}
        //else if (data.ColliderType == ColliderType.Box)
        //{
        //    _circleCollider2D.enabled = false;
        //    _boxCollider2D.enabled = true;
        //    _boxCollider2D.size = data.ColliderSize;
        //}
    }
    IEnumerator CoShotForward(bool autoDestroy)
    {
        float startTime = Time.time;

        while ((autoDestroy && Time.time - startTime < _data.Duration) || (!autoDestroy))
        {
            transform.Translate(_direction * _data.Speed * Time.deltaTime);
            yield return null;
        }
        End();
    }
    IEnumerator CoShotTracking(BaseMonster target)
    {
        float startTime = Time.time;
        while (Time.time - startTime < _data.Duration)
        {
            if (target != null)
                _direction = (target.transform.position - transform.position).normalized;
            transform.Translate(_direction * _data.Speed * Time.deltaTime);
            yield return null;
        }
        End();
    }
    IEnumerator CoShotObique()
    {
        yield return null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case var layer when layer == LayerMask.NameToLayer("Monster"):
                HandleTriggerMonster(collision);
                break;
                //case var layer when layer == LayerMask.NameToLayer("Ground"):
                //    End();
                //    break;
        }
    }
    void HandleTriggerMonster(Collider2D collision)
    {
        _collisionCount--;
        var data = PlayerCharacter.Instance.Data.CombatData;
        collision.GetComponent<IDamageable>()?.TakeDamage(
            data.SkillAttck * (1 + data.SkillAttckPercent) * _data.Damage);
        if (_collisionCount <= 0)
            End();
    }
    public void End()
    {
        if (_isDestroyed) return;
        Manager.Pool.Push<ProjectileController>(gameObject);
        _isDestroyed = true;
        Manager.Audio.StopLoop(transform);
    }
}
