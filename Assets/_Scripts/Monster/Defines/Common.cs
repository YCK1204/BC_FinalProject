using UnityEngine;

// 공용으로 사용하는 enum이나 string 등을 모아두는 스크립트
namespace Common
{
    // 몬스터 스테이트 머신의 상태를 구분하는 열거자
    public enum StateType
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hit,
        Die
    }

    // 레이어 이름 문자열
    public static class Layers
    {
        public const string Player = "Player";
        public const string Monster = "Monster";
    }

    // 적 애니메이션 패러미터 문자열
    public static class AnimatorParams
    {
        public const string Speed = "Speed";
        public const string Attack = "Attack";
        public const string Hit = "Hit";
        public const string Die = "Die";
    }
}
