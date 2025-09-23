using UnityEngine;
using UnityEngine.UI;
using Game.Traits;
using System.Collections;

[RequireComponent(typeof(Image))]
public class TraitLineIndicator : MonoBehaviour
{
    public TraitUnlockSystem system; // 인스펙터 드래그 권장
    public int fromId;
    public int toId;

    public Color unlockedColor = Color.white;                        // 둘 다 해금
    public Color canUnlockColor = new Color(0.7f, 1f, 0.7f, 1f);      // 해금 가능(연두)
    public Color lockedColor = new Color(0.35f, 0.35f, 0.35f, 1f); // 잠김(회색)

    Image _img;
    Coroutine _bind;

    void Awake()
    {
        _img = GetComponent<Image>();
        if (_img) { _img.raycastTarget = false; _img.color = lockedColor; } // ✅ 기본은 회색
    }

    void OnEnable()
    {
        _bind = StartCoroutine(BindAndRefresh());
    }

    void OnDisable()
    {
        if (_bind != null) { StopCoroutine(_bind); _bind = null; }
        if (system != null)
        {
            system.OnStateChanged -= Refresh;
            system.OnUnlocked -= OnUnlocked;
            if (system.Wallet != null) system.Wallet.OnSoulChanged -= OnSoulChanged;
        }
    }

    IEnumerator BindAndRefresh()
    {
        for (int i = 0; i < 5 && system == null; i++)
        {
#if UNITY_2023_1_OR_NEWER
            system = Object.FindFirstObjectByType<TraitUnlockSystem>();
#else
            system = FindObjectOfType<TraitUnlockSystem>();
#endif
            if (system == null) yield return null;
        }

        if (system != null)
        {
            system.OnStateChanged += Refresh;
            system.OnUnlocked += OnUnlocked;
            if (system.Wallet != null)
                system.Wallet.OnSoulChanged += OnSoulChanged;
        }

        Refresh();
    }

    void OnUnlocked(int _) => Refresh();
    void OnSoulChanged(int _) => Refresh();

    void Refresh()
    {
        if (_img == null || system == null || toId == 0) return;

        bool fromUnlocked = system.IsUnlocked(fromId);
        bool toUnlocked = system.IsUnlocked(toId);

        if (fromUnlocked && toUnlocked) _img.color = unlockedColor;   // 둘 다 해금 → 흰색
        else if (fromUnlocked && system.CanUnlock(toId)) _img.color = canUnlockColor; // 해금 가능 → 연두
        else _img.color = lockedColor;     // 나머지 → 회색
    }
}
