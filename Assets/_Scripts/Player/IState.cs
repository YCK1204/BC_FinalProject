using UnityEngine;

namespace GameSystem
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
        void PhysicsUpdate();
    }
}
