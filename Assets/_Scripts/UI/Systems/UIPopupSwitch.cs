using UnityEngine;
using UnityEngine.Events;

public class UIPopupSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject gameObject1;
    [SerializeField] private GameObject gameObject2;

    private void OnEnable()
    {
        if (gameObject1 != null) gameObject1.SetActive(true);
        if (gameObject2 != null) gameObject2.SetActive(false);
    }

    public void Switch1()
    {
        if (gameObject1 != null)
        {
            gameObject1.SetActive(true);
        }

        if (gameObject2 != null)
        {
            gameObject2.SetActive(false);
        }
    }

    public void Switch2()
    {
        if (gameObject2 != null)
        {
            gameObject2.SetActive(true);
        }

        if (gameObject1 != null)
        {
            gameObject1.SetActive(false);
        }
    }
}