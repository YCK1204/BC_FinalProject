using GameSystem;
using System.Collections;
using UnityEngine;

public class BuffAbilityData : SpecialAbilityData
{
    [SerializeField]
    float duration;
    public float Duration { get { return duration; } }
    [SerializeField]
    int maxCount;
    public int MaxCount { get { return maxCount; } }
    [SerializeField]
    ItemStat stat1;
    public ItemStat Stat1 { get { return stat1; } }
    [SerializeField]
    ItemStat stat2;
    public ItemStat Stat2 { get { return stat2; } }
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get { return icon; } }
    [SerializeField]
    string buffName;
    public string BuffName { get { return buffName; } }
    [SerializeField, TextArea]
    string description;
    public string Description { get { return description; } }

    float _lastActivationTime;
    Coroutine _activeCoroutine;
    int _currentCount = 0;
    public override void Activate(PlayerCharacter player)
    {
        _lastActivationTime = Time.time;
        if (_activeCoroutine != null) player.StopCoroutine(_activeCoroutine);
        _currentCount = Mathf.Min(_currentCount + 1, maxCount);
        _activeCoroutine = player.StartCoroutine(CoBuff(player));
    }
    public override void Inactivate(PlayerCharacter player)
    {
        if (_activeCoroutine != null) player.StopCoroutine(_activeCoroutine);
        _activeCoroutine = null;
    }
    IEnumerator CoBuff(PlayerCharacter player)
    {
        while (Time.time < _lastActivationTime + duration)
        {
            //if (stat1.ItemStatType != ItemStatType.None)
            //player.BuffStat(stat1.ItemStatType, stat1.Value * _currentCount);
            //if (stat2.ItemStatType != ItemStatType.None)
            //player.BuffStat(stat2.ItemStatType, stat2.Value * _currentCount);
            yield return null;
        }
        _activeCoroutine = null;
        _currentCount = 0;
    }
}
