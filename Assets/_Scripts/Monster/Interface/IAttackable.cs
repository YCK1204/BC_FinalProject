using UnityEngine;

namespace Game.Monster
{
    public interface IAttackable
    {
        public void Attack();
        public bool GetCheckAttackable(float margin = 0);
        public void Init();
    }

}
