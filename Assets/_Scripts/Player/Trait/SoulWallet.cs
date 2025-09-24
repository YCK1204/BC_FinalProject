using System;
using UnityEngine;

namespace Game.Traits
{
    [DefaultExecutionOrder(-1000)]
    public class SoulWallet : MonoBehaviour
    {
        public static SoulWallet Instance { get; private set; }

        [SerializeField] private int startSoul = 0;

        public int CurrentSoul { get; private set; }
        public event Action<int> OnSoulChanged;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;

            CurrentSoul = Mathf.Max(0, startSoul);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Set(int value)
        {
            value = Mathf.Max(0, value);
            if (value == CurrentSoul) return;
            CurrentSoul = value;
            OnSoulChanged?.Invoke(CurrentSoul);
        }

        public void AddSoul(int amount)
        {
            if (amount == 0) return;
            Set(CurrentSoul + amount);
        }

        public bool TryConsume(int amount)
        {
            if (amount < 0 || CurrentSoul < amount) return false;
            Set(CurrentSoul - amount);
            return true;
        }
    }
}
