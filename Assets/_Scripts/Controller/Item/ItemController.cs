using Unity.VisualScripting;
using UnityEngine;

public class ItemController : MonoBehaviour
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
        // 렌더러에 설정된 아이템 이미지에 따라 콜라이더 크기 조정
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _itemContainer.gameObject.SetActive(true);
            Manager.Item.OnTriggerEnterItem(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _itemContainer.gameObject.SetActive(false);
            Manager.Item.OnTriggerExitItem(this);
        }
    }
}