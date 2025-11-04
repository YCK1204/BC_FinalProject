using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Game.Player;

public class AwakeningBar : MonoBehaviour
{
    public Slider slider;
    public float lerpSpeed = 2f;

    void OnEnable()
    {
        PlayerCharacter player = PlayerCharacter.Instance;
        if (player != null)
        {
            player.AwakeningEvent += AwakeningUpdate;

            slider.minValue = 0f;
            slider.maxValue = player.Data.awakening.MaxAwakeningGauge;
            slider.value = player.CurrentAwakening;
        }
    }

    void OnDisable()
    {
        PlayerCharacter player = PlayerCharacter.Instance;
        if (player != null)
        {
            player.AwakeningEvent -= AwakeningUpdate;
        }
    }

    private void AwakeningUpdate(float current, float max)
    {
        if (gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateAwakeningBarCoroutine(current));
        }
        else
        {
            slider.value = current;
        }
    }

    private IEnumerator UpdateAwakeningBarCoroutine(float target)
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