using UnityEngine;

namespace Game.Player
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
        void PhysicsUpdate();
    }
}
