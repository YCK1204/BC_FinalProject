using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField]
    private Image _cooldownImage;
    [SerializeField]
    private bool _chk;

    private Coroutine _cooldownCoroutine;

    private void Start()
    {
        if (_chk)
        {
            _cooldownImage.fillAmount = 1;
        }
        else
        {
            _cooldownImage.fillAmount = 0;
        }
    }

    public void StartCooldown(float cooldownTime)
    {
        if (_cooldownCoroutine != null)
        {
            StopCoroutine(_cooldownCoroutine);
        }

        _cooldownCoroutine = StartCoroutine(CooldownCoroutine(cooldownTime));
    }

    private IEnumerator CooldownCoroutine(float cooldownTime)
    {
        float timer = 0f;
        _cooldownImage.fillAmount = 1;

        while (timer < cooldownTime)
        {
            timer += Time.deltaTime;
            _cooldownImage.fillAmount = 1.0f - (timer / cooldownTime);
            yield return null;
        }

        _cooldownImage.fillAmount = 0;
        _cooldownCoroutine = null;
    }

    public void ShowCooldown()
    {
        _cooldownImage.fillAmount = 1;
    }

    public void HideCooldown()
    {
        _cooldownImage.fillAmount = 0;
    }
}