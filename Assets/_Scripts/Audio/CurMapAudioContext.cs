using UnityEngine;

public class CurMapAudioContext : MonoBehaviour
{
    [SerializeField]
    MapAudioContext MapAudioContext;

    private void Start()
    {
        StartCoroutine(Extension.LateStart(() =>
        {
            Manager.Audio.Init(MapAudioContext);
            Destroy(gameObject);
        }));
    }
}
