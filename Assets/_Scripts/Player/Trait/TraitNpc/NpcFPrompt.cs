using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public class NpcFPrompt : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] LayerMask playerLayers;
    [SerializeField] KeyCode interactKey = KeyCode.F;
    [SerializeField] KeyCode closeKey = KeyCode.Escape;

    [Header("Position")]
    [SerializeField] bool useColliderBottom = true;
    [SerializeField] Vector2 localOffset = new Vector2(0f, -0.4f);
    [SerializeField] float extraBottomPadding = 0.05f;

    [Header("Visual")]
    [SerializeField] TMP_FontAsset font;
    [SerializeField] float fontSize = 0.45f;
    [SerializeField] Vector2 boxSize = new Vector2(0.52f, 0.30f);
    [SerializeField] Color boxColor = Color.white;
    [SerializeField] Color textColor = Color.black;
    [SerializeField] string sortingLayer = "Default";
    [SerializeField] int sortingOrderBase = 300;
    [SerializeField] string keyText = "F";

    [Header("Trait Window (no controller)")]
    [SerializeField] GameObject traitWindowRoot;
    [SerializeField] bool autoFindByName = true;
    [SerializeField] string traitWindowName = "TraitWindow";

    public UnityEvent OnInteract;

    Transform _root;
    SpriteRenderer _box;
    TextMeshPro _label;

    bool _inside;
    bool _windowOpen;

    static Sprite _whiteSprite;
    static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                var c = new Color32(255, 255, 255, 255);
                var arr = new Color32[] { c, c, c, c };
                tex.SetPixels32(arr);
                tex.Apply();
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 100f);
                _whiteSprite.name = "RuntimeWhite";
            }
            return _whiteSprite;
        }
    }

    void Awake()
    {
        EnsureVisuals();
        EnsureWindowRef();
        SetVisible(false);
        Reposition();
        CloseWindowImmediate();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) EnsureVisuals();
        ApplyVisuals();
        Reposition();
    }

    void OnTriggerEnter(Collider other) { if (IsPlayer(other.gameObject.layer)) Enter(); }
    void OnTriggerExit(Collider other) { if (IsPlayer(other.gameObject.layer)) Exit(); }
    void OnTriggerEnter2D(Collider2D o) { if (IsPlayer(o.gameObject.layer)) Enter(); }
    void OnTriggerExit2D(Collider2D o) { if (IsPlayer(o.gameObject.layer)) Exit(); }

    void Update()
    {
        if (_inside && !_windowOpen && Input.GetKeyDown(interactKey))
        {
            OnInteract?.Invoke();
            OpenWindow();
        }

        if (_windowOpen && Input.GetKeyDown(closeKey))
            CloseWindow();
    }

    bool IsPlayer(int layer) => (playerLayers.value & (1 << layer)) != 0;

    void Enter() { _inside = true; if (!_windowOpen) SetVisible(true); Reposition(); }
    void Exit() { _inside = false; if (!_windowOpen) SetVisible(false); }

    void EnsureVisuals()
    {
        if (_root == null)
        {
            var existed = transform.Find("FPrompt");
            _root = existed ? existed : new GameObject("FPrompt").transform;
            _root.SetParent(transform, false);
        }

        if (_box == null)
        {
            var go = _root.Find("Box") ? _root.Find("Box").gameObject
                                       : new GameObject("Box", typeof(SpriteRenderer));
            go.transform.SetParent(_root, false);
            _box = go.GetComponent<SpriteRenderer>();
            _box.sprite = WhiteSprite;
        }

        if (_label == null)
        {
            var go = _root.Find("Label") ? _root.Find("Label").gameObject
                                         : new GameObject("Label", typeof(TextMeshPro));
            go.transform.SetParent(_root, false);
            _label = go.GetComponent<TextMeshPro>();
        }

        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        int layerId = SortingLayer.NameToID(sortingLayer);

        if (_box)
        {
            _box.color = boxColor;
            _box.sortingLayerID = layerId;
            _box.sortingOrder = sortingOrderBase;
            _box.transform.localScale = new Vector3(boxSize.x, boxSize.y, 1f);
        }

        if (_label)
        {
            _label.font = font != null ? font : TMP_Settings.defaultFontAsset;
            _label.enableAutoSizing = false;
            _label.fontSize = fontSize;
            _label.alignment = TextAlignmentOptions.Center;
            _label.color = textColor;
            _label.text = keyText;
            _label.isOrthographic = true;
            _label.textWrappingMode = TextWrappingModes.NoWrap;

            var mr = _label.GetComponent<MeshRenderer>();
            if (mr)
            {
                mr.sortingLayerID = layerId;
                mr.sortingOrder = sortingOrderBase + 1;
            }

            var p = _label.transform.localPosition;
            p.z = -0.001f;
            _label.transform.localPosition = p;

            _label.ForceMeshUpdate();
        }
    }

    void Reposition()
    {
        if (_root == null) return;

        Vector3 pos = transform.position;

        if (useColliderBottom)
        {
            var c = GetComponent<Collider>();
            if (c) pos.y = c.bounds.min.y;
            var c2 = GetComponent<Collider2D>();
            if (c2) pos.y = c2.bounds.min.y;
            pos.y += extraBottomPadding;
        }

        pos += (Vector3)(localOffset.x * (Vector2)transform.right + localOffset.y * (Vector2)transform.up);
        _root.position = pos;
    }

    void SetVisible(bool v)
    {
        if (_root) _root.gameObject.SetActive(v);
    }

    void EnsureWindowRef()
    {
        if (traitWindowRoot == null && autoFindByName && !string.IsNullOrEmpty(traitWindowName))
        {
            var go = GameObject.Find(traitWindowName);
            if (go) traitWindowRoot = go;
        }
    }

    void OpenWindow()
    {
        EnsureWindowRef();
        if (!traitWindowRoot) return;

        var cg = traitWindowRoot.GetComponent<CanvasGroup>();
        if (!cg) cg = traitWindowRoot.AddComponent<CanvasGroup>();

        traitWindowRoot.SetActive(true);
        traitWindowRoot.transform.SetAsLastSibling();
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;

        _windowOpen = true;
        SetVisible(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Canvas.ForceUpdateCanvases();
        var rt = traitWindowRoot.transform as RectTransform;
        if (rt) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    void CloseWindow()
    {
        if (!traitWindowRoot) { _windowOpen = false; if (_inside) SetVisible(true); return; }

        var cg = traitWindowRoot.GetComponent<CanvasGroup>();
        if (!cg) cg = traitWindowRoot.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        traitWindowRoot.SetActive(false);

        _windowOpen = false;
        if (_inside) SetVisible(true);
    }

    void CloseWindowImmediate() => CloseWindow();

    public static void CloseAllTraitWindows()
    {
#if UNITY_2023_1_OR_NEWER
        var prompts = Object.FindObjectsByType<NpcFPrompt>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
#else
    var prompts = Object.FindObjectsOfType<NpcFPrompt>(true); // includeInactive
#endif

        foreach (var p in prompts) p.CloseWindow();
    }

}
