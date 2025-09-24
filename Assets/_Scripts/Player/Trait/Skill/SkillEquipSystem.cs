using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Traits
{
    public class SkillEquipSystem : MonoBehaviour
    {
        public static SkillEquipSystem Instance { get; private set; }

        [SerializeField] int _maxSlots = 2;

        readonly List<int> _equipped = new List<int>();
        public int[] Equipped => _equipped.ToArray();
        public event Action<int[]> OnEquipped;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool IsEquipped(int traitId) => _equipped.Contains(traitId);

        public void Equip(int traitId)
        {
            if (IsEquipped(traitId)) return;
            if (_equipped.Count >= _maxSlots) _equipped.RemoveAt(0);
            _equipped.Add(traitId);
            OnEquipped?.Invoke(Equipped);
        }

        public void Unequip(int traitId)
        {
            if (_equipped.Remove(traitId))
                OnEquipped?.Invoke(Equipped);
        }

        public void ToggleEquip(int traitId)
        {
            if (IsEquipped(traitId)) Unequip(traitId);
            else Equip(traitId);
        }
    }
}
