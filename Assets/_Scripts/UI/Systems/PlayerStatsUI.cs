using UnityEngine;
using UnityEngine.UI;
using Game.Player;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private Text hpText;
    [SerializeField] private Text apText;

    [SerializeField] private Text attackPowerText;
    [SerializeField] private Text extraDamageText;
    [SerializeField] private Text attackSpeedText;
    [SerializeField] private Text criticalDamageText;
    [SerializeField] private Text criticalChanceText;
    [SerializeField] private Text skillAttackText;
    [SerializeField] private Text skillDamageText;
    [SerializeField] private Text skillHasteText;

    private PlayerCharacter playerCharacter;

    private void OnEnable()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.Player != null)
        {
            playerCharacter = PlayerManager.Instance.Player;

            playerCharacter.HpEvent += UpdateHPUI;
            playerCharacter.AwakeningEvent += UpdateAPUI;

            UpdateHPUI(playerCharacter.CurrentHP, playerCharacter.Data.Stats.MaxHP);
            UpdateAPUI(playerCharacter.CurrentAwakening, playerCharacter.Data.awakening.MaxAwakeningGauge);
            UpdateCombatStatsUI();
        }
        else
        {
            Debug.LogError("null UIText.");
        }
    }

    private void OnDisable()
    {
        if (playerCharacter != null)
        {
            playerCharacter.HpEvent -= UpdateHPUI;
            playerCharacter.AwakeningEvent -= UpdateAPUI;
        }
    }

    public void UpdateHPUI(float currentHP, float maxHP)
    {
        if (hpText != null)
        {
            hpText.text = $"{Mathf.RoundToInt(currentHP)}/{Mathf.RoundToInt(maxHP)}";
        }
    }

    public void UpdateAPUI(float currentAwakening, float maxAwakening)
    {
        if (apText != null)
        {
            apText.text = $"{Mathf.RoundToInt(currentAwakening)}/{Mathf.RoundToInt(maxAwakening)}";
        }
    }

    private void UpdateCombatStatsUI()
    {
        if (playerCharacter == null) return;

        PlayerCombatData combatData = playerCharacter.Data.CombatData;

        if (attackPowerText != null) attackPowerText.text = Mathf.RoundToInt(combatData.AttackPower).ToString();
        if (extraDamageText != null) extraDamageText.text = Mathf.RoundToInt(combatData.AttackPowerPercent * 100).ToString() + "%";
        if (attackSpeedText != null) attackSpeedText.text = combatData.AttackSpeed.ToString("F1");
        if (criticalDamageText != null) criticalDamageText.text = Mathf.RoundToInt(combatData.CriticalDamage).ToString() + "%";
        if (criticalChanceText != null) criticalChanceText.text = Mathf.RoundToInt(combatData.CriticalChance).ToString() + "%";
        if (skillAttackText != null) skillAttackText.text = Mathf.RoundToInt(combatData.SkillAttck).ToString();
        if (skillDamageText != null) skillDamageText.text = Mathf.RoundToInt(combatData.SkillAttckPercent * 100).ToString() + "%";
        if (skillHasteText != null) skillHasteText.text = Mathf.RoundToInt(combatData.SkillHaste).ToString();
    }
}