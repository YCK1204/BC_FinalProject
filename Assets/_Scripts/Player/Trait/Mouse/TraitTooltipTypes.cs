using System;

namespace Game.Traits.UI
{
    public enum TraitNodeType
    {
        Passive,
        Skill
    }

    public enum Ability
    {
        none,
        plusAttack,
        attack,
        plusSkillAttack,
        skillAttack,
        attackSpeed,
        skillHaste,
        HP,
        criticalDamage,
        criticalChance,
        corruptionDuration,
        plusMovementSpeed
    }

    [Serializable]
    public struct AbilityDisplay
    {
        public Ability Key;
        public string DisplayName;
        public string Unit;
        public bool Percent;
    }
}
