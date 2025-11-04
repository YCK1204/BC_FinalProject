using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Game.Player;

public class HpBar : MonoBehaviour
{
    public Slider slider;
    public float lerpSpeed = 2f;

    void OnEnable()
    {
        PlayerCharacter player = PlayerCharacter.Instance;
        if (player != null)
        {
            player.HpEvent += HpUpdate;

            slider.minValue = 0f;
            slider.maxValue = 1f;

            float initialHpRatio = player.Data.Stats.MaxHP > 0 ? player.CurrentHP / player.Data.Stats.MaxHP : 0f;
            slider.value = initialHpRatio;
        }
    }

    void OnDisable()
    {
        PlayerCharacter player = PlayerCharacter.Instance;
        if (player != null)
        {
            player.HpEvent -= HpUpdate;
        }
    }

    void HpUpdate(float currentHp, float maxHp)
    {
        float targetHpRatio = maxHp > 0 ? currentHp / maxHp : 0f;

        if (gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateHpBarCoroutine(targetHpRatio));
        }
        else
        {
            slider.value = targetHpRatio;
        }
    }

    private IEnumerator UpdateHpBarCoroutine(float targetHp)
    {
        float elapsedTime = 0f;
        float startingValue = slider.value;

        while (!Mathf.Approximately(slider.value, targetHp))
        {
            slider.value = Mathf.Lerp(startingValue, targetHp, elapsedTime * lerpSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        slider.value = targetHp;
    }
}