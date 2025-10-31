using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Extension.LateStart(() =>
        {
            if (Manager.Data.playerSOData.IsIntroCompleted)
            {
                SceneManager.LoadScene("Main");
            }
        }));
    }
}
