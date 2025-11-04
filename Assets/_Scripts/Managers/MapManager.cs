using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    [Header("UI")]
    [SerializeField] private Text _timerText;
    [SerializeField] private Text _portalStatusText;

    private List<GameObject> _mapPool = new List<GameObject>();
    private GameObject _currentMap;
    private GameObject _currentMapPrefab;

    public GameObject CurrentMap { get { return _currentMapPrefab; } }

    private int _roomCount = 0;
    [SerializeField] private int _bossRoomTrigger = 4;

    private List<int> _clearedMaps = new List<int>();

    public bool OnPortal = false;
    public System.Action OnMapChanged;

    private float _playTime = 0f;
    private bool _isTimerOn = false;

    private GameObject _currentPortal;

    public float CurrentPlayTime => _playTime;
    public bool IsTimerRunning => _isTimerOn;

    public void SetTimerActive(bool active)
    {
        _isTimerOn = active;
        if (!active)
        {
            Debug.Log($"플레이타임 중지: {_playTime}초");
        }
        else
        {
            Debug.Log("플레이타임 시작");
        }
    }
    public FunnelStep GetCurrentFunnelStep()
    {
        string curMapName = CurrentMap.name;
        var lastUnderbarIdx = curMapName.LastIndexOf("_");
        FunnelStep step = FunnelStep.None;
        if (lastUnderbarIdx != -1)
        {
            string curMap = curMapName.Substring(lastUnderbarIdx);

            try
            {
                switch (curMap)
                {
                    case "_Item":
                        step = FunnelStep.None;
                        break;
                    default:
                        step = Enum.Parse<FunnelStep>(curMap);
                        break;
                }
            }
            catch (Exception e)
            {
                return FunnelStep._Boss;
            }

        }
        return step;
    }

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
            UpdateTimerUI();
        }

        if (OnPortal && _currentPortal != null && !_currentPortal.activeSelf)
        {
            _currentPortal.SetActive(true);
            Debug.Log("포탈 활성화!");
        }
        else if (!OnPortal && _currentPortal != null && _currentPortal.activeSelf)
        {
            _currentPortal.SetActive(false);
            Debug.Log("포탈 비활성화!");
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
        SetTimerActive(false);
        UpdateTimerUI();
        UpdatePortalStatusUI();
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

        if (_currentMapPrefab == _mapPrefabs[0])
        {
            LoadMap(_itemRoomPrefab);
            StartCoroutine(ItemStageTrigger());
            Manager.Analytics.SendFunnelStep(FunnelStep.None, 11);
            Manager.Analytics.SendFunnelStep(FunnelStep._StageC, 3);

            SetTimerActive(true);
            Debug.Log("플레이타임기록 시작 (아이템 스테이지 진입)");
            return;
        }

        if (_currentMapPrefab == _itemRoomPrefab)
        {
            Manager.Analytics.SendFunnelStep(FunnelStep._StageC, 4);
            Manager.Analytics.SendFunnelStep(FunnelStep.None, 13);
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

        if (_currentMapPrefab != _itemRoomPrefab)
            Manager.Analytics.SendFunnelStep((FunnelStep)(_mapPrefabs.IndexOf(_currentMapPrefab)), 6);

        if (_roomCount == 1 && !_isTimerOn)
        {
            SetTimerActive(true);
            Debug.Log("플레이타임기록");
        }

        if (_roomCount == _bossRoomTrigger)
        {
            LoadMap(_bossroomPrefabs);
            Debug.Log("보스방 입장!");
            Manager.Analytics.SendFunnelStep(FunnelStep._Boss, 1);
            Manager.Analytics.SendFunnelStep(FunnelStep._StageC, 13);
            return;
        }

        if (_mapPool.Count > 0)
        {
            int poolIndex = Random.Range(0, _mapPool.Count);
            GameObject prefab = _mapPool[poolIndex];
            _mapPool.RemoveAt(poolIndex);

            LoadMap(prefab);
            Manager.Game.OnMonstersClear += SendFunnelOnStageClear;
            Manager.Analytics.SendFunnelStep(FunnelStep._StageC, (_roomCount + 1) * 2 + 3);
            Debug.Log(prefab.name + " 남은맵:" + (_bossRoomTrigger - _roomCount));
        }
        else
        {
            _clearedMaps.Clear();
            UpdateRoomCount();
            ResetMaps();
            LoadMap(_mapPrefabs[0]);
            _playTime = 0f;
            SetTimerActive(false);
            UpdateTimerUI();
            UpdatePortalStatusUI();
            Debug.Log("맵 리셋");
        }
    }

    private void LoadMap(GameObject prefab)
    {
        if (_currentMap != null)
            Destroy(_currentMap);

        _currentMapPrefab = prefab;
        _currentMap = Instantiate(prefab, transform);

        var colliderTransform = _currentMap.transform.Find("Collider");
        if (colliderTransform != null)
        {
            colliderTransform.gameObject.SetActive(false);
            colliderTransform.gameObject.SetActive(true);
        }

        MovePlayerSpawn(_currentMap);
        FindAndRegisterPortal(_currentMap);

        if (OnMapChanged != null)
            OnMapChanged();

        OnPortal = false;
        UpdatePortalStatusUI();
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

    private void FindAndRegisterPortal(GameObject map)
    {
        Transform portalTransform = map.transform.Find("Portal");
        if (portalTransform != null)
        {
            _currentPortal = portalTransform.gameObject;
            _currentPortal.SetActive(OnPortal);
        }
        else
        {
            _currentPortal = null;
        }
    }

    private void UpdateRoomCount()
    {
        _roomCount = _clearedMaps.Count;
    }

    public void SetPortal()
    {
        OnPortal = true;
        Debug.Log("포탈 열림");
        UpdatePortalStatusUI();
    }

    public void DisablePortal()
    {
        OnPortal = false;
        Debug.Log("포탈 닫힘");
        UpdatePortalStatusUI();
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

        ResetMaps();

        int mapIndex = data.CurrentMapIndex;
        GameObject mapToLoad = null;

        if (mapIndex == -1)
        {
            mapToLoad = _bossroomPrefabs;
            SetTimerActive(true);
        }
        else if (mapIndex >= 0 && mapIndex < _mapPrefabs.Count)
        {
            mapToLoad = _mapPrefabs[mapIndex];
            if (mapIndex == 0)
            {
                SetTimerActive(false);
                _playTime = 0f;
            }
            else
            {
                SetTimerActive(true);
            }
        }
        else if (data.CurrentMapIndex == _mapPrefabs.IndexOf(_itemRoomPrefab))
        {
            mapToLoad = _itemRoomPrefab;
            SetTimerActive(true);
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
        UpdateTimerUI();
        UpdatePortalStatusUI();
    }

    private void SendFunnelOnStageClear()
    {
        if (_currentMapPrefab != _itemRoomPrefab && _currentMapPrefab != _bossroomPrefabs)
        {
            Manager.Analytics.SendFunnelStep(FunnelStep._StageC, _roomCount * 2 + 6);
            Manager.Game.OnMonstersClear -= SendFunnelOnStageClear;
        }
    }

    private void UpdateTimerUI()
    {
        if (_timerText == null) return;

        int minutes = Mathf.FloorToInt(_playTime / 60);
        int seconds = Mathf.FloorToInt(_playTime % 60);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdatePortalStatusUI()
    {
        if (_portalStatusText == null) return;

        Color currentColor = _portalStatusText.color;
        if (OnPortal)
        {
            _portalStatusText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }
        else
        {
            _portalStatusText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.3f);
        }
    }
}