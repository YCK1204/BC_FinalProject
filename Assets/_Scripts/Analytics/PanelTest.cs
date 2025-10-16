using UnityEngine;

public class PanelTest : MonoBehaviour
{
    public FunnelStep param;
    public int value = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            Manager.Analytics.SendFunnelStep(param, value);
    }
}
