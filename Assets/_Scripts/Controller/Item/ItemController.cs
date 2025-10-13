using Game.Player;
using Unity.VisualScripting;
using UnityEngine;

public class ItemController : InteractableController
{
    public ItemData ItemData { get; private set; }
    [SerializeField]
    Vector2 ContainerOffset = new Vector2(0f, 3f);

    [SerializeField]
    Vector2 ColliderScale;
    [SerializeField]
    ItemContainer _itemContainer;
    SpriteRenderer _spriteRenderer;

    public void SetData(ItemData data)
    {
        ItemData = data;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _itemContainer.SetUI(ItemData);
        _itemContainer.transform.position = (Vector2)transform.position + ContainerOffset;
        _itemContainer.transform.parent = transform;
        _itemContainer.gameObject.SetActive(false);
        var boxCollider = GetComponent<BoxCollider2D>();
        Vector2 size = _spriteRenderer.bounds.size;
        boxCollider.size = size * ColliderScale;
    }
    public void SetSprite(Sprite sprite)
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = sprite;
    }
    protected override void Init()
    {
        base.Init();
        Type = InteractableType.Item;
        onEnterTrigger = () =>
        {
            PlayerCharacter.Instance.Interactables.Add(this);
            _itemContainer.gameObject.SetActive(true);
            Manager.Item.OnTriggerEnterItem(this);
        };
        onExitTrigger = () =>
        {
            PlayerCharacter.Instance.Interactables.Remove(this);
            _itemContainer.gameObject.SetActive(false);
            Manager.Item.OnTriggerExitItem(this);
        };
    }

    public override void OnInteract()
    {
        Manager.Item.AddItem(PlayerCharacter.Instance);
    }
}