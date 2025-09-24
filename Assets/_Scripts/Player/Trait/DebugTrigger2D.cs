using UnityEngine;

public class DebugTrigger2D : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) { Debug.Log($"ENTER: {other.name}, tag={other.tag}"); }
    void OnTriggerStay2D(Collider2D other) { Debug.Log($"STAY:  {other.name}"); }
    void OnTriggerExit2D(Collider2D other) { Debug.Log($"EXIT:  {other.name}"); }
}
