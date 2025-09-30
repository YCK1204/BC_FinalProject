using Game.Player;
using Game.Traits.UI;
using UnityEngine;

namespace Game.Traits
{
    [DefaultExecutionOrder(50)]
    public class TraitPassiveApplier : MonoBehaviour
    {
        [SerializeField] TraitUnlockSystem _unlock;
        [SerializeField] PlayerCharacter _player;
        [SerializeField] TraitPassiveTable _passiveTable;

        struct Deltas
        {
            public float MaxHP;
            public float AttackPower;
            public float AttackPowerPercent;
            public float SkillAttack;
            public float SkillAttackPercent;
            public float AttackSpeed;
            public float SkillHaste;
            public float CriticalDamage;
            public float CriticalChance;
            public float AwakenDuration;
        }

        Deltas _applied;

#if UNITY_2023_1_OR_NEWER
        static T FindOne<T>() where T : Object =>
            Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
        static T FindOne<T>() where T : Object =>
            Object.FindObjectOfType<T>(true);
#endif

        void Awake()
        {
            if (_unlock == null) _unlock = FindOne<TraitUnlockSystem>();
            if (_player == null) _player = FindOne<PlayerCharacter>();
            if (_passiveTable == null) _passiveTable = Resources.Load<TraitPassiveTable>("TraitPassiveTable");
        }

        void OnEnable()
        {
            if (_unlock != null)
            {
                _unlock.OnStateChanged += ReapplyAll;
                _unlock.OnUnlocked += OnUnlockedHandler;
            }
            ReapplyAll();
        }

        void OnDisable()
        {
            if (_unlock != null)
            {
                _unlock.OnStateChanged -= ReapplyAll;
                _unlock.OnUnlocked -= OnUnlockedHandler;
            }
        }

        void OnUnlockedHandler(int _) => ReapplyAll();

        public void ReapplyAll()
        {
            if (_player == null || _passiveTable == null || _unlock == null) return;

            float prevAppliedMaxHp = _applied.MaxHP;

            RemoveLastDeltas();

            var sum = CalcDeltasFromUnlocked();
            Apply(sum);

            float deltaMaxHp = sum.MaxHP - prevAppliedMaxHp;
            if (deltaMaxHp > 0f) _player.Heal(deltaMaxHp);
            else _player.Heal(0f);

            _applied = sum;
        }

        void RemoveLastDeltas()
        {
            var d = _player.Data;

            d.Stats.MaxHP -= _applied.MaxHP;

            d.CombatData.AttackPower -= _applied.AttackPower;
            d.CombatData.AttackPowerPercent -= _applied.AttackPowerPercent;

            d.CombatData.SkillAttck -= _applied.SkillAttack;
            d.CombatData.SkillAttckPercent -= _applied.SkillAttackPercent;

            d.CombatData.AttackSpeed -= _applied.AttackSpeed;
            d.CombatData.SkillHaste -= _applied.SkillHaste;

            d.CombatData.CriticalDamage -= _applied.CriticalDamage;
            d.CombatData.CriticalChance -= _applied.CriticalChance;

            d.awakening.duration -= _applied.AwakenDuration;

            _applied = default;
        }

        Deltas CalcDeltasFromUnlocked()
        {
            Deltas sum = default;
            var ids = _unlock.State.UnlockedIds;

            for (int i = 0; i < ids.Count; i++)
            {
                var rows = _passiveTable.GetRowsFor(ids[i]);
                foreach (var r in rows)
                {
                    float v = r.IsPercent ? r.Value * 0.01f : r.Value;

                    switch (r.Ability)
                    {
                        case Ability.plusAttack: sum.AttackPower += v; break;
                        case Ability.attack: sum.AttackPowerPercent += v; break;
                        case Ability.plusSkillAttack: sum.SkillAttack += v; break;
                        case Ability.skillAttack: sum.SkillAttackPercent += v; break;
                        case Ability.attackSpeed: sum.AttackSpeed += v; break;
                        case Ability.skillHaste: sum.SkillHaste += v; break;
                        case Ability.HP: sum.MaxHP += v; break;
                        case Ability.criticalDamage: sum.CriticalDamage += r.Value; break;
                        case Ability.criticalChance: sum.CriticalChance += r.Value; break;
                        case Ability.awakenDuration: sum.AwakenDuration += r.Value; break;
                    }
                }
            }
            return sum;
        }

        void Apply(Deltas dlt)
        {
            var d = _player.Data;

            d.Stats.MaxHP += dlt.MaxHP;

            d.CombatData.AttackPower += dlt.AttackPower;
            d.CombatData.AttackPowerPercent += dlt.AttackPowerPercent;

            d.CombatData.SkillAttck += dlt.SkillAttack;
            d.CombatData.SkillAttckPercent += dlt.SkillAttackPercent;

            d.CombatData.AttackSpeed += dlt.AttackSpeed;
            d.CombatData.SkillHaste += dlt.SkillHaste;

            d.CombatData.CriticalDamage += dlt.CriticalDamage;
            d.CombatData.CriticalChance += dlt.CriticalChance;

            d.awakening.duration += dlt.AwakenDuration;
        }
    }
}
