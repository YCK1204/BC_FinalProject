using UnityEngine;

namespace Game.Monster
{
    public interface IDamageable
    {
        public void TakeDamage(float damage, bool hitSfxMute = false);
    }
}