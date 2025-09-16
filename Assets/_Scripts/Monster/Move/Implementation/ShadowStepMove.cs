using UnityEngine;

public class ShadowStepMove : Game.Monster.IMovable
{
    float _curMoveCoolTime;
    float _maxMoveCoolTime;
    float _blinkOffsetX;
    float _blinkOffsetY;

    Transform _tr;
    Transform _target;

    public ShadowStepMove(Transform tr, Transform target)
    {
        _curMoveCoolTime = 1f;
        _maxMoveCoolTime = 2f;
        _blinkOffsetX = 0.7f;
        _blinkOffsetY = 1.2f;

        _tr = tr;
        _target = target;
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
        
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
