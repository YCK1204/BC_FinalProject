using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameSystem;

public class HpBar : MonoBehaviour
{
    public Slider slider;
    public float lerpSpeed = 2f;

    public PlayerCharacter player;


    void Update()
    {
        if (player != null && slider.value != player.CurrentHP)
        {
            SetHp(player.CurrentHP);
        }
    }

    public void SetMaxHp(float maxHp)
    {
        slider.maxValue = maxHp;
        slider.value = maxHp;
    }

    public void SetHp(float hp)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateHpBar(hp));
    }

    private IEnumerator UpdateHpBar(float targetHp)
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