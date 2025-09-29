using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Game.Player;

public class AwakeningBar : MonoBehaviour
{
    public Slider slider;
    public float lerpSpeed = 2f;
    public PlayerCharacter player;

    private void Start()
    {
        if (player != null)
        {
            slider.minValue = 0f;
            slider.maxValue = player.Data.awakening.maxAwakeningGauge;
            slider.value = 0f;

            player.AwakeningEvent += UpdateAwakening;
        }
    }

    private void OnDestroy()
    {
        if (player != null)
            player.AwakeningEvent -= UpdateAwakening;
    }

    private void UpdateAwakening(float current, float max)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateAwakeningBar(current));
    }

    private IEnumerator UpdateAwakeningBar(float target)
    {
        float elapsedTime = 0f;
        float startValue = slider.value;

        while (!Mathf.Approximately(slider.value, target))
        {
            slider.value = Mathf.Lerp(startValue, target, elapsedTime * lerpSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        slider.value = target;
    }
}
