using System.Collections.Generic;
using UnityEngine;

public class Berserk : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    private bool _isBerserk = false;
    private List<NormalMonster> _affectedMonsterList;

    [SerializeField] private GameObject PowerUpParticle;
    private List<ParticleSystem> _particleList;

    public void Init(NormalMonster monster)
    {
        _particleList = new List<ParticleSystem>();
        _affectedMonsterList = new List<NormalMonster>();

        // 몬스터가 플레이어를 발견하면 주변 몬스터에게 버프 부여
        // 공격력 버프 제공
        monster.Ondetect += () =>
        {
            // 한번만 발동하도록 설정
            if (!_isBerserk)
            {
                _isBerserk = true;
                Collider2D[] monsters = Physics2D.OverlapCircleAll(monster.transform.position, monster.MonsterData.DetectRange, LayerMask.GetMask(Game.Monster.Layers.Monster));

                foreach (Collider2D collider in monsters)
                {
                    // 자기 자신은 제외
                    if (collider == monster.Col)
                        continue;

                    // 공격력만 상승
                    NormalMonster affectedMonster = collider.GetComponent<NormalMonster>();
                    affectedMonster?.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Attack, Game.Monster.ModifierType.Add, 10, monster));

                    // 버프를 제공한 몬스터를 리스트에 등록
                    _affectedMonsterList.Add(affectedMonster);
                    if (affectedMonster.GetComponentInChildren<ParticleSystem>() != null)
                        continue;
                    ParticleSystem particle = Instantiate(PowerUpParticle).GetComponent<ParticleSystem>();
                    if (particle != null)
                    {
                        particle.transform.parent = affectedMonster.transform;
                        particle.transform.localPosition = Vector3.zero;
                        _particleList.Add(particle);
                    }
                }
            }
        };

        // 해당 몬스터가 죽으면 리스트를 순회하며 버프 제거
        monster.OnDiedEnter += () =>
        {
            if (_affectedMonsterList != null)
            {
                foreach (StateMachineMonster affectedMonster in _affectedMonsterList)
                {
                    affectedMonster.MonsterData.RemoveModifierByCaster(monster);
                }
            }
            if (_particleList != null)
            {
                for (int i = 0; i < _particleList.Count; i++)
                {
                    if(_particleList[i] != null)
                        Destroy(_particleList[i].gameObject);
                }
                _particleList.Clear();
            }
        };
    }
}
