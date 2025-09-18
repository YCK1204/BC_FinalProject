using UnityEngine;

namespace Game.Monster
{
    public interface IState
    {
        public StateType StateType { get; }
        public void Enter();
        public void Exit();
        public void Update();
        public void FixedUpdate();
    }
}