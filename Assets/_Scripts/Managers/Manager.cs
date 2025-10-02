using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager _instance = null;
    static Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<Manager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("@Manager");
                    _instance = go.AddComponent<Manager>();
                    Init();
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }
    bool _isInit = false;
    static void Init()
    {
        if (Instance._isInit)
            return;
        Data.Load();
        Pool.Init();
        Item.Init();
        Instance._isInit = true;
    }
    GameManager _game = new GameManager();
    public static GameManager Game { get { return Instance._game; } }
    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }
    SceneManagerEx _scene;
    public static SceneManagerEx Scene { get { return Instance._scene; } set { Instance._scene = value; value.transform.parent = _instance.transform; } }
    AudioManager _audio;
    public static AudioManager Audio { get { return Instance._audio; } set { Instance._audio = value; value.transform.parent = _instance.transform; } }
    PoolManager _pool = new PoolManager();
    public static PoolManager Pool { get { return Instance._pool; } }
    DataManager _data = new DataManager();
    public static DataManager Data { get { return Instance._data; } }
    ItemManager _item = new ItemManager();
    public static ItemManager Item { get { return Instance._item; } }
}
