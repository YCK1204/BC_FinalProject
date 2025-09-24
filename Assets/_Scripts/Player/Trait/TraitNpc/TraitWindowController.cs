using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Traits.UI
{
    public class TraitWindowController : MonoBehaviour
    {
        public static TraitWindowController Instance { get; private set; }

        [SerializeField] GameObject _root;
        [SerializeField] KeyCode _closeKey = KeyCode.Escape;

        CanvasGroup _group;
        RectTransform _rt;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            AutoWireRootIfMissing();
            EnsureGroup();
            CloseImmediate();
        }

        void Update()
        {
            if (_root != null && _root.activeSelf && Input.GetKeyDown(_closeKey))
                Close();
        }

        void AutoWireRootIfMissing()
        {
            if (_root != null) { _rt = _root.transform as RectTransform; return; }
            var t = transform.Find("TraitWindow");
            if (t != null) _root = t.gameObject;
            if (_root == null) _root = GameObject.Find("TraitWindow");
            _rt = _root ? _root.transform as RectTransform : null;
        }

        void EnsureGroup()
        {
            if (_root == null) return;
            _group = _root.GetComponent<CanvasGroup>();
            if (_group == null) _group = _root.AddComponent<CanvasGroup>();
        }

        public void Open()
        {
            if (_root == null) { AutoWireRootIfMissing(); if (_root == null) return; }

            _root.SetActive(true);
            _root.transform.SetAsLastSibling();

            EnsureGroup();
            _group.alpha = 1f;
            _group.blocksRaycasts = true;
            _group.interactable = true;

            StopAllCoroutines();
            StartCoroutine(RebuildLayoutTwice());

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        IEnumerator RebuildLayoutTwice()
        {
            Canvas.ForceUpdateCanvases();
            if (_rt != null) LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
            yield return null;
            Canvas.ForceUpdateCanvases();
            if (_rt != null) LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
        }

        public void Close()
        {
            if (_root == null) return;
            EnsureGroup();
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _group.interactable = false;
            _root.SetActive(false);
        }

        public void CloseImmediate() => Close();
    }
}
