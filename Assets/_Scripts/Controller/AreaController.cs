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
        _data = data;
        _animator.runtimeAnimatorController = _data.Animator;

        switch (_data.CreatePosType)
        {
            case ItemEffectCreatePosType.Player:
                transform.position = owner.transform.position;
                break;
            case ItemEffectCreatePosType.NearestEnemy:
                transform.position = GetNearestEnemyPosition(owner).position;
                break;
            case ItemEffectCreatePosType.WithInRangeEnemy:
                transform.position = GetInRangeEnemyPosition(owner).position;
                break;
        }

        SetAnimEvent();
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
    Transform GetNearestEnemyPosition(PlayerCharacter player)
    {
        var hits = Physics2D.OverlapCircleAll(player.transform.position, _data.Radius, LayerMask.GetMask("Monster"));
        if (hits.Length != 0)
        {
            try
            {
                return hits.Where(x => x != null).OrderBy(x => Vector2.Distance(player.transform.position, x.transform.position)).First().transform;
            }
            catch (System.Exception e)
            {
            }
        }
        return player.transform;
    }
    Transform GetInRangeEnemyPosition(PlayerCharacter player)
    {
        var hits = Physics2D.OverlapCircleAll(player.transform.position, _data.Radius, LayerMask.GetMask("Monster"));
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

        foreach (var hit in hits)
        {
            var monster = hit.GetComponent<NormalMonster>();
            monster.TakeDamage(0);
        }
    }
}
