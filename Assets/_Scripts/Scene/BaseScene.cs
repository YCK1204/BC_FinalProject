using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Extension.LateStart(() =>
        {
            if (PlayerPrefs.GetInt("is_intro_completed") == 1)
            {
                SceneManager.LoadScene("Main");
            }
        }));
    }
}
