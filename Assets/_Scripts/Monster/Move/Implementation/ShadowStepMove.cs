using UnityEngine;

public class ShadowStepMove : Game.Monster.IMovable
{
    bool _isMoveStart;

    public ShadowStepMove()
    {

    }

    public void Move()
    {
        // 여기서 플래그 켜고 이후에 인보크나 코루틴으로 일정 시간 지나면 플래그 끄기
    }

    public void StopMove()
    {
        
    }
}
