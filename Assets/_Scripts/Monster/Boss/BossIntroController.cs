using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class BossIntroController : MonoBehaviour
{
    [SerializeField] GameObject LetterBox;
    [SerializeField] GameObject BossTitle;
    [SerializeField] GameObject BossCam;
    [SerializeField] GameObject BossHealthBar;
    [SerializeField] Transform CameraTr;

    PlayableDirector _director;
    BossMonster _owner;

    public void Init(BossMonster boss)
    {
        _owner = boss;
        _director = GetComponent<PlayableDirector>();
    }

    public void StartIntro()
    {
        Manager.Analytics.SendFunnelStep(FunnelStep._Boss, 2);
        DisplayManager.Instance.SetPPC(false);
        DisplayManager.Instance.HubFadeOut();
        _director.Play();
    }

    public void StopIntro()
    {
        LetterBox.SetActive(false);
        BossTitle.SetActive(false);
        BossCam.SetActive(false);

        DisplayManager.Instance.HubFadeIn();
        BossHealthBar.SetActive(true);

        _owner.IsDirecting = false;
    }

    [SerializeField] private float duration = 1f;
    [SerializeField] private float magnitude = 0.2f;

    private Vector3 originalPos;

    public void Shake()
    {
        StopAllCoroutines();
        originalPos = CameraTr.localPosition;
        StartCoroutine(ShakeCoroutine());
        Manager.Audio.Play(AudioKey.Direction.DIR_BOSS_START, _owner.transform);
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            CameraTr.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        CameraTr.localPosition = originalPos;
    }

    public void SetBossCam(bool b)
    {
        BossCam.SetActive(b);
    }
}
