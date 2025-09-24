using Unity.VisualScripting;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    ItemContainer ItemContainer;
    [SerializeField]
    Vector2 ContainerOffset;
    ItemData ItemData;
    SpriteRenderer _spriteRenderer;

    [SerializeField]
    Vector2 ColliderScale;

    Canvas _canvas;

    void Init(ItemContainer container)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 캔버스 연결 리팩토링 필요
        _canvas = GameObject.Find("CamCanvas").GetComponent<Canvas>();
        ItemContainer = Manager.Resource.Instantiate(container);
        ItemContainer.gameObject.SetActive(false);
        ItemContainer.gameObject.transform.SetParent(_canvas.transform);
        var rectTransform = ItemContainer.gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = (Vector2)transform.position + ContainerOffset;
    }
    public void SetData(ItemData data, ItemContainer containerPrefab)
    {
        Init(containerPrefab);

        ItemData = data;

        //_spriteRenderer.sprite = data.ItemIcon;
        ItemContainer.SetUI(ItemData);
        var boxCollider = GetComponent<BoxCollider2D>();
        Vector2 size = _spriteRenderer.bounds.size;
        boxCollider.size = size * ColliderScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ItemContainer.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ItemContainer.gameObject.SetActive(false);
        }
    }
}