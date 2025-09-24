using TMPro;
using UnityEngine;
using Game.Traits;

public class SoulTextUI : MonoBehaviour
{
    [SerializeField] private SoulWallet _wallet;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private string format = "{0}";

    void Reset()
    {
        _text = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        if (_wallet == null)
#if UNITY_2023_1_OR_NEWER
            _wallet = Object.FindFirstObjectByType<SoulWallet>();
#else
            _wallet = FindObjectOfType<SoulWallet>();
#endif

        if (_wallet != null) _wallet.OnSoulChanged += UpdateText;
        UpdateText(_wallet != null ? _wallet.CurrentSoul : 0);
    }

    void OnDisable()
    {
        if (_wallet != null) _wallet.OnSoulChanged -= UpdateText;
    }

    void UpdateText(int soul)
    {
        if (_text == null) return;
        _text.alignment = TextAlignmentOptions.Center;
        _text.text = string.Format(format, soul);
    }
}
