using UnityEngine;

namespace Game.Monster
{
    public interface IStateMachine<T> where T : Component
    {
        public T Owner { get; }

        public void ChangeState(Common.StateType type);
        public void ChangePrevState();
    }
}

