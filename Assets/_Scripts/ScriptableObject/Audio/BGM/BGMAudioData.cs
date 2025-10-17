using UnityEngine;

[CreateAssetMenu(fileName = "New BGM Audio Data", menuName = "ScriptableObject/Audio/BGM/BGM Audio Data")]
public class BGMAudioData : AudioData
{
    [SerializeField]
    AudioKey.BGM audioKey;
    public AudioKey.BGM AudioKey => audioKey;
    [SerializeField]
    float fadeInTime = 1.0f;
    public float FadeInTime => fadeInTime;
}
