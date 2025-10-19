using UnityEngine;

[CreateAssetMenu(fileName = "New Direciton Audio Data", menuName = "ScriptableObject/Audio/Direciton/Direciton Audio Data")]
public class DirectionAudioData : AudioData
{
    [SerializeField]
    AudioKey.Direction audioKey;
    public AudioKey.Direction AudioKey => audioKey;
}
