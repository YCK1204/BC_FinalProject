using UnityEngine;
using UnityEngine.UI;
using Game.Traits;

[RequireComponent(typeof(Image), typeof(Button))]
public class TraitButton : MonoBehaviour
{
    [SerializeField] private int _traitId;
    [SerializeField] private TraitUnlockSystem _unlockSystem;

    // 색상 상수 (알파 255 고정)
    static readonly Color32 COLOR_UNLOCKED = new Color32(255, 255, 255, 255); // 흰색
    static readonly Color32 COLOR_CAN = new Color32(153, 255, 153, 255); // 연두 #99FF99
    static readonly Color32 COLOR_LOCKED = new Color32(128, 128, 128, 255); // 회색

    Image _img;
    Button _btn;

    void Awake()
    {
        _img = GetComponent<Image>();
        _btn = GetComponent<Button>();
        if (_unlockSystem == null)
#if UNITY_2023_1_OR_NEWER
            _unlockSystem = Object.FindFirstObjectByType<TraitUnlockSystem>();
#else
            _unlockSystem = FindObjectOfType<TraitUnlockSystem>();
#endif
    }

    void OnEnable()
    {
        _btn.onClick.AddListener(OnClick);
        if (_unlockSystem != null) _unlockSystem.OnStateChanged += Refresh;
        Refresh(); // 초기 칠하기
    }

    void OnDisable()
    {
        _btn.onClick.RemoveListener(OnClick);
        if (_unlockSystem != null) _unlockSystem.OnStateChanged -= Refresh;
    }

    void OnClick()
    {
        _unlockSystem?.TryUnlock(_traitId);
    }

    void Refresh()
    {
        if (_unlockSystem == null || _img == null) return;

        bool unlocked = _unlockSystem.IsUnlocked(_traitId);
        bool canUnlock = _unlockSystem.CanUnlock(_traitId);

        _img.color = unlocked ? COLOR_UNLOCKED : (canUnlock ? COLOR_CAN : COLOR_LOCKED);
        _btn.interactable = canUnlock;
    }
}
