using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

public class Title : MonoBehaviour
{
    [Header("Obj")]
    [SerializeField] private GameObject _titleUI;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _mainCamera;

    [Header("Anim")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Animator _titleAnimator;
    [SerializeField] private Animator _uiAnimator;


    public void OnStartButton()
    {
        StartCoroutine(StartGame());
    }

    public void OnContinueButton()
    {
        if (PlayerPrefs.GetInt("is_intro_completed") == 1)
        {
            var playerData = Manager.Data.PlayerData;
            if (playerData == null)
                StartCoroutine(StartGame());
            else
            {
                MapSaveData mapSaveData = new MapSaveData()
                {
                    ClearedMapIndices = playerData.ClearedMaps,
                    CurrentMapIndex = playerData.CurMap,
                    PlayTime = playerData.PlayTime
                };
                MapManager.Instance.LoadFromData(mapSaveData);
                PlayerSaveData playerSaveData = new PlayerSaveData()
                {
                    CurrentAwakening = playerData.Awaken,
                    CurrentHP = playerData.Hp,
                };
                PlayerManager.Instance.LoadFromData(playerSaveData);
                var itemData = PlayerPrefs.GetString("ItemData");
                if (string.IsNullOrEmpty(itemData) == false)
                {
                    var itemJsonData = JsonConvert.DeserializeObject<InventoryJsonData>(itemData);
                    PlayerManager.Instance.Player.Inventory.LoadFromJson(itemJsonData);
                }
                StartCoroutine(StartGame());
            }
        }
    }

    private IEnumerator StartGame()
    {
        Debug.Log("!!");

        _titleAnimator.Play("Tilte_Gamestart", 0, 0f);

        _camera.SetActive(false);

        _player.SetActive(true);
        _mainCamera.SetActive(true);

        _playerAnimator.Play("Landing", 0, 0f);
        PlayerManager.Instance.Player.SetPlayerInput(false);

        yield return new WaitForSeconds(1f);
        _gameUI.SetActive(true);
        _uiAnimator.Play("all_on", 0, 0f);

        yield return new WaitForSeconds(1f);
        _playerAnimator.Play("Idle", 0, 0f);
        PlayerManager.Instance.Player.SetPlayerInput(true);

        yield return new WaitForSeconds(1f);
        _titleUI.SetActive(false);
    }
}
