using System.Collections;
using UnityEngine;

public class RandomPlatformGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] Platforms;

    public float GenerateDelay = 2f;
    public float Duration = 7f;

    private WaitForSeconds _generateDelay;
    private WaitForSeconds _duration;

    public void StartGenerate()
    {
        _generateDelay = new WaitForSeconds(GenerateDelay);
        _duration = new WaitForSeconds(Duration);

        StartCoroutine(GeneratePlatform());
    }

    public void StopGenerate()
    {
        StopAllCoroutines();
        foreach (GameObject p in Platforms)
        {
            p.SetActive(false);
        }
    }

    private IEnumerator GeneratePlatform()
    {
        while(true)
        {
            int index = Random.Range(0, Platforms.Length);

            Platforms[index].gameObject.SetActive(true);

            yield return _duration;

            Platforms[index].gameObject.SetActive(false);

            yield return _generateDelay;
        }
    }
}
