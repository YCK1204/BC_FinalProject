using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private Transform _player;
    [SerializeField] private CinemachineCamera _cam;

    [Header("Map")]
    [SerializeField] private List<GameObject> _mapPrefabs;

    private List<GameObject> _mapInstances = new List<GameObject>();
    private List<GameObject> _mapPool = new List<GameObject>();
    private GameObject _currentMap;

    public bool OnPortal = false;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);
    }

    void Start()
    {
        foreach (GameObject prefab in _mapPrefabs)
        {
            GameObject map = Instantiate(prefab, transform);
            map.SetActive(false);
            _mapInstances.Add(map);
        }

        ResetMaps();
        LoadMap(_mapInstances[0]);
    }

    private void ResetMaps()
    {
        _mapPool.Clear();
        for (int i = 1; i < _mapInstances.Count; i++) _mapPool.Add(_mapInstances[i]);
    }

    public void NextMap()
    {
        if (_mapPool.Count > 0)
        {
            int index = Random.Range(0, _mapPool.Count);
            GameObject nextMap = _mapPool[index];
            _mapPool.RemoveAt(index);

            LoadMap(nextMap);
            Debug.Log(nextMap.name + "남은맵:" + _mapPool.Count);
        }
        else
        {
            ResetMaps();
            LoadMap(_mapInstances[0]);
            Debug.Log("맵 없음");
        }
    }

    private void LoadMap(GameObject map)
    {
        if (_currentMap != null) 
            _currentMap.SetActive(false);

        _currentMap = map;
        _currentMap.SetActive(true);
        MovePlayerSpawn(_currentMap);

        OnPortal = false;
    }

    private void MovePlayerSpawn(GameObject map)
    {
        Transform spawnPoint = map.transform.Find("SpawnPoint");
        if (spawnPoint != null)
        {
            _player.position = spawnPoint.position;
            _player.rotation = spawnPoint.rotation;
            _cam.ForceCameraPosition(new Vector3(spawnPoint.position.x, spawnPoint.position.y, _cam.transform.position.z), Quaternion.identity);
        }
        else Debug.LogError(map.name + "!!스폰포인트 없음");
    }

    public void SetPortal()
    {
        OnPortal = true;
        Debug.Log("포탈 열림");
    }
}
