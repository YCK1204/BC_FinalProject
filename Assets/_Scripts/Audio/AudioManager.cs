using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;
using UnityEngine.UI;

public enum AudioMixerType { MASTER, BGM, EFFECT }
class LoopingAudio
{
    public AudioSource Source;
    Action<LoopingAudio> _onStop;
    public LoopingAudio(Action<LoopingAudio> onStop)
    {
        _onStop = onStop;
    }
    public void StartLoop(AudioSource source)
    {
        Source = source;
        Source.Play();
    }
    public void StopLoop()
    {
        if (Source != null)
        {
            Source.Stop();
            Source.gameObject.SetActive(false);
        }
        _onStop?.Invoke(this);
    }
}
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioMixer AudioMixer;
    [SerializeField]
    AudioSource Prefab;
    [SerializeField]
    int InitialPoolSize = 20;

    Stack<AudioSource> _pool = new Stack<AudioSource>();
    Stack<LoopingAudio> _loopingAudioPool = new Stack<LoopingAudio>();
    Dictionary<string, AudioData> _audioDataDict = new Dictionary<string, AudioData>();
    Dictionary<Transform, Queue<LoopingAudio>> _loopingAudioDict = new Dictionary<Transform, Queue<LoopingAudio>>();
    AudioSource _bgmSource;

    [Header("SetAudioSlider")]
    private Slider _bgmSliderInternal;
    public Slider BgmSlider
    {
        get { return _bgmSliderInternal; }
        set
        {
            _bgmSliderInternal = value;
            if (_bgmSliderInternal != null)
            {
                Debug.Log("BGM Slider assigned. Setting up.");
                SetupSingleSlider(AudioMixerType.BGM, _bgmSliderInternal);
            }
        }
    }
    private Slider _sfxSliderInternal;
    public Slider SfxSlider
    {
        get { return _sfxSliderInternal; }
        set
        {
            _sfxSliderInternal = value;
            if (_sfxSliderInternal != null)
            {
                Debug.Log("SFX Slider assigned. Setting up.");
                SetupSingleSlider(AudioMixerType.EFFECT, _sfxSliderInternal);
            }
        }
    }

    void Start()
    {
        if (Manager.Audio != null)
        {
            Object.Destroy(gameObject);
            return;
        }
        Manager.Audio = this;
        _bgmSource = GetComponent<AudioSource>();

        for (int i = 0; i < InitialPoolSize; i++)
        {
            var source = Instantiate(Prefab, transform).GetComponent<AudioSource>();
            source.gameObject.SetActive(false);
            _pool.Push(source);

            var loopingAudio = new LoopingAudio(LoopStopAction);
            _loopingAudioPool.Push(loopingAudio);
        }
        _originalBgmVolume = GetVolume(AudioMixerType.BGM);
        StartCoroutine(CoCheckDisableLoop());
    }

    void SetupSingleSlider(AudioMixerType type, Slider slider)
    {
        if (slider == null) return;

        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((value) => SetVolume(type, value));
        slider.value = GetVolume(type);
        Debug.Log($"Slider for {type.ToString()} initialized with value: {slider.value}");
    }
    void LoopStopAction(LoopingAudio la)
    {
        if (la.Source != null)
        {
            la.Source.transform.parent = transform;
            _pool.Push(la.Source);
        }
        _loopingAudioPool.Push(la);
    }
    IEnumerator CoCheckDisableLoop()
    {
        while (true)
        {
            List<Transform> removeList = new List<Transform>();
            foreach (var pair in _loopingAudioDict)
            {
                if (pair.Key == null || pair.Key.gameObject.activeSelf == false)
                    removeList.Add(pair.Key);
            }
            removeList.ForEach(x =>
            {
                var queue = _loopingAudioDict[x];
                while (queue.Count > 0)
                {
                    var la = queue.Dequeue();
                    la.StopLoop();
                }
                _loopingAudioDict.Remove(x);
            });
            yield return new WaitForSeconds(5f);
        }
    }
    /// <summary>
    /// 맵별 오디오 컨텍스트를 기반으로 오디오 데이터 초기화
    /// - 현재 맵에 필요한 오디오만 딕셔너리에 로드
    /// - 불필요한 오디오는 메모리에서 제거
    /// </summary>
    /// <param name="mapAudioContext">로드할 맵의 오디오 컨텍스트</param>
    public void Init(MapAudioContext mapAudioContext)
    {
        var mapAudioDict = mapAudioContext.ToDict();

        var keyList1 = _audioDataDict.Keys.ToList();
        var keyList2 = mapAudioDict.Keys.ToList();

        var removeList = keyList1.Except(keyList2).ToList();
        var needsList = keyList2.Except(keyList1).ToList();

        removeList.ForEach(x => _audioDataDict.Remove(x));
        needsList.ForEach(x =>
        {
            mapAudioDict.TryGetValue(x, out var audioData);
            if (audioData != null) _audioDataDict.Add(x, audioData);
        });

        if (mapAudioContext.AutoSetBGM)
        {
            var bgmData = mapAudioContext.BGMAudioDataContext.bgmAudioDatas[0];
            SetBgm(bgmData.AudioClip, bgmData.FadeInTime);
        }
    }
    private AudioSource GetSource()
    {
        AudioSource source = null;
        while (source == null && _pool.Count > 0)
            source = _pool.Pop();
        if (source != null) return source;
        source = Instantiate(Prefab, transform);
        return source;
    }
    LoopingAudio GetLoopingAudio()
    {
        if (_loopingAudioPool.Count > 0)
            return _loopingAudioPool.Pop();
        var loopingAudio = new LoopingAudio(LoopStopAction);
        return loopingAudio;
    }
    /// <summary>
    /// Enum 키를 사용하여 오디오 재생
    /// - Loop 설정된 오디오: 루프 재생 (StopLoop으로 중지 필요)
    /// - 일반 오디오: 한 번 재생 후 자동 반환
    /// - 3D 사운드: owner Transform 위치에서 재생 및 추적
    /// </summary>
    /// <typeparam name="T">AudioKey의 Enum 타입</typeparam>
    /// <param name="key">재생할 오디오의 Enum 키 (예: AudioKey.Player.Move.Walk)</param>
    /// <param name="owner">오디오를 소유한 Transform (3D 사운드 위치 기준)</param>
    /// <param name="is3D">3D 공간 사운드 여부 (true: spatialBlend 1.0, false: 2D)</param>
    public void Play<T>(T key, Transform owner, bool is3D = false) where T : Enum
    {
        _audioDataDict.TryGetValue(key.ToString(), out var audioData);
        if (audioData == null)
        {
            Debug.LogError($"AudioManager Play Error! No AudioData Key : {key.ToString()}");
            return;
        }
        if (audioData.AudioClip == null)
        {
            Debug.LogError($"AudioManager Play Error! No AudioClip Key : {key.ToString()}");
            return;
        }

        AudioSource source = GetSource();
        source.spatialBlend = is3D ? 1f : 0f;
        source.loop = audioData.IsLoop;
        source.clip = audioData.AudioClip;
        source.volume = audioData.GetVolume;

        source.gameObject.SetActive(true);
        if (audioData.IsLoop)
        {
            _loopingAudioDict.TryGetValue(owner, out var queue);
            if (queue == null) _loopingAudioDict.Add(owner, new Queue<LoopingAudio>());
            var loopingAudio = GetLoopingAudio();
            _loopingAudioDict[owner].Enqueue(loopingAudio);

            if (is3D)
            {
                source.transform.parent = owner;
                source.transform.localPosition = Vector3.zero;
            }
            loopingAudio.StartLoop(source);
        }
        else
        {
            StartCoroutine(CoPlay(source, owner, is3D));
        }
    }
    /// <summary>
    /// 특정 owner의 가장 오래된 루프 사운드 중지
    /// - Queue에서 FIFO 방식으로 가장 먼저 시작한 루프 제거
    /// - Coroutine 정지 및 AudioSource Pool 반환
    /// - Queue가 비면 Dictionary에서 자동 제거
    /// </summary>
    /// <param name="owner">루프 사운드를 중지할 대상 Transform</param>
    public void StopLoop(Transform owner)
    {
        _loopingAudioDict.TryGetValue(owner, out var queue);
        if (queue == null || queue.Count == 0) return;

        var loopingAudio = queue.Dequeue();
        loopingAudio.StopLoop();
        if (queue.Count == 0) _loopingAudioDict.Remove(owner);
    }
    IEnumerator CoPlay(AudioSource source, Transform owner, bool is3D)
    {
        if (is3D)
        {
            source.transform.parent = owner;
            source.transform.localPosition = Vector3.zero;
        }

        source.Play();
        if (source.loop == false)
        {
            yield return new WaitForSeconds(source.clip.length);

            if (source != null)
            {
                _pool.Push(source);
                source.gameObject.SetActive(false);
            }
        }
    }
    #region BGM Control
    /// <summary>
    /// BGM 설정 및 재생 (페이드인 효과 지원)
    /// - 동일한 BGM이 이미 재생 중이면 무시
    /// - 자동 루프 재생
    /// - DOTween을 사용한 볼륨 페이드인
    /// </summary>
    /// <param name="bgm">재생할 BGM AudioClip</param>
    /// <param name="fadeInDuration">페이드인 지속 시간 (초 단위, 0이면 즉시 재생)</param>
    /// <param name="onFadeInCompleted">페이드인 완료 시 실행할 콜백</param>
    public void SetBgm(AudioKey.BGM key, Action onFadeInCompleted = null)
    {
        SetVolume(AudioMixerType.BGM, _originalBgmVolume);
        _audioDataDict.TryGetValue(key.ToString(), out var audioData);
        if (audioData == null)
        {
            Debug.LogError($"AudioManager Play Error! No AudioData Key : {key.ToString()}");
            return;
        }
        var bgmAudioData = audioData as BGMAudioData;
        _bgmSource.volume = bgmAudioData.GetVolume;
        SetBgm(bgmAudioData.AudioClip, bgmAudioData.FadeInTime, onFadeInCompleted);
    }
    void SetBgm(AudioClip bgm, float fadeInDuration = 0f, Action onFadeInCompleted = null)
    {
        if (_bgmSource.clip == bgm) return;

        _bgmSource.clip = bgm;
        _bgmSource.loop = true;
        _bgmSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups(AudioMixerType.BGM.ToString())[0];
        _bgmSource.Play();
        _bgmSource.DOFade(1f, fadeInDuration).onComplete += () => onFadeInCompleted?.Invoke();
    }
    /// <summary>
    /// BGM 정지 (페이드아웃 효과 지원)
    /// - DOTween을 사용한 볼륨 페이드아웃
    /// - 페이드아웃 완료 후 콜백 실행
    /// </summary>
    /// <param name="fadeOutDuration">페이드아웃 지속 시간 (초 단위, 0이면 즉시 정지)</param>
    /// <param name="onFadeOutCompleted">페이드아웃 완료 시 실행할 콜백</param>
    /// 
    float _originalBgmVolume = 1f;
    public void StopBgm(float fadeOutDuration = 0, Action onFadeOutCompleted = null)
    {
        _originalBgmVolume = GetVolume(AudioMixerType.BGM);
        _bgmSource.DOFade(0f, fadeOutDuration).onComplete += () => onFadeOutCompleted?.Invoke();
    }
    /// <summary>
    /// BGM 일시정지
    /// - 현재 재생 위치를 유지한 채 정지
    /// - ResumeBgm()으로 재개 가능
    /// </summary>
    public void PauseBgm()
    {
        _bgmSource.Pause();
    }
    /// <summary>
    /// BGM 일시정지 해제
    /// - PauseBgm()으로 정지한 BGM을 이어서 재생
    /// - 일시정지한 위치부터 계속 재생
    /// </summary>
    public void ResumeBgm()
    {
        _bgmSource.UnPause();
    }
    #endregion
    #region Volume Control
    /// <summary>
    /// AudioMixer 그룹의 볼륨 설정
    /// - MASTER: 전체 볼륨
    /// - BGM: 배경음악 볼륨
    /// - EFFECT: 효과음 볼륨
    /// - 데시벨 변환 자동 적용: Mathf.Log10(volume) * 20
    /// </summary>
    /// <param name="type">볼륨을 설정할 AudioMixer 타입</param>
    /// <param name="volume">볼륨 값 (0.0001 ~ 1.0, 로그 스케일로 변환)</param>
    public void SetVolume(AudioMixerType type, float volume, float fadeInDuration = 0f)
    {
        AudioMixer.SetFloat(type.ToString(), Mathf.Log10(0.001f) * 20);
        AudioMixer.DOSetFloat(type.ToString(), Mathf.Log10(volume) * 20, fadeInDuration);
    }
    public float GetVolume(AudioMixerType type)
    {
        AudioMixer.GetFloat(type.ToString(), out var dbVolume);
        return Mathf.Pow(10, dbVolume / 20);
    }
    #endregion
    /// <summary>
    /// 현재 AudioMixer 볼륨 설정값을 JSON 형태로 반환
    /// - MASTER, BGM, EFFECT의 데시벨 값을 선형 값(0~1)으로 변환
    /// - 저장/로드 시스템에서 사용
    /// </summary>
    /// <returns>볼륨 데이터가 담긴 JSON 문자열</returns>
    public string GetJsonVolumeData()
    {
        var json = new JObject();

        string bmgKey = AudioMixerType.BGM.ToString();
        string effectKey = AudioMixerType.EFFECT.ToString();
        string masterKey = AudioMixerType.MASTER.ToString();

        AudioMixer.GetFloat(bmgKey, out var bgmVolume);
        json.Add(bmgKey, JsonConvert.SerializeObject(Mathf.Pow(10, bgmVolume / 20)));
        AudioMixer.GetFloat(effectKey, out var effectVolume);
        json.Add(effectKey, JsonConvert.SerializeObject(Mathf.Pow(10, effectVolume / 20)));
        AudioMixer.GetFloat(masterKey, out var masterVolume);
        json.Add(masterKey, JsonConvert.SerializeObject(Mathf.Pow(10, masterVolume / 20)));

        return json.ToString();
    }
}