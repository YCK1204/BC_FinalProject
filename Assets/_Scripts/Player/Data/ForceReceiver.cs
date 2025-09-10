using UnityEngine;

namespace GameSystem
{
    public class ForceReceiver : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D Body;

        public void Jump(float force)
        {
            var v = Body.linearVelocity;
            Body.linearVelocity = new Vector2(v.x, force);
        }
    }
}
