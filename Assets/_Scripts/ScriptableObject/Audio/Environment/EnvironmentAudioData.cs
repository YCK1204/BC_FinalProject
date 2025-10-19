using UnityEngine;

[CreateAssetMenu(fileName = "New Environment Audio Data", menuName = "ScriptableObject/Audio/Environment/Environment Audio Data")]
public class EnvironmentAudioData : AudioData
{
    [SerializeField]
    AudioKey.Environment audioKey;
    public AudioKey.Environment AudioKey => audioKey;
}
