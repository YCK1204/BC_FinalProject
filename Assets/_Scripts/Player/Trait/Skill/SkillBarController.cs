using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Traits.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SkillBarController : MonoBehaviour, IPointerClickHandler
    {
        public enum AnchorCorner { TopLeft, TopRight, BottomLeft, BottomRight }

        [Header("Layout")]
        [SerializeField] Vector2 barPadding = new Vector2(12, 12);
        [SerializeField] Vector2 slotSize = new Vector2(44, 44);
        [SerializeField] float slotGap = 10f;
        [SerializeField] AnchorCorner anchorCorner = AnchorCorner.BottomRight;
        [SerializeField] Vector2 anchorOffset = new Vector2(20, 20);

        [Header("Style")]
        [SerializeField] Color slotColor = new Color(0.93f, 0.93f, 0.93f);
        [SerializeField] Color selectedOutline = new Color(0.1f, 0.1f, 0.1f);
        [SerializeField] TMP_FontAsset font;
        [SerializeField] int fontSize = 18;
        [SerializeField] Color labelColor = new Color(0.7f, 0.7f, 0.7f);

        [Header("Unequip Button")]
        [SerializeField] Vector2 unequipSize = new Vector2(52, 24);
        [SerializeField] float unequipGap = 6f;
        [SerializeField] string unequipText = "해제";
        [SerializeField] Color32 unequipBase = new Color32(60, 60, 60, 255);
        [SerializeField] Color32 unequipHi = new Color32(80, 80, 80, 255);
        [SerializeField] Color32 unequipPressed = new Color32(40, 40, 40, 255);
        [SerializeField] Color unequipLabelColor = new Color(0.9f, 0.9f, 0.9f);

        RectTransform _rt;
        readonly List<SlotView> _views = new();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (SkillEquipSystem.Instance != null) SkillEquipSystem.Instance.ClearSelection();
            RefreshSelected();
        }

        void Awake()
        {
            _rt = (RectTransform)transform;
            ApplyAnchor();
            BuildSlots();
            _rt.SetAsFirstSibling();
        }

        void OnEnable()
        {
            Wire();
            RefreshAll();
        }

        void OnDisable()
        {
            Unwire();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (_rt == null) _rt = (RectTransform)transform;
                ApplyAnchor();
            }
        }
#endif

        void Wire()
        {
            var sys = Game.Traits.SkillEquipSystem.Instance;
            if (sys == null) return;
            Unwire();
            sys.OnEquipped += OnEquipped;
            sys.OnSelectedSlotChanged += OnSelectedSlot;
        }

        void Unwire()
        {
            var sys = Game.Traits.SkillEquipSystem.Instance;
            if (sys == null) return;
            sys.OnEquipped -= OnEquipped;
            sys.OnSelectedSlotChanged -= OnSelectedSlot;
        }

        void ApplyAnchor()
        {
            Vector2 anchor = Vector2.zero, pivot = Vector2.zero;
            switch (anchorCorner)
            {
                case AnchorCorner.TopLeft: anchor = new Vector2(0, 1); pivot = new Vector2(0, 1); break;
                case AnchorCorner.TopRight: anchor = new Vector2(1, 1); pivot = new Vector2(1, 1); break;
                case AnchorCorner.BottomLeft: anchor = new Vector2(0, 0); pivot = new Vector2(0, 0); break;
                case AnchorCorner.BottomRight: anchor = new Vector2(1, 0); pivot = new Vector2(1, 0); break;
            }
            _rt.anchorMin = _rt.anchorMax = anchor;
            _rt.pivot = pivot;

            float x = (anchor.x > 0.5f) ? -anchorOffset.x : anchorOffset.x;
            float y = (anchor.y > 0.5f) ? -anchorOffset.y : anchorOffset.y;
            _rt.anchoredPosition = new Vector2(x, y);
        }

        void BuildSlots()
        {
            int count = Game.Traits.SkillEquipSystem.Instance ? Game.Traits.SkillEquipSystem.Instance.SlotCount : 2;

            float width = barPadding.x * 2 + (slotSize.x * count) + slotGap * (count - 1);
            float height = barPadding.y * 2 + slotSize.y + unequipGap + unequipSize.y;
            _rt.sizeDelta = new Vector2(width, height);

            for (int i = 0; i < count; i++)
            {
                var go = new GameObject($"Slot{i + 1}", typeof(RectTransform), typeof(Image), typeof(Button));
                var rt = (RectTransform)go.transform;
                rt.SetParent(_rt, false);
                rt.sizeDelta = slotSize;
                rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
                rt.pivot = new Vector2(0, 0);
                rt.anchoredPosition = new Vector2(barPadding.x + i * (slotSize.x + slotGap),
                                                  barPadding.y + unequipGap + unequipSize.y);

                var bg = go.GetComponent<Image>();
                bg.color = slotColor;
                bg.raycastTarget = true;

                var tgo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
                var tr = (RectTransform)tgo.transform;
                tr.SetParent(rt, false);
                tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
                tr.offsetMin = tr.offsetMax = Vector2.zero;

                var label = tgo.GetComponent<TextMeshProUGUI>();
                if (font) label.font = font;
                label.fontSize = fontSize;
                label.fontStyle |= FontStyles.Bold;
                label.alignment = TextAlignmentOptions.Center;
                label.color = labelColor;
                label.raycastTarget = false;

                var selGo = new GameObject("Selected", typeof(RectTransform), typeof(Image));
                var srt = (RectTransform)selGo.transform;
                srt.SetParent(rt, false);
                srt.anchorMin = Vector2.zero; srt.anchorMax = Vector2.one;
                srt.offsetMin = srt.offsetMax = Vector2.zero;
                var selImg = selGo.GetComponent<Image>();
                selImg.color = selectedOutline;
                selImg.raycastTarget = false;
                selImg.type = Image.Type.Sliced;
                selImg.enabled = false;

                var unequipGo = new GameObject("Unequip", typeof(RectTransform), typeof(Image), typeof(Button));
                var urt = (RectTransform)unequipGo.transform;
                urt.SetParent(_rt, false);
                urt.sizeDelta = unequipSize;
                urt.anchorMin = urt.anchorMax = new Vector2(0, 0);
                urt.pivot = new Vector2(0.5f, 1f);
                urt.anchoredPosition = new Vector2(rt.anchoredPosition.x + slotSize.x * 0.5f,
                                                   barPadding.y + unequipSize.y);

                var uimg = unequipGo.GetComponent<Image>();
                uimg.color = unequipBase;

                var utxtGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
                var utr = (RectTransform)utxtGo.transform;
                utr.SetParent(urt, false);
                utr.anchorMin = Vector2.zero; utr.anchorMax = Vector2.one;
                utr.offsetMin = utr.offsetMax = Vector2.zero;

                var utxt = utxtGo.GetComponent<TextMeshProUGUI>();
                if (font) utxt.font = font;
                utxt.fontSize = Mathf.Max(12, fontSize - 4);
                utxt.alignment = TextAlignmentOptions.Center;
                utxt.color = unequipLabelColor;
                utxt.text = unequipText;
                utxt.raycastTarget = false;

                var ub = unequipGo.GetComponent<Button>();
                var cb = ub.colors;
                cb.normalColor = unequipBase;
                cb.highlightedColor = unequipHi;
                cb.pressedColor = unequipPressed;
                cb.selectedColor = unequipBase;
                cb.disabledColor = unequipBase;
                cb.colorMultiplier = 1f;
                ub.colors = cb;

                int slotIndex = i;

                var view = new SlotView
                {
                    index = i,
                    root = rt,
                    label = label,
                    select = selImg,
                    button = go.GetComponent<Button>(),
                    unequipRoot = urt,
                    unequipButton = ub
                };

                view.button.onClick.AddListener(() =>
                {
                    var sys = Game.Traits.SkillEquipSystem.Instance;
                    if (sys != null) sys.SetSelectedSlot(slotIndex);
                    RefreshSelected();
                });

                view.unequipButton.onClick.AddListener(() =>
                {
                    var sys = Game.Traits.SkillEquipSystem.Instance;
                    if (sys == null) return;
                    if (sys.UnequipAt(slotIndex))
                    {
                        sys.SetSelectedSlot(-1);
                        RefreshAll();
                    }
                });

                view.unequipRoot.gameObject.SetActive(false);
                _views.Add(view);
            }
        }

        void OnEquipped(int[] _) => RefreshAll();
        void OnSelectedSlot(int _) => RefreshSelected();

        void RefreshAll()
        {
            var sys = Game.Traits.SkillEquipSystem.Instance;
            if (sys == null) return;

            for (int i = 0; i < _views.Count; i++)
            {
                int tid = sys.GetEquippedAt(i);
                _views[i].label.text = (tid >= 0) ? GetTraitNameById(tid) : $"슬롯 {i + 1}";
                _views[i].unequipRoot.gameObject.SetActive(tid >= 0);
            }
            RefreshSelected();
        }

        void RefreshSelected()
        {
            int sel = Game.Traits.SkillEquipSystem.Instance ? Game.Traits.SkillEquipSystem.Instance.SelectedSlot : -1;
            for (int i = 0; i < _views.Count; i++)
                _views[i].select.enabled = (i == sel);
        }

        string GetTraitNameById(int traitId)
        {
#if UNITY_2023_1_OR_NEWER
            var builders = FindObjectsByType<TraitTooltipBuilder>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var builders = FindObjectsOfType<TraitTooltipBuilder>(true);
#endif
            for (int i = 0; i < builders.Length; i++)
            {
                var tb = builders[i];
                var btn = tb.GetComponent<TraitButton>();
                if (btn != null && btn.TraitId == traitId)
                    return tb.GetName();
            }
            return $"ID {traitId}";
        }

        class SlotView
        {
            public int index;
            public RectTransform root;
            public Button button;
            public TextMeshProUGUI label;
            public Image select;
            public RectTransform unequipRoot;
            public Button unequipButton;
        }
    }
}
