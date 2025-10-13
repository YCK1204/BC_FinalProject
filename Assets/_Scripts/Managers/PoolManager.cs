using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    Stack<GameObject> _items = new Stack<GameObject>();
    Transform _root;
    GameObject _prefab;
    public void Init(int count, Transform root, GameObject prefab)
    {
        _root = root;
        _prefab = prefab;
        for (int i = 0; i < count; i++)
        {
            var go = Object.Instantiate(prefab);
            go.gameObject.SetActive(false);
            go.transform.parent = root;
            _items.Push(go);
        }
    }
    public GameObject Pop()
    {
        if (_items.Count > 0)
        {
            var item = _items.Pop();
            item.gameObject.SetActive(true);
            return item;
        }
        GameObject go = Object.Instantiate(_prefab);
        go.gameObject.SetActive(true);
        go.transform.parent = _root;
        return go;
    }
    public void Push(GameObject item)
    {
        item.gameObject.SetActive(false);
        _items.Push(item);
    }
}

public class PoolManager
{
    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
    GameObject _root;
    public void Init()
    {
        _root = new GameObject("@Pool");
        Object.DontDestroyOnLoad(_root);
    }
    public void CreatePool<T>(int count, GameObject prefab) where T : MonoBehaviour
    {
        string key = typeof(T).Name;
        if (_pools.ContainsKey(key)) return;

        Pool pool = new Pool();
        pool.Init(count, _root.transform, prefab);
        _pools.Add(key, pool);
    }
    public T Pop<T>() where T : MonoBehaviour
    {
        string key = typeof(T).Name;
        if (_pools.ContainsKey(key) == false) return null;
        Pool pool = _pools[key];
        return pool.Pop().GetComponent<T>();
    }
    public void Push<T>(GameObject item) where T : MonoBehaviour
    {
        string key = typeof(T).Name;
        if (_pools.ContainsKey(key) == false) return;
        Pool pool = _pools[key];
        pool.Push(item);
    }
    public void Clear()
    {
        _pools.Clear();
    }
}