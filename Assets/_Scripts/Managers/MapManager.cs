using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MapSaveData
{
    public int CurrentMapIndex;
    public List<int> ClearedMapIndices;
    public float PlayTime;
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private Transform _player;
    [SerializeField] private CinemachineCamera _cam;
    public GameObject CameraObj;

    [Header("Map")]
    [SerializeField] private List<GameObject> _mapPrefabs;
    [SerializeField] private GameObject _bossroomPrefabs;
    [SerializeField] private GameObject _itemRoomPrefab;

    private List<GameObject> _mapPool = new List<GameObject>();
    private GameObject _currentMap;
    private GameObject _currentMapPrefab;

    private int _roomCount = 0;
    [SerializeField] private int _bossRoomTrigger = 4;

    private List<int> _clearedMaps = new List<int>();

    public bool OnPortal = false;

    private float _playTime = 0f;
    private bool _isTimerOn = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitFloor();
    }

    private void Update()
    {
        if (_isTimerOn)
        {
            _playTime += Time.deltaTime;
        }
    }


    public void InitFloor()
    {
        _clearedMaps.Clear();
        UpdateRoomCount();
        ResetMaps();
        LoadMap(_mapPrefabs[0]);
        OnPortal = true;
        _playTime = 0f;
        _isTimerOn = false;
    }

    private void ResetMaps()
    {
        _mapPool.Clear();
        for (int i = 1; i < _mapPrefabs.Count; i++)
        {
            if (_clearedMaps.Contains(i)) continue;
            _mapPool.Add(_mapPrefabs[i]);
        }
    }

    IEnumerator ItemStageTrigger()
    {
        yield return null;
        yield return null;
        Manager.Game.MonsterCount = 0;
    }

    public void NextMap()
    {
        if(_currentMapPrefab == _mapPrefabs[0])
        {
            LoadMap(_itemRoomPrefab);
            //Manager.Game.MonsterCount = 0;
            StartCoroutine(ItemStageTrigger());
            return;
        }

        if (_currentMapPrefab != null)
        {
            int clearedIndex = _mapPrefabs.IndexOf(_currentMapPrefab);
            if (clearedIndex > 0 && !_clearedMaps.Contains(clearedIndex))
            {
                _clearedMaps.Add(clearedIndex);
            }
        }

        UpdateRoomCount();
        OnPortal = false;

        if (_roomCount == 1 && !_isTimerOn)
        {
            _isTimerOn = true;
            Debug.Log("플레이타임기록");
        }

        if (_roomCount == _bossRoomTrigger)
        {
            LoadMap(_bossroomPrefabs);
            Debug.Log("보스방 입장!");
            return;
        }

        if (_mapPool.Count > 0)
        {
            int poolIndex = Random.Range(0, _mapPool.Count);
            GameObject prefab = _mapPool[poolIndex];
            _mapPool.RemoveAt(poolIndex);

            LoadMap(prefab);
            Debug.Log(prefab.name + " 남은맵:" + _mapPool.Count);
        }
        else
        {
            _clearedMaps.Clear();
            UpdateRoomCount();
            ResetMaps();
            LoadMap(_mapPrefabs[0]);
            Debug.Log("리셋");
        }
    }

    private void LoadMap(GameObject prefab)
    {
        if (_currentMap != null)
            Destroy(_currentMap);

        _currentMapPrefab = prefab;
        _currentMap = Instantiate(prefab, transform);

        //콜라이더 초기화
        var colliderTransform = _currentMap.transform.Find("Collider");
        if (colliderTransform != null)
        {
            colliderTransform.gameObject.SetActive(false);
            colliderTransform.gameObject.SetActive(true);
        }

        MovePlayerSpawn(_currentMap);

        //포탈 off
        OnPortal = false;
    }

    private void MovePlayerSpawn(GameObject map)
    {
        Transform spawnPoint = map.transform.Find("SpawnPoint");
        if (spawnPoint != null)
        {
            _player.position = spawnPoint.position;
            _player.rotation = spawnPoint.rotation;
            _cam.ForceCameraPosition(
                new Vector3(spawnPoint.position.x, spawnPoint.position.y, _cam.transform.position.z),
                Quaternion.identity
            );
        }
        else Debug.LogError(map.name + "!! 스폰포인트 없음");
    }

    private void UpdateRoomCount()
    {
        _roomCount = _clearedMaps.Count;
    }

    public void SetPortal()
    {
        OnPortal = true;
        Debug.Log("포탈 열림");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitFloor();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public MapSaveData GetSaveData()
    {
        MapSaveData data = new MapSaveData();

        if (_currentMapPrefab == _bossroomPrefabs)
        {
            data.CurrentMapIndex = -1;
        }
        else if (_currentMapPrefab != null)
        {
            data.CurrentMapIndex = _mapPrefabs.IndexOf(_currentMapPrefab);
        }
        else
        {
            data.CurrentMapIndex = -99;
        }

        data.ClearedMapIndices = new List<int>(_clearedMaps);
        data.PlayTime = _playTime;
        return data;
    }

    public void LoadFromData(MapSaveData data)
    {
        _clearedMaps = new List<int>(data.ClearedMapIndices);
        UpdateRoomCount();
        _playTime = data.PlayTime;

        if (_roomCount > 0)
        {
            _isTimerOn = true;
        }

        ResetMaps();

        int mapIndex = data.CurrentMapIndex;
        GameObject mapToLoad = null;

        if (mapIndex == -1)
        {
            mapToLoad = _bossroomPrefabs;
        }
        else if (mapIndex >= 0 && mapIndex < _mapPrefabs.Count)
        {
            mapToLoad = _mapPrefabs[mapIndex];
        }

        if (mapToLoad != null)
        {
            LoadMap(mapToLoad);
            Debug.Log("맵: " + mapToLoad.name);
        }
        else
        {
            Debug.LogWarning("저장된 맵 없음");
            LoadMap(_mapPrefabs[0]);
        }
    }
}