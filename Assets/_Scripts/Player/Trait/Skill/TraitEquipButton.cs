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
        [SerializeField] bool _requireUnlock = true;

        bool _alwaysUnlocked = false; 
        Button _btn;

        TraitUnlockSystem _unlock; 

#if UNITY_2023_1_OR_NEWER
        static T FindOne<T>() where T : Object =>
            Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
        static T FindOne<T>() where T : Object =>
            Object.FindObjectOfType<T>(true);
#endif

        public void Init(int traitId) { _traitId = traitId; }
        public void SetAlwaysUnlocked(bool v) { _alwaysUnlocked = v; }

        void Awake()
        {
            _btn = GetComponent<Button>();
            if (_label == null) _label = GetComponentInChildren<TMP_Text>(true);
            if (_unlock == null) _unlock = FindOne<TraitUnlockSystem>();
        }

        void OnEnable()
        {
            if (_btn != null) _btn.onClick.AddListener(OnClick);

            if (_unlock == null) _unlock = FindOne<TraitUnlockSystem>();
            if (_unlock != null) _unlock.OnStateChanged += Refresh;

            if (SkillEquipSystem.Instance != null)
                SkillEquipSystem.Instance.OnSnapshotChanged += OnSnapshot;

            Refresh();
        }

        void OnDisable()
        {
            if (_btn != null) _btn.onClick.RemoveListener(OnClick);

            if (_unlock != null) _unlock.OnStateChanged -= Refresh;

            if (SkillEquipSystem.Instance != null)
                SkillEquipSystem.Instance.OnSnapshotChanged -= OnSnapshot;
        }

        void OnClick()
        {
            var sys = SkillEquipSystem.Instance;
            if (sys == null) return;

            if (_requireUnlock && !_alwaysUnlocked)
            {
                if (_unlock == null) _unlock = FindOne<TraitUnlockSystem>();
                if (_unlock == null || !_unlock.IsUnlocked(_traitId)) return;
            }

            sys.Equip(_traitId);
        }

        void OnSnapshot(int[] _) => Refresh();

        public void Refresh()
        {
            var sys = SkillEquipSystem.Instance;

            bool equipped = (sys != null) && sys.IsEquipped(_traitId);
            bool unlocked = true;

            if (_requireUnlock && !_alwaysUnlocked)
            {
                if (_unlock == null) _unlock = FindOne<TraitUnlockSystem>();
                unlocked = (_unlock != null) && _unlock.IsUnlocked(_traitId);
            }

            bool show = (!equipped) && unlocked;
            gameObject.SetActive(show);

            if (_label != null) _label.text = _txtEquip;
            if (_btn != null) _btn.interactable = show;
        }
    }
}
