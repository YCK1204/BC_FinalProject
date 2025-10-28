using Unity.Cinemachine;
using UnityEngine;

public class CameraBorderSetter : MonoBehaviour
{
    CinemachineConfiner2D confiner2D;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        if(MapManager.Instance != null)
        {
            MapManager.Instance.OnMapChanged -= SetCamBounds;
            MapManager.Instance.OnMapChanged += SetCamBounds;
        }
        SetCamBounds();
    }

    public void SetCamBounds()
    {
        if (MapManager.Instance == null)
            return;

        PolygonCollider2D polygon = MapManager.Instance.CurrentMap.GetComponentInChildren<PolygonCollider2D>();
        if (polygon != null)
        {
            confiner2D.BoundingShape2D = MapManager.Instance.CurrentMap.GetComponentInChildren<PolygonCollider2D>();
        }
    }
}
