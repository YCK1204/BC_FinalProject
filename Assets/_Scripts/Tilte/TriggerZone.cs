using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [SerializeField]
    private EventController _eventController;

    public int Index;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) 
            return;

        _eventController.OnTriggerActive(Index);

        gameObject.SetActive(false);
    }
}
