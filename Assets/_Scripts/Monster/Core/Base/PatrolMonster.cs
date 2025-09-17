using Game.Monster;
using UnityEngine;

/// <summary>
/// 순찰 기능(사실상 이동 기능)이 있는 몬스터 클래스
/// </summary>
public abstract class PatrolMonster : Monster
{
    protected IMovable curPatrolMovement;
    protected IMovable curChaseMovement;

    protected override void Init()
    {
        base.Init();

        curPatrolMovement = new PatrolMove(_dataHandler.Speed, transform, _rb, _col); ;
        curChaseMovement = new ChaseMove(_dataHandler.Speed, transform, _rb, _col, Target, _anim);
        //curChaseMovement = new VanishMove(transform, Target);
    }

    public IMovable GetChaseMovement()
    {
        return curChaseMovement;
    }

    public IMovable GetPatrolMovement()
    {
        return curPatrolMovement;
    }
}
