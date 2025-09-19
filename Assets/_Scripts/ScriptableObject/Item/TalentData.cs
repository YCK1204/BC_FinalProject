using GameSystem;
using UnityEngine;

public enum TalentType
{
    Corruption,
    Normal,
    Both
}
public enum TalentActionType
{
    OnAttack,
    OnHit,
    OnKill,
    OnSkill,
    OnStartRound,
    OnDashEnd,
    Always
}

[CreateAssetMenu(fileName = "New Special Ability", menuName = "ScriptableObject/Item/SpecialAbilityData")]
public class TalentData : ScriptableObject
{
    [SerializeField]
    int talentID;
    public int TalentID { get { return talentID; } }
    [SerializeField]
    TalentType condition;
    public TalentType Condition { get { return condition; } }
    [SerializeField]
    [Range(0f, 100f)]
    float activationChance;
    public float ActivationChance { get { return activationChance; } }
    [SerializeField]
    [Range(0f, 100f)]
    float cooldown;
    public float Cooldown { get { return cooldown; } }
    [SerializeField]
    SpecialAbilityData specialAbility;
    public SpecialAbilityData SpecialAbility { get { return specialAbility; } }
    [SerializeField]
    TalentActionType actionType;
    public TalentActionType ActionType { get { return actionType; } }


    float _lastCooldown = 0f;
    public void RegisterSpecialAbilityEvent(PlayerCharacter player)
    {
        _lastCooldown = Time.time;

        //switch (ActionType)
        //{
        //    case TalentActionType.OnAttack:
        //        player.OnAttack += OnEvent;
        //        break;
        //        case TalentActionType.OnHit:
        //        player.OnHit += OnEvent;
        //        break;
        //        case TalentActionType.OnKill:
        //        player.OnKill += OnEvent;
        //        break;
        //        case TalentActionType.OnSkill:
        //        player.OnSkill += OnEvent;
        //        break;
        //        case TalentActionType.OnStartRound:
        //        player.OnStartRound += OnEvent;
        //        break;
        //        case TalentActionType.OnDashEnd:
        //        player.OnDashEnd += OnEvent;
        //        break;
        //        case TalentActionType.Always:
        //        OnEvent(player);
        //        break;
        //}
    }
    public void UnregisterSpecialAbilityEvent(PlayerCharacter player)
    {
        switch (actionType)
        {
            case TalentActionType.OnAttack:
                //player.OnAttack -= OnEvent;
                break;
            case TalentActionType.OnHit:
                //player.OnHit -= OnEvent;
                break;
            case TalentActionType.OnKill:
                //player.OnKill -= OnEvent;
                break;
            case TalentActionType.OnSkill:
                //player.OnSkill -= OnEvent;
                break;
            case TalentActionType.OnStartRound:
                //player.OnStartRound -= OnEvent;
                break;
            case TalentActionType.OnDashEnd:
                //player.OnDashEnd -= OnEvent;
                break;
            case TalentActionType.Always:
                break;
        }
    }
    void OnEvent(PlayerCharacter player)
    {
        if (Time.time - _lastCooldown < Cooldown) return;
        if (Random.Range(0f, 100f) > ActivationChance) return;
        _lastCooldown = Time.time;
        SpecialAbility.Activate(player);
    }
}
