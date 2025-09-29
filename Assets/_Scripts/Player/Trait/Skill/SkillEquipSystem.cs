// Assets/_Scripts/Player/Trait/Skill/SkillEquipSystem.cs
using System;
using UnityEngine;

namespace Game.Traits
{
    public class SkillEquipSystem : MonoBehaviour
    {
        public static SkillEquipSystem Instance { get; private set; }

        [SerializeField, Min(1)] int _slotCount = 2;
        public int SlotCount => _slotCount;

        public int SelectedSlot { get; private set; } = -1;

        int[] _equipped;

        public event Action<int[]> OnEquipped;
        public event Action<int[]> OnSnapshotChanged;
        public event Action<int> OnSelectedSlotChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (transform.parent != null) transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            _equipped = new int[_slotCount];
            for (int i = 0; i < _equipped.Length; i++) _equipped[i] = -1;
        }

        public int[] GetSnapshot() => (int[])_equipped.Clone();
        public int GetEquippedAt(int slot) => (slot >= 0 && slot < _equipped.Length) ? _equipped[slot] : -1;

        public void SetSelectedSlot(int slot)
        {
            int clamped = (slot < 0) ? -1 : Mathf.Clamp(slot, 0, _slotCount - 1);
            if (clamped == SelectedSlot) return;
            SelectedSlot = clamped;
            OnSelectedSlotChanged?.Invoke(SelectedSlot);
        }
        public void ClearSelection() => SetSelectedSlot(-1);

        public bool IsEquipped(int traitId) => FindSlotOf(traitId) >= 0;

        public int FindSlotOf(int traitId)
        {
            for (int i = 0; i < _equipped.Length; i++)
                if (_equipped[i] == traitId) return i;
            return -1;
        }

        public bool Equip(int traitId)
        {
            if (SelectedSlot < 0) return false;
            return EquipToSlot(traitId, SelectedSlot);
        }

        public bool EquipToSlot(int traitId, int slot)
        {
            if (slot < 0 || slot >= _equipped.Length) return false;
            if (_equipped[slot] == traitId) return false;

            int prev = FindSlotOf(traitId);
            if (prev >= 0) _equipped[prev] = -1;

            _equipped[slot] = traitId;
            FireSnapshotChanged();
            return true;
        }

        public bool UnequipAt(int slot)
        {
            if (slot < 0 || slot >= _equipped.Length) return false;
            if (_equipped[slot] == -1) return false;
            _equipped[slot] = -1;
            FireSnapshotChanged();
            return true;
        }

        public bool UnequipTrait(int traitId)
        {
            int s = FindSlotOf(traitId);
            return s >= 0 && UnequipAt(s);
        }

        void FireSnapshotChanged()
        {
            var snap = GetSnapshot();
            OnEquipped?.Invoke(snap);
            OnSnapshotChanged?.Invoke(snap);
        }
    }
}
