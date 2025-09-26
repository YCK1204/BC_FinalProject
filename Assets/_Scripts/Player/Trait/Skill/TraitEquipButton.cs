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
        [SerializeField] string _txtUnequip = "해제";

        Button _btn;

        public void Init(int traitId) { _traitId = traitId; }

        void Awake()
        {
            _btn = GetComponent<Button>();
            if (_label == null) _label = GetComponentInChildren<TMP_Text>(true);
        }

        void OnEnable()
        {
            _btn.onClick.AddListener(OnClick);
            if (SkillEquipSystem.Instance != null)
                SkillEquipSystem.Instance.OnEquipped += Refresh;
            Refresh(SkillEquipSystem.Instance ? SkillEquipSystem.Instance.GetSnapshot() : null);
        }

        void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);
            if (SkillEquipSystem.Instance != null)
                SkillEquipSystem.Instance.OnEquipped -= Refresh;
        }

        void OnClick()
        {
            var sys = SkillEquipSystem.Instance;   // ✅ null-prop 제거
            if (sys != null) sys.Toggle(_traitId);
        }

        void Refresh(int[] _)
        {
            var sys = SkillEquipSystem.Instance;   // (선택) Unity null 체크 명시화
            bool equipped = sys != null && sys.IsEquipped(_traitId);
            if (_label) _label.text = equipped ? _txtUnequip : _txtEquip;
        }
    }
}
