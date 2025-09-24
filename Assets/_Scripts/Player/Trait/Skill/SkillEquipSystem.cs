using System;
using UnityEngine;

namespace Game.Traits
{
    public class SkillEquipSystem : MonoBehaviour
    {
        public static SkillEquipSystem Instance { get; private set; }

        public readonly int[] Equipped = new int[2] { -1, -1 };
        public event Action<int[]> OnEquipped;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Equip(int traitId)
        {
            if (traitId < 0) return;
            if (Array.IndexOf(Equipped, traitId) >= 0) { OnEquipped?.Invoke(Equipped); return; }

            for (int i = 0; i < Equipped.Length; i++)
            {
                if (Equipped[i] == -1)
                {
                    Equipped[i] = traitId;
                    OnEquipped?.Invoke(Equipped);
                    Debug.Log($"[Equip] Slot {i + 1} <- {traitId}");
                    return;
                }
            }

            Equipped[0] = traitId;
            OnEquipped?.Invoke(Equipped);
            Debug.Log($"[Equip] Slot 1 <- {traitId} (replaced)");
        }

        public bool IsEquipped(int traitId) => Array.IndexOf(Equipped, traitId) >= 0;
    }
}
