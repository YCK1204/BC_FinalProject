// UILineBetween.cs
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform), typeof(Image))]
public class UILineBetween : MonoBehaviour
{
    public enum NodeShape { Circle, Rect, Diamond }

    [Header("Targets")]
    public RectTransform from;
    public RectTransform to;

    [Header("Layer & Style")]
    public RectTransform lineLayer;
    [Min(1f)] public float thickness = 2f;

    [Header("Endpoint")]
    public NodeShape fromShape = NodeShape.Circle;
    public NodeShape toShape = NodeShape.Rect;
    public float fromInsetPx = 1.0f;
    public float toInsetPx = 1.0f;

    [Header("Auto Tighten")]
    public bool autoTight = true;
    public float autoOverlapPx = 1.0f;
    public bool snapOnlyAxisAligned = true;

    RectTransform _self, _layer;
    Canvas _canvas;
    Camera _cam;
    Image _img;
    bool _dirty;

    #region Unity
    void Awake() { Init(); MarkDirty(); }

    void OnEnable()
    {
        Init();
        MarkDirty();
        Canvas.willRenderCanvases += OnCanvasWillRender;  // ★ 렌더 직전 정렬
    }

    void OnDisable()
    {
        Canvas.willRenderCanvases -= OnCanvasWillRender;  // ★ 해제
    }

    void OnTransformParentChanged() { Init(); MarkDirty(); }
    void OnRectTransformDimensionsChange() { MarkDirty(); }
    void OnValidate() { MarkDirty(); }

    void LateUpdate()
    {
        // 에디터/플레이 프레임 갱신 보조
        if (_dirty) { _dirty = false; Refresh(); }
    }

    // ★ 캔버스가 렌더되기 직전에 항상 한 번 더 정렬
    void OnCanvasWillRender()
    {
        if (!isActiveAndEnabled) return;
        Init();      // 카메라/레이어 바뀌었을 수 있으니 재확인
        Refresh();
    }
    #endregion

    void Init()
    {
        _self = GetComponent<RectTransform>();
        _img = GetComponent<Image>();
        if (_img) { _img.type = Image.Type.Simple; _img.raycastTarget = false; }
        if (_self && _self.pivot != new Vector2(0f, 0.5f)) _self.pivot = new Vector2(0f, 0.5f);

        _layer = lineLayer ? lineLayer : (_self ? _self.parent as RectTransform : null);
        _canvas = _layer ? _layer.GetComponentInParent<Canvas>() : GetComponentInParent<Canvas>();
        _cam = (_canvas && _canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? _canvas.worldCamera : null;
    }
    void MarkDirty() { _dirty = true; }

    void Refresh()
    {
        if (_self == null || _layer == null || from == null || to == null) return;

        Vector2 a = WorldCenterInLayer(from);
        Vector2 b = WorldCenterInLayer(to);

        Vector2 ab = b - a;
        float dist = ab.magnitude;
        if (dist < 0.001f) { _self.sizeDelta = Vector2.zero; return; }
        Vector2 dir = ab / dist;

        Vector2 halfA = HalfSizeInLayer(from);
        Vector2 halfB = HalfSizeInLayer(to);

        Vector2 start =
            fromShape == NodeShape.Circle ? EdgeOnEllipse(a, dir, halfA, fromInsetPx) :
            fromShape == NodeShape.Diamond ? EdgeOnDiamond(a, dir, halfA, fromInsetPx) :
                                             EdgeOnRect(a, dir, halfA, fromInsetPx);

        Vector2 end =
            toShape == NodeShape.Circle ? EdgeOnEllipse(b, -dir, halfB, toInsetPx) :
            toShape == NodeShape.Diamond ? EdgeOnDiamond(b, -dir, halfB, toInsetPx) :
                                             EdgeOnRect(b, -dir, halfB, toInsetPx);

        if (autoTight)
        {
            bool diagonal = Mathf.Abs(dir.x) > 0.01f && Mathf.Abs(dir.y) > 0.01f;
            float overlap = Mathf.Max(0f, autoOverlapPx + (diagonal ? 0.5f : 0f));
            start -= dir * overlap;
            end += dir * overlap;
        }

        Vector2 d = end - start;
        float len = d.magnitude;
        if (len < 0.001f) { _self.sizeDelta = Vector2.zero; return; }

        bool axisAligned = Mathf.Abs(dir.x) < 0.001f || Mathf.Abs(dir.y) < 0.001f;
        if (!snapOnlyAxisAligned || axisAligned)
        {
            start = new Vector2(Mathf.Round(start.x), Mathf.Round(start.y));
            len = Mathf.Round(len);
        }

        if (_self.parent != _layer) _self.SetParent(_layer, false);
        _self.anchoredPosition = start;
        _self.sizeDelta = new Vector2(len, Mathf.Max(1f, Mathf.Round(thickness)));
        _self.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
    }

    Vector2 WorldCenterInLayer(RectTransform t)
    {
        Vector3 world = t.TransformPoint(t.rect.center);
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(_cam, world);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_layer, screen, _cam, out var local);
        return local;
    }
    Vector2 HalfSizeInLayer(RectTransform t)
    {
        Vector2 scale = new Vector2(
            t.lossyScale.x / _layer.lossyScale.x,
            t.lossyScale.y / _layer.lossyScale.y);
        Vector2 size = Vector2.Scale(t.rect.size, scale);
        return size * 0.5f;
    }

    Vector2 EdgeOnEllipse(Vector2 center, Vector2 dirFromCenter, Vector2 half, float inset)
    {
        Vector2 d = dirFromCenter.normalized;
        float a = Mathf.Max(0.0001f, half.x - inset);
        float b = Mathf.Max(0.0001f, half.y - inset);
        float t = 1f / Mathf.Sqrt((d.x * d.x) / (a * a) + (d.y * d.y) / (b * b));
        return center + d * t;
    }
    Vector2 EdgeOnRect(Vector2 center, Vector2 dirFromCenter, Vector2 half, float inset)
    {
        Vector2 d = dirFromCenter.normalized;
        Vector2 h = new Vector2(Mathf.Max(0f, half.x - inset), Mathf.Max(0f, half.y - inset));
        float tx = (Mathf.Abs(d.x) < 1e-4f) ? float.PositiveInfinity : h.x / Mathf.Abs(d.x);
        float ty = (Mathf.Abs(d.y) < 1e-4f) ? float.PositiveInfinity : h.y / Mathf.Abs(d.y);
        return center + d * Mathf.Min(tx, ty);
    }
    Vector2 EdgeOnDiamond(Vector2 center, Vector2 dirFromCenter, Vector2 half, float inset)
    {
        Vector2 d = dirFromCenter.normalized;
        float a = Mathf.Max(0.0001f, half.x - inset);
        float b = Mathf.Max(0.0001f, half.y - inset);
        float denom = (Mathf.Abs(d.x) / a) + (Mathf.Abs(d.y) / b);
        float t = 1f / Mathf.Max(denom, 1e-6f);
        return center + d * t;
    }
}
