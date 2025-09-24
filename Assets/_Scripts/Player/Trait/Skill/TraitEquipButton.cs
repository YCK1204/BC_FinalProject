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
        [SerializeField] string _txtEquipped = "장착됨";
        Button _btn;

        void Awake()
        {
            _btn = GetComponent<Button>();
            if (_label == null) _label = GetComponentInChildren<TMP_Text>(true);
        }

        void OnEnable()
        {
            _btn.onClick.AddListener(OnClick);
            if (SkillEquipSystem.Instance) SkillEquipSystem.Instance.OnEquipped += Refresh;
            Refresh(SkillEquipSystem.Instance ? SkillEquipSystem.Instance.Equipped : null);
        }

        void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);
            if (SkillEquipSystem.Instance) SkillEquipSystem.Instance.OnEquipped -= Refresh;
        }

        public void Init(int traitId) { _traitId = traitId; }

        void OnClick()
        {
            if (SkillEquipSystem.Instance) SkillEquipSystem.Instance.Equip(_traitId);
        }

        void Refresh(int[] equipped)
        {
            bool mine = SkillEquipSystem.Instance && SkillEquipSystem.Instance.IsEquipped(_traitId);
            if (_label) _label.text = mine ? _txtEquipped : _txtEquip;
            _btn.interactable = !mine;

            var cb = _btn.colors;
            cb.disabledColor = cb.normalColor;
            _btn.colors = cb;
        }
    }
}
