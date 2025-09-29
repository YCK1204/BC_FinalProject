using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Traits;

namespace Game.Traits.UI
{
    [RequireComponent(typeof(Button))]
    public class TraitEquipButton : MonoBehaviour
    {
        [SerializeField] int _traitId;
        [SerializeField] TMP_Text _label;
        [SerializeField] string _txtEquip = "장착";
        [SerializeField] bool _alwaysUnlocked = false;

        Button _btn;
        CanvasGroup _group;
        TraitUnlockSystem _unlock;

        void Awake()
        {
            _btn = GetComponent<Button>();
            if (_label == null) _label = GetComponentInChildren<TMP_Text>(true);
            if (_label) _label.text = _txtEquip;

            _group = GetComponent<CanvasGroup>();
            if (_group == null) _group = gameObject.AddComponent<CanvasGroup>();

#if UNITY_2023_1_OR_NEWER
            _unlock = Object.FindFirstObjectByType<TraitUnlockSystem>(FindObjectsInactive.Include);
#else
            _unlock = FindObjectOfType<TraitUnlockSystem>(true);
#endif
        }

        void OnEnable()
        {
            _btn.onClick.AddListener(OnClick);

            var sys = SkillEquipSystem.Instance;
            if (sys != null)
            {
                sys.OnEquipped += OnSys;
                sys.OnSnapshotChanged += OnSys;
                sys.OnSelectedSlotChanged += _ => UpdateState();
            }
            if (_unlock != null) _unlock.OnStateChanged += UpdateState;

            UpdateState();
        }

        void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);

            var sys = SkillEquipSystem.Instance;
            if (sys != null)
            {
                sys.OnEquipped -= OnSys;
                sys.OnSnapshotChanged -= OnSys;
                sys.OnSelectedSlotChanged -= _ => UpdateState();
            }
            if (_unlock != null) _unlock.OnStateChanged -= UpdateState;
        }

        void OnClick()
        {
            var sys = SkillEquipSystem.Instance;
            if (sys == null) return;
            if (sys.Equip(_traitId))
                sys.ClearSelection();
            UpdateState();
        }

        void OnSys(int[] _) => UpdateState();

        void UpdateState()
        {
            var sys = SkillEquipSystem.Instance;

            bool isBasic = _alwaysUnlocked;
            bool unlocked = isBasic || (_unlock != null && _unlock.IsUnlocked(_traitId));
            bool equipped = sys != null && sys.IsEquipped(_traitId);
            bool hasSelection = sys != null && sys.SelectedSlot >= 0;

            bool visible = isBasic || (unlocked && !equipped);
            bool interactable = visible && hasSelection;

            if (_group != null)
            {
                _group.alpha = visible ? 1f : 0f;
                _group.interactable = interactable;
                _group.blocksRaycasts = interactable;
            }
            if (_label != null) _label.text = _txtEquip;
        }

        public void Init(int traitId) { _traitId = traitId; UpdateState(); }

        public void SetAlwaysUnlocked(bool v) { _alwaysUnlocked = v; UpdateState(); }
        public void Refresh() { UpdateState(); }
    }
}
