using UnityEngine;

[CreateAssetMenu(fileName = "New Trait Audio Data", menuName = "ScriptableObject/Audio/Trait/Trait Audio Data")]
public class TraitAudioData : AudioData
{
    [SerializeField]
    AudioKey.Trait audioKey;
    public AudioKey.Trait AudioKey => audioKey;
}
