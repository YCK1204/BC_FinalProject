// 컴파일 테스트 파일
using Game.Player;
using Game.Item;
using UnityEngine;

public class CompileTest : MonoBehaviour
{
    void TestCompile()
    {
        // 1. ItemTriggerType 테스트
        ItemTriggerType trigger = ItemTriggerType.OnUsingSkill;
        Debug.Log($"Trigger type: {trigger}");

        // 2. ItemSynergyData nullable 테스트
        ItemSynergyData? synergyData = null;
        if (synergyData.HasValue)
        {
            Debug.Log("Synergy data exists");
        }
        else
        {
            Debug.Log("Synergy data is null");
        }

        // 3. ItemEffectFactory 테스트
        var areaData = new ItemAreaData(1, 1f, ItemCreateAreaPosType.Player, 2f, 50f, 1);
        var effect = ItemEffectFactory.CreateEffect(areaData, ItemTriggerType.OnHit);
        Debug.Log($"Effect created: {effect != null}");

        // 4. ItemEffectManager 테스트
        var effectManager = new ItemEffectManager();
        Debug.Log("ItemEffectManager created successfully");
    }
}