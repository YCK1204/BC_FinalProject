using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Traits
{
    [Serializable] public class PlayerTraitStateData { public List<int> UnlockedIds = new(); }

    public class TraitUnlockSystem : MonoBehaviour
    {
        public TraitTable Table;
        public SoulWallet Wallet;
        public PlayerTraitStateData State = new();

        public List<int> StartUnlocked = new();

        public event Action<int> OnUnlocked;
        public event Action OnStateChanged;

        public bool IsUnlocked(int id) => State.UnlockedIds.Contains(id);

        void Awake()
        {
            for (int i = 0; i < StartUnlocked.Count; i++)
            {
                int id = StartUnlocked[i];
                if (!State.UnlockedIds.Contains(id))
                {
                    State.UnlockedIds.Add(id);
                    OnUnlocked?.Invoke(id);
                }
            }
        }

        void Start()
        {
            OnStateChanged?.Invoke();
        }

        public bool CanUnlock(int id)
        {
            if (IsUnlocked(id)) return false;
            if (!Table || !Table.TryGet(id, out var data)) return false;
            if (data.ConditionId != 0 && !IsUnlocked(data.ConditionId)) return false;
            if (Wallet == null || Wallet.CurrentSoul < data.SoulCost) return false;
            return true;
        }

        public bool TryUnlock(int id)
        {
            if (!CanUnlock(id)) return false;
            if (!Table.TryGet(id, out var data)) return false;
            if (!Wallet.TryConsume(data.SoulCost)) return false;

            State.UnlockedIds.Add(id);
            OnUnlocked?.Invoke(id);
            OnStateChanged?.Invoke();
            return true;
        }
    }
}
