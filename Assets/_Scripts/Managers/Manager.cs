using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        Screen.SetResolution(1920, 1080, true);
        Data.Load();
        Pool.Init();
        Item.Init();
        Analytics.Init();
        Instance._isInit = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Destructible"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Gold"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Destructible"), LayerMask.NameToLayer("Gold"));
    }
    GameManager _game = new GameManager();
    public static GameManager Game { get { return Instance._game; } }
    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }
    SceneManagerEx _scene;
    public static SceneManagerEx Scene { get { return Instance._scene; } set { Instance._scene = value; value.transform.parent = Instance.transform; } }
    AudioManager _audio;
    public static AudioManager Audio { get { return Instance._audio; } set { Instance._audio = value; value.transform.parent = Instance.transform; } }
    PoolManager _pool = new PoolManager();
    public static PoolManager Pool { get { return Instance._pool; } }
    DataManager _data = new DataManager();
    public static DataManager Data { get { return Instance._data; } }
    ItemManager _item = new ItemManager();
    public static ItemManager Item { get { return Instance._item; } }
    AnalyticsManager _analytics = new AnalyticsManager();
    public static AnalyticsManager Analytics { get { return Instance._analytics; } }
}
