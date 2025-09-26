using UnityEngine;

namespace Game.Monster
{
    public enum SpecilaAbillityType
    {
        SplitOnDeath,
        SuperArmor,
        Berserk
    }

    public interface ISpecialAbillity
    {
        public void Init(NormalMonster monster);
    }
}
