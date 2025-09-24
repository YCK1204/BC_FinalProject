using UnityEngine;
using Game.Traits.UI;

public class NpcInteractable : MonoBehaviour
{
    [SerializeField] LayerMask _playerLayers;   // Player 레이어만 체크
    [SerializeField] KeyCode _key = KeyCode.F;
    [SerializeField] GameObject _fIcon;

    bool _inside;

    bool IsPlayerLayer(int layer) => (_playerLayers.value & (1 << layer)) != 0;

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            _inside = true;
            if (_fIcon) _fIcon.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            _inside = false;
            if (_fIcon) _fIcon.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            _inside = true;
            if (_fIcon) _fIcon.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayerLayer(other.gameObject.layer))
        {
            _inside = false;
            if (_fIcon) _fIcon.SetActive(false);
        }
    }

    void Update()
    {
        if (_inside && Input.GetKeyDown(_key))
            TraitWindowController.Instance?.Open();
    }
}
