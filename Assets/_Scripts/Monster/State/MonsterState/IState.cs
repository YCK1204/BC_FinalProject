using UnityEngine;

namespace Game.Monster
{
    public interface IState
    {
        public Common.StateType StateType { get; }
        public void Enter();
        public void Exit();
        public void Update();
        public void FixedUpdate();
    }
}