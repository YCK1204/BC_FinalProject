using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DeadControl : MonoBehaviour
{
    [SerializeField] private GameObject _deadEffect;
    [SerializeField] private Animator _deadAnimator;
    [SerializeField] private Animator _setAnimator;
    private string _dieAnim = "all_off";
    private string _dieAnim_on = "all_on";
    private string _againAnim = "again";
    private string _againAnim2 = "again2";
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _playerDumy;

    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private string _message = "AGAIN...";
    [SerializeField] private float _textDelay = 1f;
    [SerializeField] private float _textSpeed = 0.3f;

    [Header("Volume")]
    [SerializeField] private Volume _volume;
    [SerializeField] private float _bloomMin = 0.4f;
    [SerializeField] private float _bloomMax = 1.0f;
    [SerializeField] private float _bloomSpeed = 3.0f;

    [Header("Interaction")]
    [SerializeField] private GameObject _pickObject;
    [SerializeField] private GameObject _choiceDumy;

    [SerializeField] private CameraShake _camShake;

    private Bloom _bloom;
    private Collider2D _pickCol;
    private Coroutine _pulseLoop;

    private bool _canInteract = false;
    private bool _isMouseOver = false;
    private bool _isPress = false;
    private bool _onChoice = false;

    void Start()
    {
        _gameOverText.text = "";
        _volume.profile.TryGet<Bloom>(out _bloom);
        _pickCol = _pickObject.GetComponent<Collider2D>();

        _canInteract = false;
        _bloom.scatter.value = 0.5f;
    }

    void Update()
    {
        if (!_canInteract)
            return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool onPick = _pickCol.OverlapPoint(mousePosition);

        if (onPick && !_isMouseOver)
        {
            _isMouseOver = true;
            _bloom.scatter.value = 0.4f;
        }
        else if (!onPick && _isMouseOver)
        {
            _isMouseOver = false;
            _bloom.scatter.value = 0.2f;
        }

        if (onPick && Input.GetMouseButtonDown(0))
        {
            _isPress = true;
        }

        if (_onChoice && ( Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Z)))
        {
            if (onPick && _isPress)
            {
                StartCoroutine(OnPickClicked());
            }
            _isPress = false;
        }
    }

    public void DieSet()
    {
        Manager.Audio.StopBgm();
        StartCoroutine(DeathSequence());

        _bloom.scatter.value = 0.2f;
    }

    IEnumerator DeathSequence()
    {
        PlayerManager.Instance.Player.Animator.Play("Idle", 0, 0f);
        _onChoice = false;

        yield return new WaitForSeconds(1f);

        _setAnimator.Play("Die", 0, 0f);
        _deadEffect.SetActive(true);
        _deadAnimator.Play(_dieAnim, 0, 0f);
        _pickObject.SetActive(true);

        Debug.Log("죽음연출");

        yield return new WaitForSeconds(_textDelay);

        _pulseLoop = StartCoroutine(Bloomloop());

        _gameOverText.gameObject.SetActive(true);
        yield return StartCoroutine(AgainText(_message));

        _choiceDumy.SetActive(true);
        _onChoice = true;
    }

    IEnumerator AgainText(string text)
    {
        _gameOverText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            _gameOverText.text += letter;
            yield return new WaitForSeconds(_textSpeed);
        }

        _canInteract = true;
        _player.gameObject.SetActive(false);
        _playerDumy.gameObject.SetActive(true);
    }

    IEnumerator Bloomloop()
    {
        float timer = 0f;

        while (true)
        {
            float range = _bloomMax - _bloomMin;
            float loopValue = _bloomMin + Mathf.PingPong(timer * _bloomSpeed, range);

            float setIntensity = loopValue;

            if (_isMouseOver)
            {
                setIntensity += 1f;

                if (_isPress)
                {
                    setIntensity -= 0.5f;
                }
            }

            _bloom.intensity.value = setIntensity;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator OnPickClicked()
    {
        _canInteract = false;
        _isMouseOver = false;
        _pickObject.SetActive(false);
        _choiceDumy.SetActive(false);

        PlayerManager.Instance.Player.SetPlayerInput(false);

        StopCoroutine(_pulseLoop);
        _pulseLoop = null;
        _bloom.scatter.value = 0.4f;
        _gameOverText.text = "";
        _setAnimator.Play(_againAnim, 0, 0f);

        StartCoroutine(FadeBloom(10f, 3f));

        yield return new WaitForSeconds(1);
        PlayerManager.Instance.Player.PlayerMaterial.SetGlitchMaterial();

        yield return new WaitForSeconds(3);

        MapManager.Instance.InitFloor();
        _setAnimator.Play(_againAnim2, 0, 0f);

        yield return new WaitForSeconds(1.5f);

        _player.gameObject.SetActive(true);
        PlayerManager.Instance.Player.Resurrection();
        PlayerManager.Instance.Player.Animator.Play("Landing", 0, 0f);
        _deadEffect.SetActive(false);
        _bloom.scatter.value = 0.5f;

        _camShake.Shake(1f, 3f, 0.2f);

        StartCoroutine(FadeBloom(0.4f, 0.5f));

        yield return new WaitForSeconds(1.2f);

        PlayerManager.Instance.Player.Animator.Play("Idle", 0, 0f);
        _deadAnimator.Play(_dieAnim_on, 0, 0f);
        PlayerManager.Instance.Player.SetPlayerInput(true);
        _gameOverText.gameObject.SetActive(false);

        Manager.Audio.SetBgm(AudioKey.BGM.BGM_BASE);
    }

    IEnumerator FadeBloom(float bloom, float over)
    {
        float from = _bloom.intensity.value;
        float timer = 0f;

        while (timer < over)
        {
            timer += Time.deltaTime;
            float pross = timer / over;
            _bloom.intensity.value = Mathf.Lerp(from, bloom, pross);
            yield return null;
        }

        _bloom.intensity.value = bloom;
    }
}