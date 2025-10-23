using UnityEngine;

public class FunnelGenerator : MonoBehaviour
{
    public FunnelStep param;
    public int value = 0;
    private bool _isTriggered = false;

    // 플레이어가 해당 영역에 닿으면 지정된 퍼널을 발생시킴
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !_isTriggered)
        {
            _isTriggered = true;
            Manager.Analytics.SendFunnelStep(param, value);
        }
    }
}
