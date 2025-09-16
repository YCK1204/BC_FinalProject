using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Slider slider;
    public float lerpSpeed = 2f;

    //임시
    private float hp = 1;

    //테스트 함수
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            hp -= 0.1f;
            SetHp(hp);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            hp -= 0.2f;
            SetHp(hp);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            slider.value = 1f;
            hp = 1f;
            SetHp(hp);
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
        Debug.Log(hp);
    }

    private IEnumerator UpdateHpBar(float hp)
    {
        float elapsedTime = 0f;
        float startingValue = slider.value;

        //반복
        while (!Mathf.Approximately(slider.value, hp))
        {
            //선형보간함수
            slider.value = Mathf.Lerp(startingValue, hp, elapsedTime * lerpSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        slider.value = hp;
    }
}
