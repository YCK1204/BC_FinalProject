using System.Collections.Generic;
using UnityEngine;

public class PlayerPool : MonoBehaviour
{
    public static PlayerPool Instance;

    private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> _prefabReferences = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string prefabName = prefab.name;

        if (!_poolDictionary.ContainsKey(prefabName))
        {
            _poolDictionary.Add(prefabName, new Queue<GameObject>());
            _prefabReferences.Add(prefabName, prefab);
        }

        Queue<GameObject> pool = _poolDictionary[prefabName];

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab, position, rotation);

            EffectReturn effectReturn = obj.GetComponent<EffectReturn>();
            if (effectReturn != null)
            {
                effectReturn.OriginalPrefabName = prefabName;
            }
            return obj;
        }
    }

    public void ReturnPoolName(string prefabName, GameObject obj)
    {
        if (_poolDictionary.ContainsKey(prefabName))
        {
            obj.SetActive(false);
            _poolDictionary[prefabName].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

}
