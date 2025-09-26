using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Traits;

namespace Game.Traits.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SkillBarController : MonoBehaviour
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
        [SerializeField] Color fontColor = Color.black;

        [Header("Unequip Button")]
        [SerializeField] string unequipText = "해제";
        [SerializeField] Vector2 unequipSize = new Vector2(44, 20);
        [SerializeField] Color unequipBg = new Color(0.18f, 0.18f, 0.18f);
        [SerializeField] Color unequipTextColor = Color.white;
        [SerializeField] int unequipFontSize = 16;
        [SerializeField] float unequipMarginTop = 6f; // 슬롯과 버튼 사이 간격

        readonly List<SlotView> _views = new();
        RectTransform _rt;

        void Awake()
        {
            _rt = (RectTransform)transform;
            ApplyAnchor();
            BuildSlots();
            Wire();
            RefreshAll();
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

        void OnDestroy()
        {
            if (SkillEquipSystem.Instance != null)
            {
                SkillEquipSystem.Instance.OnEquipped -= OnEquipped;
                SkillEquipSystem.Instance.OnSelectedSlotChanged -= OnSelectedSlot;
            }
        }

        void Wire()
        {
            if (SkillEquipSystem.Instance == null) return;
            SkillEquipSystem.Instance.OnEquipped += OnEquipped;
            SkillEquipSystem.Instance.OnSelectedSlotChanged += OnSelectedSlot;
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
            int count = SkillEquipSystem.Instance ? SkillEquipSystem.Instance.SlotCount : 2;

            // 버튼 공간까지 포함한 전체 높이
            float height = barPadding.y * 2 + slotSize.y + unequipMarginTop + unequipSize.y;
            float width = barPadding.x * 2 + (slotSize.x * count) + slotGap * (count - 1);
            _rt.sizeDelta = new Vector2(width, height);

            for (int i = 0; i < count; i++)
            {
                // 슬롯 배경
                var slotGO = new GameObject($"Slot{i + 1}", typeof(RectTransform), typeof(Image), typeof(Button));
                var slotRT = (RectTransform)slotGO.transform;
                slotRT.SetParent(_rt, false);
                slotRT.sizeDelta = slotSize;
                slotRT.anchorMin = slotRT.anchorMax = new Vector2(0, 0);
                slotRT.pivot = new Vector2(0, 0);
                // 슬롯은 위쪽(버튼 위)에 배치
                slotRT.anchoredPosition = new Vector2(
                    barPadding.x + i * (slotSize.x + slotGap),
                    barPadding.y + unequipMarginTop + unequipSize.y
                );

                var bg = slotGO.GetComponent<Image>();
                bg.color = slotColor;

                // 슬롯 라벨
                var tgo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
                var tr = (RectTransform)tgo.transform;
                tr.SetParent(slotRT, false);
                tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
                tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

                var label = tgo.GetComponent<TextMeshProUGUI>();
                if (font) label.font = font;
                label.fontSize = fontSize;
                label.color = fontColor;
                label.alignment = TextAlignmentOptions.Center;

                // 선택 아웃라인
                var selGo = new GameObject("Selected", typeof(RectTransform), typeof(Image));
                var srt = (RectTransform)selGo.transform;
                srt.SetParent(slotRT, false);
                srt.anchorMin = Vector2.zero; srt.anchorMax = Vector2.one;
                srt.offsetMin = srt.offsetMax = Vector2.zero;
                var selImg = selGo.GetComponent<Image>();
                selImg.color = selectedOutline;
                selImg.raycastTarget = false;
                selImg.type = Image.Type.Sliced;
                selImg.enabled = false;

                // ▶ 해제 버튼 (루트에 두되, 슬롯 기준으로 가운데 정렬 좌표 계산)
                var uGo = new GameObject("Unequip", typeof(RectTransform), typeof(Image), typeof(Button));
                var uRT = (RectTransform)uGo.transform;
                uRT.SetParent(_rt, false);
                uRT.sizeDelta = unequipSize;
                uRT.anchorMin = uRT.anchorMax = new Vector2(0, 0);
                uRT.pivot = new Vector2(0, 0);
                // X는 슬롯의 X + (슬롯폭-버튼폭)/2 → 수평 중앙
                float ux = slotRT.anchoredPosition.x + (slotSize.x - unequipSize.x) * 0.5f;
                float uy = barPadding.y; // 아래쪽 라인
                uRT.anchoredPosition = new Vector2(ux, uy);

                var uBg = uGo.GetComponent<Image>();
                uBg.color = unequipBg;

                var uTextGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                var uTextRT = (RectTransform)uTextGO.transform;
                uTextRT.SetParent(uRT, false);
                uTextRT.anchorMin = Vector2.zero; uTextRT.anchorMax = Vector2.one;
                uTextRT.offsetMin = Vector2.zero; uTextRT.offsetMax = Vector2.zero;

                var uLabel = uTextGO.GetComponent<TextMeshProUGUI>();
                if (font) uLabel.font = font;
                uLabel.fontSize = unequipFontSize;
                uLabel.color = unequipTextColor;
                uLabel.alignment = TextAlignmentOptions.Center;
                uLabel.text = unequipText;

                // 뷰/이벤트 연결
                var view = new SlotView
                {
                    index = i,
                    root = slotRT,
                    label = label,
                    select = selImg,
                    button = slotGO.GetComponent<Button>(),
                    unequipButton = uGo.GetComponent<Button>(),
                    unequipRoot = uGo
                };

                view.button.onClick.AddListener(() =>
                {
                    if (SkillEquipSystem.Instance != null)
                        SkillEquipSystem.Instance.SetSelectedSlot(view.index);
                    RefreshSelected();
                });

                view.unequipButton.onClick.AddListener(() =>
                {
                    if (SkillEquipSystem.Instance == null) return;
                    if (SkillEquipSystem.Instance.UnequipAt(view.index))
                    {
                        SkillEquipSystem.Instance.SetSelectedSlot(-1); // 선택 해제
                        RefreshAll();
                    }
                });

                _views.Add(view);
            }
        }

        void OnEquipped(int[] _) => RefreshAll();
        void OnSelectedSlot(int _) => RefreshSelected();

        void RefreshAll()
        {
            var sys = SkillEquipSystem.Instance;
            if (sys == null) return;

            for (int i = 0; i < _views.Count; i++)
            {
                int tid = sys.GetEquippedAt(i);
                _views[i].label.text = (tid >= 0) ? GetTraitNameById(tid) : $"슬롯 {i + 1}";

                // ✔ 장착된 경우에만 해제 버튼 표시(아예 숨김/표시)
                _views[i].unequipRoot.SetActive(tid >= 0);
            }
            RefreshSelected();
        }

        void RefreshSelected()
        {
            int sel = SkillEquipSystem.Instance ? SkillEquipSystem.Instance.SelectedSlot : -1;
            for (int i = 0; i < _views.Count; i++)
                _views[i].select.enabled = (i == sel);
        }

        string GetTraitNameById(int traitId)
        {
#if UNITY_2023_1_OR_NEWER
            foreach (var builder in FindObjectsByType<TraitTooltipBuilder>(FindObjectsSortMode.None))
#else
            foreach (var builder in FindObjectsOfType<TraitTooltipBuilder>(true))
#endif
            {
                if (builder.TryGetComponent<TraitButton>(out var btn) && btn.TraitId == traitId)
                    return builder.GetName();
            }
            return $"ID {traitId}";
        }

        // 빈 화면 클릭 시 선택 해제(원하면 유지)
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (SkillEquipSystem.Instance != null)
                {
                    SkillEquipSystem.Instance.SetSelectedSlot(-1);
                    RefreshSelected();
                }
            }
        }

        class SlotView
        {
            public int index;
            public RectTransform root;
            public Button button;
            public TextMeshProUGUI label;
            public Image select;

            public Button unequipButton;
            public GameObject unequipRoot; // 표시/숨김 제어용
        }
    }
}
