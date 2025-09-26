using UnityEngine;
using UnityEngine.UI;
using Game.Traits.UI;   // ← NpcFPrompt 네임스페이스 추가

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    Button _btn;

    void Awake() => _btn = GetComponent<Button>();
    void OnEnable() => _btn.onClick.AddListener(OnClick);
    void OnDisable() => _btn.onClick.RemoveListener(OnClick);

    void OnClick()
    {
        NpcFPrompt.CloseAllTraitWindows();
    }
}
