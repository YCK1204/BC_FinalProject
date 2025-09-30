using System;
using UnityEngine;
using Game.Traits;

namespace Game.Traits.UI
{
    public class TraitTooltipBuilder : MonoBehaviour
    {
        [Header("Common")]
        [SerializeField] private string _name;
        [SerializeField] private TraitNodeType _nodeType = TraitNodeType.Passive;
        [SerializeField] private int _conditionGoods = 0;
        [SerializeField] private bool _isUnlocked = false;

        [Header("Skill Only")]
        [SerializeField] private string _skillType = "";
        [SerializeField] private float _skillCooldown = 0f;
        [SerializeField] private int _descId = 0;

        [Header("Passive Only")]
        [SerializeField] private Ability _ability = Ability.none;
        [SerializeField] private float _value = 0f;

        [Header("Ability Display Map")]
        [SerializeField]
        private AbilityDisplay[] _abilityMap = new AbilityDisplay[]
        {
            new AbilityDisplay{ Key=Ability.plusAttack,         DisplayName="추가 공격력",       Unit="고정 값", Percent=false },
            new AbilityDisplay{ Key=Ability.attack,             DisplayName="추가 피해",         Unit="%",      Percent=true  },
            new AbilityDisplay{ Key=Ability.plusSkillAttack,    DisplayName="추가 스킬 공격력",   Unit="고정 값", Percent=false },
            new AbilityDisplay{ Key=Ability.skillAttack,        DisplayName="추가 스킬 피해",     Unit="%",      Percent=true  },
            new AbilityDisplay{ Key=Ability.attackSpeed,        DisplayName="추가 공격 속도",     Unit="%",      Percent=true  },
            new AbilityDisplay{ Key=Ability.skillHaste,         DisplayName="추가 스킬 가속",     Unit="%",      Percent=true  },
            new AbilityDisplay{ Key=Ability.HP,                 DisplayName="추가 체력",         Unit="고정 값", Percent=false },
            new AbilityDisplay{ Key=Ability.criticalDamage,     DisplayName="추가 치명타 피해",   Unit="%",      Percent=true  },
            new AbilityDisplay{ Key=Ability.criticalChance,     DisplayName="추가 치명타 확률",   Unit="%",      Percent=true  },
            new AbilityDisplay{ Key=Ability.plusMovementSpeed,  DisplayName="추가 이동 속도",     Unit="%",      Percent=true  },
            new AbilityDisplay{ Key=Ability.awakenDuration,     DisplayName="추가 각성 지속 시간", Unit="고정 값", Percent=false },
        };

        public string GetName() => _name;

        public TraitTooltipModel Build()
        {
            var m = new TraitTooltipModel { Title = _name };

            if (_nodeType == TraitNodeType.Skill)
            {
                if (!string.IsNullOrEmpty(_skillType))
                    m.SubTitle = _skillType;

                if (_skillCooldown > 0.01f)
                    m.Lines.Add(new TooltipLine($"쿨다운 : {Mathf.RoundToInt(_skillCooldown)}초", emphasize: true));

                string desc = (DescriptionDB.Instance != null) ? DescriptionDB.Instance.Get(_descId) : string.Empty;
                if (!string.IsNullOrEmpty(desc))
                    m.Lines.Add(new TooltipLine(desc));
            }
            else
            {
                var disp = FindDisplay(_ability);
                string valueText = disp.Percent ? $"{_value:0.#}%" : $"{_value:0.#}";
                m.Lines.Add(new TooltipLine($"{disp.DisplayName}  {valueText}", emphasize: true));
            }

            if (!_isUnlocked && _conditionGoods > 0)
            {
                int owned = SoulWallet.Instance ? SoulWallet.Instance.CurrentSoul : 0;
                bool lack = owned < _conditionGoods;
                m.Lines.Add(new TooltipLine($"활성화에 필요한 재화 : {_conditionGoods}", false, lack));
            }

            return m;
        }

        AbilityDisplay FindDisplay(Ability key)
        {
            foreach (var a in _abilityMap)
                if (a.Key == key) return a;

            return new AbilityDisplay
            {
                Key = key,
                DisplayName = key.ToString(),
                Unit = "",
                Percent = false
            };
        }

        public void SetUnlocked(bool unlocked) => _isUnlocked = unlocked;

        public void SetOwnedSouls(int souls)
        {
            if (SoulWallet.Instance)
                SoulWallet.Instance.Set(souls);
        }
    }
}
