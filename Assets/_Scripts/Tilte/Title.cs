using Game.Player;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [Header("Obj")]
    [SerializeField] private GameObject _titleUI;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _canvas;

    [Header("Anim")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Animator _titleAnimator;
    [SerializeField] private Animator _uiAnimator;

    [SerializeField] Button ContinueBtn;
    private void Start()
    {
        ContinueBtn.interactable = (JsonConvert.DeserializeObject<PlayerJsonData>
            (Manager.Data.playerSOData.PlayerJsonData).ClearedMaps.Count > 0);
    }
    public void OnStartButton()
    {
        Manager.Data.playerSOData.ResetData();
        StartCoroutine(StartGame());
    }

    public void OnContinueButton()
    {
        var playerData = Manager.Data.playerSOData;
        PlayerJsonData playerJsonData = JsonConvert.DeserializeObject<PlayerJsonData>(Manager.Data.playerSOData.PlayerJsonData);
        MapSaveData mapSaveData = new MapSaveData()
        {
            ClearedMapIndices = playerJsonData.ClearedMaps,
            CurrentMapIndex = playerJsonData.CurMap,
            PlayTime = playerJsonData.PlayTime
        };
        MapManager.Instance.LoadFromData(mapSaveData);
        PlayerCharacter.Instance.LoadPlayerFromJson(playerData.PlayerJsonData);
        var itemData = JsonConvert.DeserializeObject<InventoryJsonData>(playerData.InventoryJsonData);
        PlayerManager.Instance.Player.Inventory.LoadFromJson(itemData);
        StartCoroutine(StartGame());
        PlayerManager.Instance.gameObject.SetActive(true);
    }

    private IEnumerator StartGame()
    {
        Debug.Log("!!");

        Manager.Analytics.SendFunnelStep(FunnelStep.None, 10);

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
