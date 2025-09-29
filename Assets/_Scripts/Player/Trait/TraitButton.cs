using UnityEngine;
using UnityEngine.UI;
using Game.Traits;
using Game.Traits.UI;

[RequireComponent(typeof(Image), typeof(Button))]
public class TraitButton : MonoBehaviour
{
    [SerializeField] private int _traitId;
    [SerializeField] private TraitUnlockSystem _unlockSystem;
    [SerializeField] private TraitTooltipBuilder _tooltipBuilder;

    [Header("Basic Skill")]
    [SerializeField] private bool _alwaysUnlocked = false;

    static readonly Color32 COLOR_UNLOCKED = new Color32(255, 255, 255, 255);
    static readonly Color32 COLOR_CAN = new Color32(153, 255, 153, 255);
    static readonly Color32 COLOR_LOCKED = new Color32(128, 128, 128, 255);

    Image _img;
    Button _btn;

    public int TraitId => _traitId;

    void Awake()
    {
        _img = GetComponent<Image>();
        _btn = GetComponent<Button>();

        if (_unlockSystem == null)
#if UNITY_2023_1_OR_NEWER
            _unlockSystem = Object.FindFirstObjectByType<TraitUnlockSystem>(FindObjectsInactive.Include);
#else
            _unlockSystem = FindObjectOfType<TraitUnlockSystem>(true);
#endif

        if (_tooltipBuilder == null)
            _tooltipBuilder = GetComponent<TraitTooltipBuilder>();
    }

    void OnEnable()
    {
        _btn.onClick.AddListener(OnClick);
        if (_unlockSystem != null) _unlockSystem.OnStateChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        _btn.onClick.RemoveListener(OnClick);
        if (_unlockSystem != null) _unlockSystem.OnStateChanged -= Refresh;
    }

    void OnClick()
    {
        if (_alwaysUnlocked) return;

        if (_unlockSystem != null)
            _unlockSystem.TryUnlock(_traitId);

        Refresh();
    }

    void Refresh()
    {
        if (_img == null) return;

        bool unlocked = _alwaysUnlocked || (_unlockSystem != null && _unlockSystem.IsUnlocked(_traitId));
        bool canUnlock = !_alwaysUnlocked && _unlockSystem != null && _unlockSystem.CanUnlock(_traitId);

        _img.color = unlocked ? COLOR_UNLOCKED : (canUnlock ? COLOR_CAN : COLOR_LOCKED);

        _btn.interactable = !_alwaysUnlocked && canUnlock;

        if (_tooltipBuilder != null)
            _tooltipBuilder.SetUnlocked(unlocked);

        var tip = TraitTooltip.Instance;
        if (tip != null && tip.gameObject.activeSelf && _tooltipBuilder != null)
            tip.Show(_tooltipBuilder.Build());
    }
}
