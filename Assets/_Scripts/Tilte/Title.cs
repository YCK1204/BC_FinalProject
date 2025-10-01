using System.Collections;
using UnityEngine;

public class Title : MonoBehaviour
{
    [Header("Obj")]
    [SerializeField] private GameObject _titleUI;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _camera;

    [Header("Anim")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Animator _titleAnimator;
    [SerializeField] private Animator _uiAnimator;


    public void OnStartButton()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        _titleAnimator.Play("Tilte_Gamestart", 0, 0f);
        
        _camera.SetActive(false);

        _player.SetActive(true);

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
