using Unity.VisualScripting;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemData ItemData { get; private set; }

    [SerializeField]
    Vector2 ColliderScale;
    
    public void SetData(ItemData data)
    {
        ItemData = data;
        // 렌더러에 설정된 아이템 이미지에 따라 콜라이더 크기 조정
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.ItemIcon;
        var boxCollider = GetComponent<BoxCollider2D>();
        Vector2 size = spriteRenderer.bounds.size;
        boxCollider.size = size * ColliderScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Manager.Item.ShowItemInfo(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Manager.Item.HideItemInfo(this);
        }
    }
}