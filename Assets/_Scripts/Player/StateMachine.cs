using UnityEngine;

namespace Game.Player
{
    public class StateMachine
    {
        protected IState _current;

        public void ChangeState(IState next)
        {
            if (_current == next) return;
            _current?.Exit();
            _current = next;
            _current?.Enter();
        }

        public void Tick() { _current?.Update(); }
        public void FixedTick() { _current?.PhysicsUpdate(); }
    }
}
