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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    GameManager _game;
    public static GameManager Game { get { return Instance._game; } set { Instance._game = value; value.transform.parent = _instance.transform; } }
    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }
    SceneManagerEx _scene;
    public static SceneManagerEx Scene { get { return Instance._scene; } set { Instance._scene = value; value.transform.parent = _instance.transform; } }
    AudioManager _audio;
    public static AudioManager Audio { get { return Instance._audio; } set { Instance._audio = value; value.transform.parent = _instance.transform; } }
    PoolManager _pool;
    public static PoolManager Pool { get { return Instance._pool; } set { Instance._pool = value; value.transform.parent = _instance.transform; } }
}
