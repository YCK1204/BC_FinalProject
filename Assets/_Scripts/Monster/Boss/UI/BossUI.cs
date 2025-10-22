using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    BossMonster _owner;

    public GameObject BossHpBar;
    public Image BossHpBarForward;

    public void Init(BossMonster boss)
    {
        _owner = boss;
        _owner.OnHealthchanged += UpdateHpBar;
    }

    public void UpdateHpBar()
    {
        if(_owner.MonsterData.CurHp >0)
        {
            BossHpBarForward.fillAmount = _owner.MonsterData.CurHp / _owner.MonsterData.MaxHp;
        }
        else
        {
            BossHpBar.gameObject.SetActive(false);
        }
    }

    public void DisableUI()
    {
        BossHpBar.SetActive(false);
    }    
}
