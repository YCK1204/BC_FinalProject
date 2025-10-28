using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Trait Window")]
    [SerializeField] GameObject traitWindowRoot;

    [Header("Debug")]
    [SerializeField] bool alwaysShow = false;
    [SerializeField] bool logEvents = true;

    public UnityEvent OnInteract;

    Transform _root;
    SpriteRenderer _box;
    TextMeshPro _label;
    MeshRenderer _labelMr;

    Collider2D _col2D;

    bool _inside;
    bool _windowOpen;
    bool _suspendTriggers;

    int _sortingLayerId;

    static Sprite _whiteSprite;
    static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Repeat;
                var c = new Color32(255, 255, 255, 255);
                var arr = new Color32[16];
                for (int i = 0; i < arr.Length; i++) arr[i] = c;
                tex.SetPixels32(arr);
                tex.Apply(false, true);
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
                _whiteSprite.name = "RuntimeWhite_4x4";
            }
            return _whiteSprite;
        }
    }

    Collider2D[] _overlapBuf = new Collider2D[8];

    void Awake()
    {
        TryGetComponent(out _col2D);
        EnsureVisuals();
        ApplyVisuals();
        Reposition();

        _inside = false;
        _windowOpen = false;
        _suspendTriggers = false;

        SetVisible(alwaysShow);

        if (!_col2D && logEvents)
            Debug.LogWarning("[NpcFPrompt] Collider2D가 필요합니다.", this);
    }

    void OnEnable()
    {
        _inside = false;
        _windowOpen = false;
        _suspendTriggers = false;

        SetVisible(alwaysShow);
        StartCoroutine(InitialOverlapNextFrame());
    }

    IEnumerator InitialOverlapNextFrame()
    {
        yield return new WaitForFixedUpdate();
        SnapshotOverlap();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_suspendTriggers) return;
        if (!IsPlayer(other.gameObject.layer)) return;
        if (!_inside) Enter();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (_suspendTriggers) return;
        if (!IsPlayer(other.gameObject.layer)) return;
        if (_inside) Exit();
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
        if (!_windowOpen && !alwaysShow) SetVisible(false);
    }

    void EnsureVisuals()
    {
        if (_root == null)
        {
            var t = transform.Find("FPrompt");
            _root = t ? t : new GameObject("FPrompt").transform;
            _root.SetParent(transform, false);
            _root.gameObject.layer = gameObject.layer;
        }

        //if (_box == null)
        //{
        //    var t = _root.Find("Box");
        //    var go = t ? t.gameObject : new GameObject("Box", typeof(SpriteRenderer));
        //    go.transform.SetParent(_root, false);
        //    go.layer = _root.gameObject.layer;
        //    _box = go.GetComponent<SpriteRenderer>();
        //    _box.sprite = WhiteSprite;
        //}

        //if (_label == null)
        //{
        //    var t = _root.Find("Label");
        //    var go = t ? t.gameObject : new GameObject("Label", typeof(TextMeshPro));
        //    go.transform.SetParent(_root, false);
        //    go.layer = _root.gameObject.layer;
        //    _label = go.GetComponent<TextMeshPro>();
        //    _labelMr = _label.GetComponent<MeshRenderer>();
        //}
        else
        {
            _labelMr = _label.GetComponent<MeshRenderer>();
        }
    }

    void ApplyVisuals()
    {
        _sortingLayerId = SortingLayer.NameToID(sortingLayer);

        if (_box)
        {
            var col = boxColor; col.a = 1f;
            _box.color = col;
            _box.sortingLayerID = _sortingLayerId;
            _box.sortingOrder = sortingOrderBase;

            float sx = Mathf.Clamp(boxSize.x, 0.02f, 50f);
            float sy = Mathf.Clamp(boxSize.y, 0.02f, 50f);
            _box.transform.localScale = new Vector3(sx, sy, 1f);
        }

        if (_label)
        {
            _label.font = font ? font : TMP_Settings.defaultFontAsset;
            _label.enableAutoSizing = false;
            _label.fontSize = fontSize;
            _label.alignment = TextAlignmentOptions.Center;
            _label.color = textColor;
            _label.text = keyText;
            _label.isOrthographic = true;
            _label.textWrappingMode = TextWrappingModes.NoWrap;

            if (_labelMr)
            {
                _labelMr.sortingLayerID = _sortingLayerId;
                _labelMr.sortingOrder = sortingOrderBase + 1;
            }

            var p = _label.transform.localPosition; p.z = -0.001f;
            _label.transform.localPosition = p;
            _label.ForceMeshUpdate();
        }
    }

    void Reposition()
    {
        if (_root == null) return;
        Vector3 pos = transform.position;

        if (useColliderBottom && _col2D)
        {
            pos.y = _col2D.bounds.min.y + extraBottomPadding;
        }

        pos += (Vector3)(localOffset.x * (Vector2)transform.right + localOffset.y * (Vector2)transform.up);
        _root.position = pos;
    }

    void SetVisible(bool v)
    {
        if (_root) _root.gameObject.SetActive(v);
    }

    void OpenWindow()
    {
        if (!traitWindowRoot) return;
        PlayerManager.Instance.HUBSet(false);
        PlayerManager.Instance.Player.OnUI = true;
        PlayerManager.Instance.Player.SetPlayerInput(false);

        var cg = traitWindowRoot.GetComponent<CanvasGroup>();
        if (!cg) cg = traitWindowRoot.AddComponent<CanvasGroup>();

        traitWindowRoot.SetActive(true);
        traitWindowRoot.transform.SetAsLastSibling();
        cg.alpha = 1f; cg.blocksRaycasts = true; cg.interactable = true;

        _windowOpen = true;
        _suspendTriggers = true;
        SetVisible(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Canvas.ForceUpdateCanvases();
        var rt = traitWindowRoot.transform as RectTransform;
        if (rt) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    void CloseWindow()
    {
        PlayerManager.Instance.HUBSet(true);
        PlayerManager.Instance.Player.OnUI = false;
        PlayerManager.Instance.Player.SetPlayerInput(true);

        if (traitWindowRoot)
        {
            var cg = traitWindowRoot.GetComponent<CanvasGroup>();
            if (!cg) cg = traitWindowRoot.AddComponent<CanvasGroup>();
            cg.alpha = 0f; cg.blocksRaycasts = false; cg.interactable = false;
            traitWindowRoot.SetActive(false);
        }

        _windowOpen = false;
        _suspendTriggers = false;

        SnapshotOverlap();
    }

    void SnapshotOverlap()
    {
        bool any = false;

        if (_col2D)
        {
            var b = _col2D.bounds;
#if UNITY_6000_0_OR_NEWER
            var filter = new ContactFilter2D { useLayerMask = true, layerMask = playerLayers };
            int n = Physics2D.OverlapBox(b.center, b.size, 0f, filter, _overlapBuf);
            any = n > 0;
#else
            int n = Physics2D.OverlapBoxNonAlloc(b.center, b.size, 0f, _overlapBuf, playerLayers);
            any = n > 0;
#endif
        }

        _inside = any;
        if (!_windowOpen)
            SetVisible(any || alwaysShow);

        Reposition();
    }

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
        var c2 = GetComponent<Collider2D>();
        if (c2) pos.y = c2.bounds.min.y + extraBottomPadding;
        pos += (Vector3)(localOffset.x * (Vector2)transform.right + localOffset.y * (Vector2)transform.up);
        Gizmos.DrawSphere(pos, 0.05f);
    }
}
