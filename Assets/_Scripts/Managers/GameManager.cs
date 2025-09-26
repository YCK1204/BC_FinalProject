using System;
using UnityEngine;

public class GameManager
{
    public Action OnMonstersClear = null;
    int _monsterCount = 0;
    public int MonsterCount
    {
        get { return _monsterCount; } 
        set
        {
            if (value < 0) return;
            _monsterCount = value;
            if (_monsterCount == 0)
            {
                OnMonstersClear?.Invoke();
                MapManager.Instance.SetPortal();
                Debug.Log("몬스터 전부 처치");
            }
        }
    }
}
