using System.Collections;
using UnityEngine;

public class EffectReturn : MonoBehaviour
{
    [HideInInspector]
    public string OriginalPrefabName;

    [Header("returnTime")]
    public float AutoReturnTime = 0f;

    private Coroutine _returnCoroutine;

    private void OnEnable()
    {
        if (AutoReturnTime > 0)
        {
            if (_returnCoroutine != null)
                StopCoroutine(_returnCoroutine);

            _returnCoroutine = StartCoroutine(AutoReturnCoroutine());
        }
    }

    private IEnumerator AutoReturnCoroutine()
    {
        yield return new WaitForSeconds(AutoReturnTime);
        Return();
    }

    public void Return()
    {
        if (!string.IsNullOrEmpty(OriginalPrefabName))
        {
            PlayerPool.Instance.ReturnPoolName(OriginalPrefabName, this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
