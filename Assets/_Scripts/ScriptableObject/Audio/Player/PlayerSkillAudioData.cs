using UnityEngine;

[CreateAssetMenu(fileName = "New Player Skill Audio Data", menuName = "ScriptableObject/Audio/Player/Skill Audio Data")]
public class PlayerSkillAudioData : AudioData
{
    [SerializeField]
    AudioKey.Player.Skill audioKey;
    public AudioKey.Player.Skill AudioKey => audioKey;
}
