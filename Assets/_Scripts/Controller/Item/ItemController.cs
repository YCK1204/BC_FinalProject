using Game.Player;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemController : InteractableController
{
    public ItemData ItemData { get; private set; }
    [SerializeField]
    Vector2 ContainerOffset = new Vector2(0f, 3f);

    [SerializeField]
    Vector2 ColliderScale;
    //[SerializeField]
    //ItemContainer _itemContainer;
    SpriteRenderer _spriteRenderer;

    private ItemFieldUI _itemFieldUI;

    [SerializeField] private GameObject _itemText;

    public void SetData(ItemData data)
    {
        ItemData = data;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //_itemContainer.SetUI(ItemData);
        //_itemContainer.transform.position = (Vector2)transform.position + ContainerOffset;
        //_itemContainer.transform.parent = transform;
        //_itemContainer.gameObject.SetActive(false);
        var boxCollider = GetComponent<BoxCollider2D>();
        Vector2 size = _spriteRenderer.bounds.size;
        boxCollider.size = size * ColliderScale;
    }
    public void SetSprite(Sprite sprite)
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.material = ItemData.Material;

        _itemFieldUI = DisplayManager.Instance.ItemFieldUI;
        _itemText = DisplayManager.Instance.ItemText;
    }
    protected override void Init()
    {
        base.Init();
        Type = InteractableType.Item;
        onEnterTrigger = () =>
        {
            _itemFieldUI.ShowTooltip(ItemData);

            PlayerCharacter.Instance.Interactables.Add(this);
            _itemText.SetActive(true);
            PlayerCharacter.Instance.Inventory.EnterCurItem(this);
        };
        onExitTrigger = () =>
        {
            _itemFieldUI.HideTooltip();

            PlayerCharacter.Instance.Interactables.Remove(this);
            _itemText.SetActive(false);
            PlayerCharacter.Instance.Inventory.ExitCurItem(this);
        };
    }

    public override void OnInteract()
    {
        var player = PlayerCharacter.Instance;
        player.Inventory.AddItem(player);
    }
    private void OnDestroy()
    {
        PlayerCharacter.Instance.Interactables.Remove(this);
    }
}