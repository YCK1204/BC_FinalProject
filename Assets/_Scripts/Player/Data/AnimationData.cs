using UnityEngine;

namespace Game.Player
{
    [System.Serializable]
    public class AnimationData
    {
        public int IdleParameterHash = Animator.StringToHash("Idle");
        public int WalkParameterHash = Animator.StringToHash("Walk");
        public int JumpParameterHash = Animator.StringToHash("Jump");
        public int FallParameterHash = Animator.StringToHash("Fall");
        public int AirParameterHash = Animator.StringToHash("Air");
        public int GroundParameterHash = Animator.StringToHash("Ground");
        public int AttackParameterHash = Animator.StringToHash("Attack");
        public int DashParameterHash = Animator.StringToHash("Dash");
        public int HurtParameterHash = Animator.StringToHash("Hurt");
        public int DieParameterHash = Animator.StringToHash("Die");
        public int ComboParameterHash = Animator.StringToHash("Combo");

        public int AwakeningParameterHash = Animator.StringToHash("Awaken");
    }
}
