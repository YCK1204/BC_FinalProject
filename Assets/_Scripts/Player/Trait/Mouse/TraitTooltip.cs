using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Traits.UI
{
    [Serializable]
    public struct TooltipLine
    {
        public string Text;
        public bool Emphasize;
        public bool Alert;
        public TooltipLine(string text, bool emphasize = false, bool alert = false)
        { Text = text; Emphasize = emphasize; Alert = alert; }
    }

    [Serializable]
    public class TraitTooltipModel
    {
        public string Title;
        public string SubTitle;
        public List<TooltipLine> Lines = new List<TooltipLine>();
        public bool HasAction;
        public string ActionText;
        public Action Action;
    }

    public class TraitTooltip : MonoBehaviour
    {
        public static TraitTooltip Instance { get; private set; }

        [Header("Refs")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _subTitle;
        [SerializeField] private RectTransform _body;
        [SerializeField] private TMP_Text _linePrefab;

        [Header("Action")]
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionLabel;

        [Header("Layout (px)")]
        [SerializeField] private float _maxWidth = 520f;
        [SerializeField] private float _paddingX = 18f;
        [SerializeField] private float _paddingTop = 18f;
        [SerializeField] private float _paddingBottom = 18f;
        [SerializeField] private float _gapTitleToSub = 4f;
        [SerializeField] private float _gapSubToBody = 12f;
        [SerializeField] private float _gapTitleToBody = 12f;
        [SerializeField] private float _lineSpacing = 8f;
        [SerializeField] private float _gapBodyToAction = 12f;
        [SerializeField] private float _actionHeight = 40f;

        [Header("Follow")]
        [SerializeField] private Vector2 _mouseOffset = new Vector2(24f, -16f);
        [SerializeField] private Vector2 _paddingClamp = new Vector2(12f, 12f);

        [Header("Style")]
        [SerializeField] private bool _overrideTextStyle = true;
        [SerializeField] private float _titleFontSize = 32f;
        [SerializeField] private float _subTitleFontSize = 18f;
        [SerializeField] private float _bodyFontSize = 18f;
        [SerializeField] private bool _centerTitleAndSub = true;
        [SerializeField] private Color _bodyColor = new Color(0.82f, 0.82f, 0.82f);
        [SerializeField] private Color _alertColor = new Color(0.90f, 0.20f, 0.20f);

        private RectTransform _canvasRect;
        private Camera _uiCam;
        private readonly List<TMP_Text> _spawned = new List<TMP_Text>();
        private Action _pendingAction;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            _canvasRect = _canvas.GetComponent<RectTransform>();
            _uiCam = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;

            if (_panel) _panel.pivot = new Vector2(0f, 1f);
            HideImmediate();

            if (_title) _title.textWrappingMode = TextWrappingModes.Normal;
            if (_subTitle) _subTitle.textWrappingMode = TextWrappingModes.Normal;

            if (_actionButton != null) _actionButton.onClick.AddListener(() => _pendingAction?.Invoke());
        }

        public void Show(TraitTooltipModel model)
        {
            if (model == null) { HideImmediate(); return; }

            _title.text = model.Title ?? string.Empty;

            bool hasSub = !string.IsNullOrEmpty(model.SubTitle);
            _subTitle.gameObject.SetActive(hasSub);
            _subTitle.text = hasSub ? model.SubTitle : string.Empty;

            if (_overrideTextStyle)
            {
                if (_title) _title.fontSize = _titleFontSize;
                if (_subTitle) _subTitle.fontSize = _subTitleFontSize;
                if (_centerTitleAndSub)
                {
                    _title.alignment = TextAlignmentOptions.Center;
                    _subTitle.alignment = TextAlignmentOptions.Center;
                }
            }

            foreach (var t in _spawned) if (t) Destroy(t.gameObject);
            _spawned.Clear();

            foreach (var line in model.Lines)
            {
                var t = Instantiate(_linePrefab, _body);
                t.gameObject.SetActive(true);
                t.textWrappingMode = TextWrappingModes.Normal;
                if (_overrideTextStyle) t.fontSize = _bodyFontSize;
                t.text = line.Text;
                t.fontStyle = line.Emphasize ? FontStyles.Bold : FontStyles.Normal;
                t.color = line.Alert ? _alertColor : (line.Emphasize ? Color.white : _bodyColor);
                _spawned.Add(t);
            }

            _pendingAction = null;
            if (_actionButton != null)
            {
                bool show = model.HasAction && model.Action != null;
                _actionButton.gameObject.SetActive(show);
                if (show)
                {
                    _pendingAction = model.Action;
                    if (_actionLabel != null) _actionLabel.text = string.IsNullOrEmpty(model.ActionText) ? "장착" : model.ActionText;
                }
            }

            Relayout();
            _panel.gameObject.SetActive(true);
            LateReposition(Input.mousePosition);
        }

        public void Hide() => _panel.gameObject.SetActive(false);
        public void HideImmediate() => Hide();

        private void Relayout()
        {
            float contentWidth = Mathf.Max(0f, _maxWidth - 2f * _paddingX);

            var titleSize = MeasureTMP(_title, contentWidth);
            bool hasSub = _subTitle.gameObject.activeSelf;
            Vector2 subSize = Vector2.zero;
            if (hasSub) subSize = MeasureTMP(_subTitle, contentWidth);

            float y = -_paddingTop;

            PlaceText(_title.rectTransform, new Vector2(_paddingX, -_paddingTop), new Vector2(contentWidth, titleSize.y));
            y -= titleSize.y;

            if (hasSub)
            {
                y -= _gapTitleToSub;
                PlaceText(_subTitle.rectTransform, new Vector2(_paddingX, y), new Vector2(contentWidth, subSize.y));
                y -= subSize.y + _gapSubToBody;
            }
            else
            {
                y -= _gapTitleToBody;
            }

            _body.anchorMin = new Vector2(0f, 1f);
            _body.anchorMax = new Vector2(0f, 1f);
            _body.pivot = new Vector2(0f, 1f);
            _body.anchoredPosition = new Vector2(_paddingX, y);

            float bodyHeight = 0f;
            for (int i = 0; i < _spawned.Count; i++)
            {
                var lt = _spawned[i];
                var size = MeasureTMP(lt, contentWidth);
                PlaceText(lt.rectTransform, new Vector2(0f, -bodyHeight), new Vector2(contentWidth, size.y));
                bodyHeight += size.y;
                if (i < _spawned.Count - 1) bodyHeight += _lineSpacing;
            }
            _body.sizeDelta = new Vector2(contentWidth, bodyHeight);
            y -= bodyHeight;

            float actionBlock = 0f;
            if (_actionButton != null && _actionButton.gameObject.activeSelf)
            {
                actionBlock = _gapBodyToAction + _actionHeight;
                var brt = (RectTransform)_actionButton.transform;
                brt.anchorMin = new Vector2(0f, 1f);
                brt.anchorMax = new Vector2(0f, 1f);
                brt.pivot = new Vector2(0f, 1f);
                brt.anchoredPosition = new Vector2(_paddingX, y - _gapBodyToAction);
                brt.sizeDelta = new Vector2(contentWidth, _actionHeight);
            }

            float totalHeight = _paddingTop + titleSize.y
                                + (hasSub ? (_gapTitleToSub + subSize.y + _gapSubToBody) : _gapTitleToBody)
                                + bodyHeight + actionBlock + _paddingBottom;

            _panel.sizeDelta = new Vector2(_maxWidth, totalHeight);
        }

        private Vector2 MeasureTMP(TMP_Text t, float width)
        {
            t.textWrappingMode = TextWrappingModes.Normal;
            var size = t.GetPreferredValues(t.text, width, 0f);
            return new Vector2(width, size.y);
        }

        private void PlaceText(RectTransform rt, Vector2 topLeft, Vector2 size)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = topLeft;
            rt.sizeDelta = size;
        }

        public void LateReposition(Vector3 screenPos)
        {
            if (!_panel.gameObject.activeSelf) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, screenPos, _uiCam, out Vector2 localAtMouse);

            Vector2 size = _panel.rect.size;
            Vector2 half = _canvasRect.rect.size * 0.5f;

            Vector2 pivot = new Vector2(0f, 1f);
            if (localAtMouse.x + _mouseOffset.x + size.x + _paddingClamp.x > half.x) pivot.x = 1f;
            if (localAtMouse.y + _mouseOffset.y - size.y - _paddingClamp.y < -half.y) pivot.y = 0f;
            _panel.pivot = pivot;

            Vector2 offset = _mouseOffset;
            offset.x = (pivot.x == 1f) ? -Mathf.Abs(_mouseOffset.x) : Mathf.Abs(_mouseOffset.x);
            offset.y = (pivot.y == 0f) ? Mathf.Abs(_mouseOffset.y) : -Mathf.Abs(_mouseOffset.y);

            Vector2 target = (Vector2)screenPos + offset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, target, _uiCam, out Vector2 local);
            _panel.anchoredPosition = local;

            Vector2 minPos = -half + _paddingClamp + Vector2.Scale(size, _panel.pivot);
            Vector2 maxPos = half - _paddingClamp - Vector2.Scale(size, Vector2.one - _panel.pivot);

            var p = _panel.anchoredPosition;
            p.x = Mathf.Clamp(p.x, minPos.x, maxPos.x);
            p.y = Mathf.Clamp(p.y, minPos.y, maxPos.y);
            _panel.anchoredPosition = p;
        }
    }
}
