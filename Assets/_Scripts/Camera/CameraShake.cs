using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float size, float rate, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(size, rate, duration));
    }

    private IEnumerator DoShake(float size, float rate, float duration)
    {
        noise.AmplitudeGain = size;
        noise.FrequencyGain = rate;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }
}
