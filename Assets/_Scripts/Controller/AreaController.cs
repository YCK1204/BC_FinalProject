using Game.Monster;
using Game.Player;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    ItemAreaData _data;
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    bool _isSetComponent = false;
    bool _isDestroyed = false;
    void SetPomponent()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); // 나중에 투사체 이미지 따로 저장해야함
        _animator = GetComponent<Animator>();
        _isSetComponent = true;
    }
    public void Init(ItemAreaData data, PlayerCharacter owner)
    {
        if (!_isSetComponent)
            SetPomponent();
        _isDestroyed = false;
        _data = data;
        _animator.runtimeAnimatorController = _data.Animator;

        Bounds bounds = default(Bounds);

        switch (_data.CreatePosType)
        {
            case ItemEffectCreatePosType.Player:
                transform.position = owner.transform.position;
                bounds = owner.transform.FindChild<SpriteRenderer>().bounds;
                break;
            case ItemEffectCreatePosType.NearestEnemy:
                var pos = owner.transform.FindNearestObject<BaseMonster>(_data.Radius, "Monster")?.transform.position;
                if (pos == null)
                    transform.position = owner.transform.position;
                else
                    bounds = owner.transform.FindChild<SpriteRenderer>().bounds;
                break;
            case ItemEffectCreatePosType.WithInRangeEnemy:
                var ranPos = GetInRangeEnemyPosition(owner);
                var renderer = ranPos.GetComponent<SpriteRenderer>();
                if (renderer == null)
                    renderer = ranPos.FindChild<SpriteRenderer>(true);
                bounds = renderer.bounds;
                break;
        }

        switch (_data.DetailPosition)
        {
            case DetailPosition.TopLeft:
                transform.position = new Vector2(bounds.min.x, bounds.max.y);
                break;
            case DetailPosition.TopCenter:
                transform.position = new Vector2(bounds.center.x, bounds.max.y);
                break;
            case DetailPosition.TopRight:
                transform.position = new Vector2(bounds.max.x, bounds.max.y);
                break;
            case DetailPosition.MiddleLeft:
                transform.position = new Vector2(bounds.min.x, bounds.center.y);
                break;
            case DetailPosition.MiddleCenter:
                transform.position = new Vector2(bounds.center.x, bounds.center.y);
                break;
            case DetailPosition.MiddleRight:
                transform.position = new Vector2(bounds.max.x, bounds.center.y);
                break;
            case DetailPosition.BottomLeft:
                transform.position = new Vector2(bounds.min.x, bounds.min.y);
                break;
            case DetailPosition.BottomCenter:
                transform.position = new Vector2(bounds.center.x, bounds.min.y);
                break;
            case DetailPosition.BottomRight:
                transform.position = new Vector2(bounds.max.x, bounds.min.y);
                break;
        }

        SetAnimEvent();
        Manager.Audio.Play(data.AudioKey, transform);
    }
    void SetAnimEvent()
    {
        var clips = _animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            bool modified = false;
            var events = clip.events;
            foreach (var e in events)
            {
                if (e == events[events.Length - 1])
                {
                    if (e.functionName != "End")
                    {
                        e.functionName = "End";
                        modified = true;
                    }
                    continue;
                }
                if (string.IsNullOrEmpty(e.functionName))
                {
                    e.functionName = "HitEnemiesInRadius";
                    modified = true;
                }
            }
            if (modified)
            {
                clip.events = events;
            }
        }
    }
    Transform GetInRangeEnemyPosition(PlayerCharacter player)
    {
        var hits = Physics2D.OverlapCircleAll(player.transform.position, _data.DetectionRange, LayerMask.GetMask("Monster"));
        if (hits.Length != 0)
        {
            hits = hits.Where(x => x != null).ToArray();
            return hits[Random.Range(0, hits.Length)].transform;
        }
        return player.transform;
    }
    public void HitEnemiesInRadius()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _data.Radius, LayerMask.GetMask("Monster"));

        var data = PlayerCharacter.Instance.Data.CombatData;
        foreach (var hit in hits)
        {
            var monster = hit.GetComponent<IDamageable>();
            monster.TakeDamage(data.SkillAttck * (1 + data.SkillAttckPercent) * _data.Damage);
        }
    }
    public void End()
    {
        if (_isDestroyed) return;
        Manager.Pool.Push<AreaController>(gameObject);
        _isDestroyed = true;
        Manager.Audio.StopLoop(transform);
    }
}
