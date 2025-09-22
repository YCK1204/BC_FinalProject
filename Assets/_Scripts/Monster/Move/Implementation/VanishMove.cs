using UnityEngine;

public class VanishMove : Game.Monster.IMovable
{
    StateMachineMonster _owner;

    float _curMoveCoolTime;
    float _maxMoveCoolTime;
    float _blinkOffsetX;
    float _blinkOffsetY;

    Transform _tr;
    Transform _target;

    public VanishMove(StateMachineMonster owner)
    {
        _owner = owner;

        _curMoveCoolTime = 1f;
        _maxMoveCoolTime = 2f;
        _blinkOffsetX = 0.7f;
        _blinkOffsetY = 1f;

        _tr = owner.transform;
        _target = owner.Target;
    }

    public void Move()
    {
        if(_curMoveCoolTime > _maxMoveCoolTime)
        {
            _curMoveCoolTime = 0f;
            float dir = _tr.localScale.x > 0 ? 1f : -1f;

            Vector3 blinkPos = _target.position + new Vector3(dir * _blinkOffsetX, _blinkOffsetY, 0);

            _tr.position = blinkPos;
        }
        _curMoveCoolTime += Time.deltaTime;
    }

    public void StopMove()
    {
        // 순간이동에서는 굳이 사용 안함
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
