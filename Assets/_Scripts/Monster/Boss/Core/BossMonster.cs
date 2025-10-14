using System;
using UnityEngine;

public abstract class BossMonster : BaseMonster
{
    // 보스몬스터는 뭐를 가지고 있어야 할까?
    // 1. 데이터
    // 2. 각 컴포넌트
    // 3. 행동 트리
    // 4. 타겟 정보

    public Action OnHealthchanged;
    public bool IsAttacking;
    public bool IsDirecting;

    protected float _bossScale = 1f;
    public float BossScale { get { return _bossScale; } }
}
