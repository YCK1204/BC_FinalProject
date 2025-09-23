using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    }

    public class TraitTooltip : MonoBehaviour
    {
        public static TraitTooltip Instance { get; private set; }

        [Header("Refs")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _panel;   // Image 포함
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _subTitle;
        [SerializeField] private RectTransform _body;    // 본문 컨테이너
        [SerializeField] private TMP_Text _linePrefab;   // 비활성 복제용

        [Header("Layout (px)")]
        [SerializeField] private float _maxWidth = 520f;   // 박스 폭(조절 가능)
        [SerializeField] private float _paddingX = 18f;    // 좌/우 여백
        [SerializeField] private float _paddingTop = 18f;  // 위 여백 ↑   (여유공간)
        [SerializeField] private float _paddingBottom = 18f; // 아래 여백 ↓
        [SerializeField] private float _gapTitleToSub = 4f;
        [SerializeField] private float _gapSubToBody = 12f;
        [SerializeField] private float _gapTitleToBody = 12f; // SubTitle 없을 때
        [SerializeField] private float _lineSpacing = 8f;

        [Header("Follow")]
        [SerializeField] private Vector2 _mouseOffset = new Vector2(24f, -16f);
        [SerializeField] private Vector2 _paddingClamp = new Vector2(12f, 12f);

        [Header("Style")]
        [SerializeField] private bool _overrideTextStyle = true; // 아래 폰트/정렬을 코드에서 강제 적용
        [SerializeField] private float _titleFontSize = 28f;     // 제목 폰트 크기 ↑ (원하는 만큼 키워도 됨)
        [SerializeField] private float _subTitleFontSize = 18f;
        [SerializeField] private float _bodyFontSize = 16f;
        [SerializeField] private bool _centerTitleAndSub = true; // 제목/부제 중앙 정렬
        [SerializeField] private Color _bodyColor = new Color(0.82f, 0.82f, 0.82f);
        [SerializeField] private Color _alertColor = new Color(0.90f, 0.20f, 0.20f);

        private RectTransform _canvasRect;
        private Camera _uiCam;
        private readonly List<TMP_Text> _spawned = new List<TMP_Text>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            _canvasRect = _canvas.GetComponent<RectTransform>();
            _uiCam = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;

            if (_panel) _panel.pivot = new Vector2(0f, 1f); // 기본: 좌상 피벗
            HideImmediate();

            // 최신 TMP API
            if (_title) _title.textWrappingMode = TextWrappingModes.Normal;
            if (_subTitle) _subTitle.textWrappingMode = TextWrappingModes.Normal;
        }

        public void Show(TraitTooltipModel model)
        {
            if (model == null) { HideImmediate(); return; }

            // 헤더 텍스트 세팅
            _title.text = model.Title ?? string.Empty;

            bool hasSub = !string.IsNullOrEmpty(model.SubTitle);
            _subTitle.gameObject.SetActive(hasSub);
            _subTitle.text = hasSub ? model.SubTitle : string.Empty;

            // 스타일 오버라이드(원하면 끄기 가능)
            if (_overrideTextStyle)
            {
                if (_title) _title.fontSize = _titleFontSize;
                if (_subTitle) _subTitle.fontSize = _subTitleFontSize;
                if (_centerTitleAndSub)
                {
                    if (_title) _title.alignment = TextAlignmentOptions.Center;
                    if (_subTitle) _subTitle.alignment = TextAlignmentOptions.Center;
                }
            }

            // 본문 재생성
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

            Relayout();
            _panel.gameObject.SetActive(true);
            LateReposition(Input.mousePosition);
        }

        public void Hide() => _panel.gameObject.SetActive(false);
        public void HideImmediate() => Hide();

        // ===== 수동 레이아웃 =====
        private void Relayout()
        {
            float contentWidth = Mathf.Max(0f, _maxWidth - 2f * _paddingX);

            var titleSize = MeasureTMP(_title, contentWidth);
            bool hasSub = _subTitle.gameObject.activeSelf;
            Vector2 subSize = Vector2.zero;
            if (hasSub) subSize = MeasureTMP(_subTitle, contentWidth);

            float y = -_paddingTop; // 좌상 기준

            // Title
            PlaceText(_title.rectTransform, new Vector2(_paddingX, -_paddingTop), new Vector2(contentWidth, titleSize.y));
            y -= titleSize.y;

            if (hasSub)
            {
                y -= _gapTitleToSub;
                PlaceText(_subTitle.rectTransform, new Vector2(_paddingX, y), new Vector2(contentWidth, subSize.y));
                y -= subSize.y;
                y -= _gapSubToBody;
            }
            else
            {
                y -= _gapTitleToBody;
            }

            // Body
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

            // 패널 최종 크기
            float totalHeight = _paddingTop + titleSize.y
                                + (hasSub ? (_gapTitleToSub + subSize.y + _gapSubToBody) : _gapTitleToBody)
                                + bodyHeight
                                + _paddingBottom;

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

        // ===== 자동 뒤집기 + 클램프 =====
        public void LateReposition(Vector3 screenPos)
        {
            if (!_panel.gameObject.activeSelf) return;

            Vector2 target = (Vector2)screenPos + _mouseOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, target, _uiCam, out Vector2 local);

            Vector2 size = _panel.rect.size;
            Vector2 half = _canvasRect.rect.size * 0.5f;

            Vector2 pivot = new Vector2(0f, 1f); // 기본: 좌상

            if (local.x + size.x + _paddingClamp.x > half.x) pivot.x = 1f;   // 오른쪽 끝 → 좌측 기준
            if (local.y - size.y - _paddingClamp.y < -half.y) pivot.y = 0f;  // 아래 끝    → 상단 기준

            _panel.pivot = pivot;
            _panel.anchoredPosition = local;

            Vector2 min = -half + _paddingClamp;
            Vector2 max = half - _paddingClamp - size;

            var p = _panel.anchoredPosition;
            p.x = Mathf.Clamp(p.x, min.x, max.x);
            p.y = Mathf.Clamp(p.y, min.y, max.y);
            _panel.anchoredPosition = p;
        }
    }
}
