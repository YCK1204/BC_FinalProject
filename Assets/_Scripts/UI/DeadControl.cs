using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DeadControl : MonoBehaviour
{
    [SerializeField] private GameObject _deadEffect;
    [SerializeField] private Animator _deadAnimator;
    [SerializeField] private string _dieAnim = "all_off";
    [SerializeField] private string _againAnim = "again";

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

    private Bloom _bloom;
    private Collider2D _pickCol;
    private Coroutine _pulseLoop;
    private Animator _setAnimator;
    private bool _canInteract = false;
    private bool _isMouseOver = false;
    private bool _isPress = false;

    void Start()
    {
        _setAnimator = GetComponent<Animator>();
        _gameOverText.text = "";
        _volume.profile.TryGet<Bloom>(out _bloom);
        _pickCol = _pickObject.GetComponent<Collider2D>();

        _canInteract = false;
        _bloom.scatter.value = 0f;
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
            _bloom.scatter.value = 0.2f;
        }
        else if (!onPick && _isMouseOver)
        {
            _isMouseOver = false;
            _bloom.scatter.value = 0f;
        }

        if (onPick && Input.GetMouseButtonDown(0))
        {
            _isPress = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (onPick && _isPress)
            {
                OnPickClicked();
            }
            _isPress = false;
        }
    }

    public void DieSet()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        _deadEffect.SetActive(true);
        _deadAnimator.Play(_dieAnim, 0, 0f);

        Debug.Log("죽음연출");

        yield return new WaitForSeconds(_textDelay);

        _pulseLoop = StartCoroutine(Bloomloop());

        _gameOverText.gameObject.SetActive(true);
        yield return StartCoroutine(AgainText(_message));
    }

    private IEnumerator AgainText(string text)
    {
        _gameOverText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            _gameOverText.text += letter;
            yield return new WaitForSeconds(_textSpeed);
        }

        _canInteract = true;
    }

    private IEnumerator Bloomloop()
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

    private void OnPickClicked()
    {
        _canInteract = false;
        _isMouseOver = false;

        if (_pulseLoop != null)
        {
            StopCoroutine(_pulseLoop);
            _pulseLoop = null;
        }

        _bloom.scatter.value = 0f;
        _bloom.intensity.value = _bloomMin;

        _gameOverText.text = "";
        _setAnimator.Play(_againAnim, 0, 0f);
    }
}