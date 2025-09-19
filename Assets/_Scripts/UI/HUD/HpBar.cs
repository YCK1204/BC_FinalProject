using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameSystem;

public class HpBar : MonoBehaviour
{
    public Slider slider;
    public float lerpSpeed = 2f;

    public PlayerCharacter player;


    void Start()
    {
        if (player != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            player.HpEvent += UpdateHp;
        }
    }
    void OnDestroy()
    {
        if (player != null)
            player.HpEvent -= UpdateHp;
    }

    void UpdateHp(float currentHp, float maxHp)
    {
        float hp = currentHp / maxHp;
        StopAllCoroutines();
        StartCoroutine(UpdateHpBar(hp));
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