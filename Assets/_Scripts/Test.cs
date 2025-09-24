using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class MyBehaviour : MonoBehaviour
{
    [SerializeField]
    string url;
    Image rawImage;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetTexture(url));
        rawImage = GetComponent<Image>();
    }


    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator GetTexture(string url)
    {

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.sprite = myTexture.ToSprite();
        }
    }
}