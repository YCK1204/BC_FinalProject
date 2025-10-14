using System.Collections;
using UnityEngine;

public class RandomPlatformGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] Platforms;

    public float GenerateDelay = 2f;
    public float Duration = 7f;

    private WaitForSeconds _generateDelay;
    private WaitForSeconds _duration;

    private BossMonster _owner;

    public void Init(BossMonster boss)
    {
        _owner = boss;
    }

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
            if(_owner.IsDirecting)
            {
                yield return null;
                continue;
            }

            yield return _generateDelay;
            int index = Random.Range(0, Platforms.Length);

            Platforms[index].gameObject.SetActive(true);

            yield return _duration;

            Platforms[index].gameObject.SetActive(false);
        }
    }
}
