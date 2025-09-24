using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Traits.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TraitEquipButtonSpawner : MonoBehaviour
    {
        [SerializeField] int traitId;
        [SerializeField] TMP_FontAsset font;
        [SerializeField] Vector2 boxSize = new Vector2(88f, 30f);
        [SerializeField] int fontSize = 18;
        [SerializeField] float gap = 10f;

        RectTransform node;
        RectTransform parent;
        Canvas canvas;
        Camera uiCam;
        Button spawned;
        TextMeshProUGUI label;

        static readonly Color32 BASE = new Color32(125, 125, 125, 255);
        static readonly Color32 HIL = new Color32(150, 150, 150, 255);
        static readonly Color32 PRS = new Color32(100, 100, 100, 255);

#if UNITY_2023_1_OR_NEWER
        static T FindOne<T>() where T : Object =>
            Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
        static T FindOne<T>() where T : Object =>
            Object.FindObjectOfType<T>(true);
#endif
        TraitUnlockSystem _unlock;

        void Awake()
        {
            node = GetComponent<RectTransform>();
            parent = node.parent as RectTransform;
            canvas = GetComponentInParent<Canvas>();
            uiCam = canvas && canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
            _unlock = FindOne<TraitUnlockSystem>();
        }

        void OnEnable()
        {
            if (_unlock == null) _unlock = FindOne<TraitUnlockSystem>();
            if (_unlock) _unlock.OnStateChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (_unlock) _unlock.OnStateChanged -= Refresh;
        }

        void Refresh()
        {
            if (_unlock == null) _unlock = FindOne<TraitUnlockSystem>();
            bool unlocked = _unlock && _unlock.IsUnlocked(traitId);
            if (unlocked) EnsureButton();
            if (spawned)
            {
                spawned.gameObject.SetActive(unlocked);
                if (unlocked) { ApplySizing(); Repos(); }
            }
        }

        void EnsureButton()
        {
            if (spawned) return;

            var go = new GameObject("EquipButton", typeof(RectTransform), typeof(Image), typeof(Button));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);

            var img = go.GetComponent<Image>();
            img.color = BASE;

            var tgo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            var tr = (RectTransform)tgo.transform;
            tr.SetParent(rt, false);
            tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

            label = tgo.GetComponent<TextMeshProUGUI>();
            label.alignment = TextAlignmentOptions.Center;
            label.font = font != null ? font : TMP_Settings.defaultFontAsset;
            label.color = Color.white;
            label.text = "장착";

            var btn = go.GetComponent<Button>();
            var cb = btn.colors;
            cb.normalColor = BASE;
            cb.highlightedColor = HIL;
            cb.pressedColor = PRS;
            cb.selectedColor = BASE;
            cb.disabledColor = BASE;
            cb.colorMultiplier = 1f;
            btn.colors = cb;
            btn.transition = Selectable.Transition.ColorTint;

            var equip = go.AddComponent<TraitEquipButton>();
            equip.Init(traitId);

            spawned = btn;
            ApplySizing();
        }

        void ApplySizing()
        {
            var rt = (RectTransform)spawned.transform;
            rt.sizeDelta = boxSize;
            if (label)
            {
                label.font = font != null ? font : TMP_Settings.defaultFontAsset;
                label.fontSize = fontSize;
            }
        }

        void Repos()
        {
            if (spawned == null || parent == null) return;

            var btnRT = (RectTransform)spawned.transform;
            btnRT.anchorMin = new Vector2(0.5f, 0.5f);
            btnRT.anchorMax = new Vector2(0.5f, 0.5f);
            btnRT.pivot = new Vector2(0.5f, 1f);

            Vector3[] corners = new Vector3[4];
            node.GetWorldCorners(corners);
            Vector3 bottomCenterWorld = (corners[0] + corners[3]) * 0.5f;

            Vector2 bottomCenterScreen = RectTransformUtility.WorldToScreenPoint(uiCam, bottomCenterWorld);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, bottomCenterScreen, uiCam, out var local);

            local.y -= gap;
            btnRT.anchoredPosition = local;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (spawned)
            {
                ApplySizing();
                Repos();
            }
        }
#endif
    }
}
