using System;
using UnityEngine;

namespace Game.Traits
{
    public class SkillEquipSystem : MonoBehaviour
    {
        public static SkillEquipSystem Instance { get; private set; }

        [SerializeField, Min(1)] private int _slotCount = 2;
        public int SlotCount => _slotCount;

        // -1 이면 빈 칸
        private int[] _equipped;
        public int[] GetSnapshot() => (int[])_equipped.Clone();
        public int GetEquippedAt(int slot) => (slot >= 0 && slot < _equipped.Length) ? _equipped[slot] : -1;

        // ✅ 기본값을 -1로 (아무 것도 선택 안 됨)
        public int SelectedSlot { get; private set; } = -1;

        public event Action<int[]> OnEquipped;
        public event Action<int[]> OnSnapshotChanged;
        public event Action<int> OnSelectedSlotChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _equipped = new int[_slotCount];
            for (int i = 0; i < _equipped.Length; i++) _equipped[i] = -1;
        }

        // ✅ -1 허용(선택 해제), 나머지는 범위 클램프
        public void SetSelectedSlot(int slot)
        {
            int clamped = (slot < 0) ? -1 : Mathf.Clamp(slot, 0, _slotCount - 1);
            if (clamped == SelectedSlot) return;
            SelectedSlot = clamped;
            OnSelectedSlotChanged?.Invoke(SelectedSlot);
        }

        public bool IsEquipped(int traitId) => FindSlotOf(traitId) >= 0;

        public int FindSlotOf(int traitId)
        {
            for (int i = 0; i < _equipped.Length; i++)
                if (_equipped[i] == traitId) return i;
            return -1;
        }

        public bool Equip(int traitId) => EquipToSlot(traitId, SelectedSlot);

        public bool EquipToSlot(int traitId, int slot)
        {
            // ✅ 선택된 슬롯이 없으면 실패
            if (slot < 0 || slot >= _equipped.Length) return false;

            if (_equipped[slot] == traitId) return false;

            int prev = FindSlotOf(traitId);
            if (prev >= 0) _equipped[prev] = -1;

            _equipped[slot] = traitId;

            FireSnapshotChanged();

            // ✅ 장착 후 자동 선택 해제
            SetSelectedSlot(-1);
            return true;
        }

        public bool Toggle(int traitId)
        {
            if (SelectedSlot < 0) return false; // ✅ 슬롯 미선택 시 무시

            if (_equipped[SelectedSlot] == traitId)
            {
                bool ok = UnequipAt(SelectedSlot);
                if (ok) SetSelectedSlot(-1);     // ✅ 해제 후 선택 해제
                return ok;
            }
            else
            {
                return EquipToSlot(traitId, SelectedSlot); // 내부에서 자동 해제됨
            }
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

        private void FireSnapshotChanged()
        {
            var snap = GetSnapshot();
            OnEquipped?.Invoke(snap);
            OnSnapshotChanged?.Invoke(snap);
        }
    }
}
