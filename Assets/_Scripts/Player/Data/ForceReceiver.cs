using UnityEngine;

namespace Game.Player
{
    public class ForceReceiver : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D Body;

        public void Jump(float force)
        {
            var v = Body.linearVelocity;
            Body.linearVelocity = new Vector2(v.x, force);
        }

        public void Knockback(Vector2 velocity)
        {
            Body.linearVelocity = velocity;
        }

        public void AddImpulse(Vector2 force)
        {
            Body.linearVelocity = new Vector2(0, Body.linearVelocity.y);
            Body.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
