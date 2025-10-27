using System.Collections;
using UnityEngine;

public class EffectReturn : MonoBehaviour
{
    public float AutoReturnTime = 0f;

    private PlayerPool _parentPool;
    private GameObject _originalPrefab;

    private Coroutine _returnCoroutine;

    public void SetPool(PlayerPool pool, GameObject originalPrefab)
    {
        _parentPool = pool;
        _originalPrefab = originalPrefab;
        TryReturnCoroutine();
    }

    private void OnEnable()
    {
        TryReturnCoroutine();
    }

    private void TryReturnCoroutine()
    {
        if (_parentPool == null || _originalPrefab == null || AutoReturnTime <= 0)
        {
            return;
        }

        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
        }
        _returnCoroutine = StartCoroutine(AutoReturnCoroutine());
    }

    private IEnumerator AutoReturnCoroutine()
    {
        yield return new WaitForSeconds(AutoReturnTime);
        Return();
    }

    public void Return()
    {
        if (_parentPool != null && _originalPrefab != null)
        {
            _parentPool.ReturnToPool(_originalPrefab, this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _returnCoroutine = null;
    }

    private void OnDisable()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
    }
}