using UnityEngine;

[CreateAssetMenu(fileName = "New Player Move Audio Data", menuName = "ScriptableObject/Audio/Player/Move Audio Data")]
public class PlayerMoveAudioData : AudioData
{
    [SerializeField]
    AudioKey.Player.Move audioKey;
    public AudioKey.Player.Move AudioKey => audioKey;
}
