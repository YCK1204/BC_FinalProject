using UnityEngine;
using DamageNumbersPro;

public class DamageIndicator : MonoBehaviour
{
    public static DamageIndicator Instance { get; private set; }

    public DamageNumber numberPrefab;

    private void Awake()
    {
        // 싱글턴 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GetDamage(Vector3 pos, float damage)
    {
        if (numberPrefab == null)
            return;

        DamageNumber damageNumber = numberPrefab.Spawn(pos, damage);
    }
}
