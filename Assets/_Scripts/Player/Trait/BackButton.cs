using UnityEngine;
using UnityEngine.UI;
using Game.Traits.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TraitWindowController _controller;

    void Awake()
    {
        if (_button == null) _button = GetComponent<Button>();
    }

    void OnEnable()
    {
        if (_button != null) _button.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        if (_button != null) _button.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        var controller = _controller != null ? _controller : TraitWindowController.Instance;
        if (controller != null) controller.Close();
    }
}
