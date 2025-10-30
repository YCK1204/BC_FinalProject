using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool _isPlayer = false;

    [SerializeField] private GameObject _portalFade;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayer = true;
            DisplayManager.Instance.PotalText.SetActive(true);
            //Debug.Log("포탈!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayer = false;
            DisplayManager.Instance.PotalText.SetActive(false);
            //Debug.Log("포탈나감");
        }
    }

    private void Update()
    {
        //테스트
        if (_isPlayer && Input.GetKeyDown(KeyCode.F) && MapManager.Instance.OnPortal)
        {
            Debug.Log("이동!");
            DisplayManager.Instance.PotalFade.SetActive(true);
            MapManager.Instance.NextMap();
            Manager.Data.Save();
        }
    }
}
