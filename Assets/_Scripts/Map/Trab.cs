using Game.Monster;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trap_ToggleCollider : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    private float damagetime = 0.1f;

    private Collider2D _trapCollider;

    private bool _isWaiting = false;

    private void Awake()
    {
        _trapCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (_isWaiting)
            return;

        if (collision.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);

            //코루틴으로 껏다 키기
            StartCoroutine(TrabCrt());
        }
    }

    private IEnumerator TrabCrt()
    {

        //콜라이더를 껏다켜서 OnTriggerEnter2D를 유도
        _isWaiting = true;
        _trapCollider.enabled = false;

        yield return new WaitForSeconds(damagetime);

        _trapCollider.enabled = true;
        _isWaiting = false;
    }
}