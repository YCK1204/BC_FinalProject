using Game.Player;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public float CurrentHP;
    public float CurrentAwakening;
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public PlayerCharacter Player;

    public GameObject[] HUB;

    public CooldownUI CooldownQ;
    public CooldownUI CooldownW;
    public CooldownUI CooldownS;
    public CooldownUI CooldownD;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //PoolManager.CreatePool<a>(20,a);
    }

    public void HUBSet(bool setbool)
    {
        foreach (GameObject hub in HUB)
        {
            if (hub != null)
                hub.SetActive(setbool);
        }
    }

    public PlayerSaveData GetSaveData()
    {
        if (Player == null) return null;

        PlayerSaveData data = new PlayerSaveData();

        data.CurrentHP = Player.CurrentHP;
        data.CurrentAwakening = Player.CurrentAwakening;

        return data;
    }

    public void LoadFromData(PlayerSaveData data)
    {
        if (Player == null || data == null) return;

        Player.ApplyData(data);
    }
}
