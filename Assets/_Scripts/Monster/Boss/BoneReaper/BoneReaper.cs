using UnityEngine;

public class BoneReaper : BossMonster
{
    // 각 파츠들에 대한 정보
    private BoneReaperHead _head;
    public BoneReaperHead Head { get { return _head; } }
    private BoneReaperHand _leftHand;
    public BoneReaperHand LeftHand { get { return _leftHand; } }
    private BoneReaperHand _rightHand;
    public BoneReaperHand RightHand { get { return _rightHand; } }

    protected float _patternCoolTime = 5f;
    protected float _patternMaxCoolTime = 8f;

    protected int _curSlamCount;
    protected int _curBreathCount;

    private BoneReaperBT _curBT;
    public BoneReaperBT BT { get { return _curBT; } }

    private void Awake()
    {
        _curBT = GetComponent<BoneReaperBT>();
        _curBT.Init(this);

        if (_head != null && _leftHand != null && _rightHand != null)
        {
            _head.Init(this);
            _leftHand.Init(this);
            _rightHand.Init(this);
        }
    }

    private void Update()
    {
        if(_dataHandler.CurHp > 0)
            _curBT.Evaluate();
    }

    public void FindTarget()
    {

    }

    public void TakeDamage(int damage)
    {
        if (_dataHandler.CurHp <= 0)
            return;

        _dataHandler.TakeDamage(damage);
        if (_dataHandler.CurHp <= 0)
        {
            
        }

    }
}
