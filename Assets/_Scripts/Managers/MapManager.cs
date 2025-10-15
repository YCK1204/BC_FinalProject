using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MapSaveData
{
    public string CurrentMapName;
    public List<string> ClearedMaps;
    public int RoomCount;
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

    private List<GameObject> _mapPool = new List<GameObject>();
    private GameObject _currentMap;
    private GameObject _currentMapPrefab;

    private int _roomCount = 0;
    [SerializeField] private int _bossRoomTrigger = 4;

    private List<string> _clearedMaps = new List<string>();

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
    }

    private void Update()
    {
        if (_isTimerOn)
        {
            _playTime += Time.deltaTime;
            Debug.Log(_playTime);
        }
    }

    private void Start()
    {
        //초기화
        InitFloor();
    }


    public void InitFloor()
    {
        _roomCount = 0;
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
            string mapName = _mapPrefabs[i].name;
            if (_clearedMaps.Contains(mapName)) continue;
            _mapPool.Add(_mapPrefabs[i]);
        }
    }

    public void NextMap()
    {
        if (_currentMapPrefab != null)
            _clearedMaps.Add(_currentMapPrefab.name.Replace("(Clone)", ""));

        _roomCount++;
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
            int index = Random.Range(0, _mapPool.Count);
            GameObject prefab = _mapPool[index];
            _mapPool.RemoveAt(index);

            LoadMap(prefab);
            Debug.Log(prefab.name + " 남은맵:" + _mapPool.Count);
        }
        else
        {
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
            PlayerManager.Instance.Player.StartRound();
        }
        else Debug.LogError(map.name + "!! 스폰포인트 없음");
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
        data.CurrentMapName = _currentMapPrefab != null ? _currentMapPrefab.name : "None";
        data.ClearedMaps = new List<string>(_clearedMaps);
        data.RoomCount = _roomCount;
        data.PlayTime = _playTime;
        return data;
    }

    public void LoadFromData(MapSaveData data)
    {
        _roomCount = data.RoomCount;
        _clearedMaps = new List<string>(data.ClearedMaps);
        _playTime = data.PlayTime;

        if(_roomCount > 0)
        {
            _isTimerOn = true;
        }

        ResetMaps();

        GameObject found = _mapPrefabs.Find(p => p.name == data.CurrentMapName);
        if (found != null)
        {
            LoadMap(found);
            Debug.Log("맵: " + found.name);
        }
        else
        {
            if (_bossroomPrefabs != null && _bossroomPrefabs.name == data.CurrentMapName)
            {
                LoadMap(_bossroomPrefabs);
                Debug.Log("맵: " + _bossroomPrefabs.name);
            }
            else
            {
                Debug.LogWarning("저장된 맵 없음");
                LoadMap(_mapPrefabs[0]);
            }
        }
    }
}