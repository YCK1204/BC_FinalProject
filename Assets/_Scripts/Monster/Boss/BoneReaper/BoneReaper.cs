using UnityEngine;

public class BoneReaper : BossMonster
{
    // 각 파츠들에 대한 정보
    private BoneReaperHead _head;
    private BoneReaperHand _leftHand;
    private BoneReaperHand _rightHand;

    protected float _patternCoolTime = 5f;
    protected float _patternMaxCoolTime = 8f;

    protected int _curSlamCount;
    protected int _curBreathCount;

    private BoneReaperBT _curBT;
    public BoneReaperBT BT { get { return _curBT; } }

    private void Awake()
    {
        if (_head != null && _leftHand != null && _rightHand != null)
        {
            _head.Init(this);
            _leftHand.Init(this);
            _rightHand.Init(this);
        }
    }

    private void OnEnable()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (_dataHandler.CurHp <= 0)
            return;

        _dataHandler.TakeDamage(damage);
        if (_dataHandler.CurHp > 0)
        {
            // 피격 효과 코루틴 작동
        }
    }
}
