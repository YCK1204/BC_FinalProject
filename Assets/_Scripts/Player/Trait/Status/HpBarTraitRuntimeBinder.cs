// Assets/_Scripts/UI/HpBarTraitRuntimeBinder.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Player;
using Game.Traits;

[DefaultExecutionOrder(-150)]
public class HpBarTraitRuntimeBinder : MonoBehaviour
{
    [SerializeField] PlayerCharacter player;
    [SerializeField] TraitUnlockSystem unlock;
    [SerializeField] List<HpBar> bars = new List<HpBar>();

#if UNITY_2023_1_OR_NEWER
    static T FindOne<T>() where T : Object =>
        Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
    static T[] FindMany<T>() where T : Object =>
        Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
    static T FindOne<T>() where T : Object =>
        Object.FindObjectOfType<T>(true);
    static T[] FindMany<T>() where T : Object =>
        Object.FindObjectsOfType<T>(true);
#endif

    void Awake()
    {
        if (!player) player = FindOne<PlayerCharacter>();
        if (!unlock) unlock = FindOne<TraitUnlockSystem>();
        if (bars.Count == 0) bars.AddRange(FindMany<HpBar>());
        AttachBars();
    }

    void OnEnable()
    {
        if (unlock != null)
        {
            unlock.OnStateChanged += RefreshNow;
            unlock.OnUnlocked += _ => RefreshNow();
        }
        if (player != null) player.HpEvent += OnHpEvent;
        RefreshNow();
    }

    void OnDisable()
    {
        if (unlock != null)
        {
            unlock.OnStateChanged -= RefreshNow;
            unlock.OnUnlocked -= _ => RefreshNow();
        }
        if (player != null) player.HpEvent -= OnHpEvent;
    }

    void AttachBars()
    {
        foreach (var b in bars)
        {
            if (!b) continue;
            if (!b.slider)
            {
                var s = b.GetComponent<Slider>();
                if (!s) s = b.GetComponentInChildren<Slider>(true);
                b.slider = s;
            }
            if (!b.player && player) b.player = player;
        }
    }

    void OnHpEvent(float cur, float max) => Propagate(cur, max);

    void RefreshNow()
    {
        if (!player) return;
        Propagate(player.CurrentHP, player.Data.Stats.MaxHP);
    }

    void Propagate(float cur, float max)
    {
        float ratio = max <= 0f ? 0f : cur / max;
        foreach (var b in bars)
            if (b) b.SetHp(ratio);
    }
}
