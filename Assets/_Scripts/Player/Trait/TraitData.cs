using UnityEngine;

namespace Game.Traits
{
    public enum TraitKind { Skill, Passive }

    [CreateAssetMenu(fileName = "TraitData", menuName = "Game/Trait Data")]
    public class TraitData : ScriptableObject
    {
        [Header("Key")]
        public int Id;
        public int ConditionId;

        [Header("Type")]
        public TraitKind Kind;

        [Header("Cost")]
        public int SoulCost;
    }
}
