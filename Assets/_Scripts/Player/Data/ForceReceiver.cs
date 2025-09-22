using UnityEngine;

namespace Game.Player
{
    public class ForceReceiver : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D Body;

        public void Jump(float force)
        {
#if UNITY_2022_3_OR_NEWER
            var v = Body.linearVelocity;
            Body.linearVelocity = new Vector2(v.x, force);
#else
            var v = Body.velocity;
            Body.velocity = new Vector2(v.x, force);
#endif
        }

        public void Knockback(Vector2 velocity)
        {
#if UNITY_2022_3_OR_NEWER
            Body.linearVelocity = velocity;
#else
            Body.velocity = velocity;
#endif
        }

        public void AddImpulse(Vector2 force)
        {
            Body.linearVelocity = new Vector2(0, Body.linearVelocity.y);
            Body.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
