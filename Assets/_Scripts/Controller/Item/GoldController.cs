using Game.Player;
using UnityEngine;

public class GoldController : InteractableController
{
    [SerializeField]
    int Gold;
    [SerializeField]
    GameObject GoldUIPrefab;
    [SerializeField]
    Vector3 UIOffset;

    GameObject _goldUI;
    public override void OnInteract()
    {
        PlayerCharacter.Instance.Inventory.Gold += Gold;
        Debug.Log(PlayerCharacter.Instance.Inventory.GetJsonString());
        Destroy(gameObject);
    }
    protected override void Init()
    {
        base.Init();
        var _goldUI = Instantiate(GoldUIPrefab);
        _goldUI.transform.position = transform.position + UIOffset;
        _goldUI.transform.SetParent(transform);
        _goldUI.SetActive(false);
        onEnterTrigger = () =>
        {
            PlayerCharacter.Instance.Interactables.Add(this);
            _goldUI.SetActive(true);
        };
        onExitTrigger = () =>
        {
            PlayerCharacter.Instance.Interactables.Remove(this);
            _goldUI.SetActive(false);
        };
    }
}
