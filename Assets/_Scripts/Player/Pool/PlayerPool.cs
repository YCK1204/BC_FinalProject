using System.Collections.Generic;
using UnityEngine;

public class PlayerPool : MonoBehaviour
{
    public static PlayerPool Instance { get; private set; }

    private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string prefabName = prefab.name;

        if (!_poolDictionary.ContainsKey(prefabName))
        {
            _poolDictionary.Add(prefabName, new Queue<GameObject>());
        }

        Queue<GameObject> pool = _poolDictionary[prefabName];
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            obj = Instantiate(prefab, position, rotation, transform);

            EffectReturn effectReturn = obj.GetComponent<EffectReturn>();
            if (effectReturn != null)
            {
                effectReturn.SetPool(this, prefab);
            }
            return obj;
        }
    }

    public void ReturnToPool(GameObject originalPrefab, GameObject obj)
    {
        string prefabName = originalPrefab.name;

        if (_poolDictionary.ContainsKey(prefabName))
        {
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            _poolDictionary[prefabName].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}