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
    [SerializeField] int sortingOrderBase = 5000;
    [SerializeField] string keyText = "F";

    [Header("Trait Window (no controller)")]
    [SerializeField] GameObject traitWindowRoot;
    [SerializeField] bool autoFindByHierarchy = true;
    [SerializeField] string traitWindowPath = "TraitWindow";
    [SerializeField] bool autoFindByTag = false;
    [SerializeField] string traitWindowTag = "TraitWindow";

    [Header("Debug")]
    [SerializeField] bool alwaysShow = false;
    [SerializeField] bool logEvents = true;

    public UnityEvent OnInteract;

    Transform _root;
    SpriteRenderer _box;
    TextMeshPro _label;
    Renderer _labelRenderer;

    bool _inside;
    bool _windowOpen;
    int _overlapCount = 0;

    Collider _col3D;
    Collider2D _col2D;

    int _sortingLayerId;

    static Sprite _whiteSprite;
    static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                var c = Color.white;
                tex.SetPixels(new[] { c, c, c, c });
                tex.Apply();
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 100f);
                _whiteSprite.name = "RuntimeWhite";
            }
            return _whiteSprite;
        }
    }

    void Awake()
    {
        CacheComponents();
        EnsureVisuals();
        EnsureWindowRef();
        SetVisible(false);
        Reposition();
        CloseWindowImmediate();
        if ((_col2D == null && _col3D == null) && logEvents)
            Debug.LogWarning("[NpcFPrompt] 충돌체(2D/3D)가 없습니다.", this);
    }

    void CacheComponents()
    {
        TryGetComponent(out _col3D);
        TryGetComponent(out _col2D);
    }

    void OnValidate()
    {
        if (!Application.isPlaying) EnsureVisuals();
        ApplyVisuals();
        Reposition();
    }

    void OnTriggerEnter(Collider other) { if (_windowOpen) return; if (IsPlayer(other.gameObject.layer)) TryEnter(); }
    void OnTriggerExit(Collider other) { if (_windowOpen) return; if (IsPlayer(other.gameObject.layer)) TryExit(); }
    void OnTriggerEnter2D(Collider2D other) { if (_windowOpen) return; if (IsPlayer(other.gameObject.layer)) TryEnter(); }
    void OnTriggerExit2D(Collider2D other) { if (_windowOpen) return; if (IsPlayer(other.gameObject.layer)) TryExit(); }

    void TryEnter()
    {
        int prev = _overlapCount;
        _overlapCount++;
        if (prev == 0 && _overlapCount == 1) Enter();
    }

    void TryExit()
    {
        int prev = _overlapCount;
        _overlapCount = Mathf.Max(0, _overlapCount - 1);
        if (prev > 0 && _overlapCount == 0) Exit();
    }

    void Update()
    {
        if (alwaysShow) { SetVisible(true); Reposition(); }
        if (_inside && !_windowOpen && Input.GetKeyDown(interactKey))
        {
            if (logEvents) Debug.Log("[NpcFPrompt] Interact (F)!", this);
            OnInteract?.Invoke();
            OpenWindow();
        }
        if (_windowOpen && Input.GetKeyDown(closeKey))
            CloseWindow();
    }

    bool IsPlayer(int layer) => (playerLayers.value & (1 << layer)) != 0;

    void Enter()
    {
        _inside = true;
        if (logEvents) Debug.Log("[NpcFPrompt] Player ENTER", this);
        if (!_windowOpen) SetVisible(true);
        Reposition();
    }

    void Exit()
    {
        _inside = false;
        if (logEvents) Debug.Log("[NpcFPrompt] Player EXIT", this);
        if (!_windowOpen) SetVisible(false);
    }

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
            var existedBox = _root.Find("Box");
            var go = existedBox ? existedBox.gameObject : new GameObject("Box", typeof(SpriteRenderer));
            go.transform.SetParent(_root, false);
            _box = go.GetComponent<SpriteRenderer>();
            _box.sprite = WhiteSprite;
        }
        if (_label == null)
        {
            var existedLabel = _root.Find("Label");
            var go = existedLabel ? existedLabel.gameObject : new GameObject("Label", typeof(TextMeshPro));
            go.transform.SetParent(_root, false);
            _label = go.GetComponent<TextMeshPro>();
        }
        _labelRenderer = _label ? _label.GetComponent<Renderer>() : null;
        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        _sortingLayerId = SortingLayer.NameToID(sortingLayer);
        if (_box)
        {
            _box.color = boxColor;
            _box.sortingLayerID = _sortingLayerId;
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
            if (_labelRenderer is MeshRenderer mr)
            {
                mr.sortingLayerID = _sortingLayerId;
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
            if (_col3D) pos.y = _col3D.bounds.min.y;
            if (_col2D) pos.y = _col2D.bounds.min.y;
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
        if (traitWindowRoot) return;
        if (autoFindByHierarchy && !string.IsNullOrEmpty(traitWindowPath))
        {
            Transform t = transform.Find(traitWindowPath);
            if (!t)
            {
                var canvas = GetComponentInParent<Canvas>();
                if (canvas) t = canvas.transform.Find(traitWindowPath);
            }
            if (t)
            {
                traitWindowRoot = t.gameObject;
                return;
            }
        }
        if (autoFindByTag && !string.IsNullOrEmpty(traitWindowTag))
        {
            var go = GameObject.FindWithTag(traitWindowTag);
            if (go)
            {
                traitWindowRoot = go;
                return;
            }
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
        EnableTrigger(false);
        _inside = false;
        _overlapCount = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Canvas.ForceUpdateCanvases();
        var rt = traitWindowRoot.transform as RectTransform;
        if (rt) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    void CloseWindow()
    {
        if (!traitWindowRoot)
        {
            _windowOpen = false;
            if (_inside || alwaysShow) SetVisible(true);
            return;
        }
        var cg = traitWindowRoot.GetComponent<CanvasGroup>();
        if (!cg) cg = traitWindowRoot.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        traitWindowRoot.SetActive(false);
        _windowOpen = false;
        EnableTrigger(true);
        if (_inside || alwaysShow) SetVisible(true);
    }

    void EnableTrigger(bool enable)
    {
        if (_col3D) _col3D.enabled = enable;
        if (_col2D) _col2D.enabled = enable;
    }

    void CloseWindowImmediate() => CloseWindow();

    public static void CloseAllTraitWindows()
    {
#if UNITY_2023_1_OR_NEWER
        var prompts = Object.FindObjectsByType<NpcFPrompt>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        var prompts = Object.FindObjectsOfType<NpcFPrompt>(true);
#endif
        foreach (var p in prompts) p.CloseWindow();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var pos = transform.position;
        var c2 = _col2D ? _col2D : GetComponent<Collider2D>();
        if (c2) pos.y = c2.bounds.min.y + extraBottomPadding;
        pos += (Vector3)(localOffset.x * (Vector2)transform.right + localOffset.y * (Vector2)transform.up);
        Gizmos.DrawSphere(pos, 0.05f);
    }
}
