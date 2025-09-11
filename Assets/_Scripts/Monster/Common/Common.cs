using UnityEngine;

namespace Common
{
    public enum StateType
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hit,
        Die
    }

    public static class Layers
    {
        public const string Player = "Player";
        public const string Monster = "Monster";
    }

    public static class AnimatorParams
    {
        public const string Speed = "Speed";
        public const string Attack = "Attack";
        public const string Hit = "Hit";
        public const string Die = "Die";
    }
}
