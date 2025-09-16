using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

public enum AudioType { MASTER, BGM, EFFECT }

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource prefab;
    private Queue<AudioSource> pool = new Queue<AudioSource>();
    AudioSource _bgmSource;
    [SerializeField]
    AudioMixer AudioMixer;
    void Start()
    {
        if (Manager.Audio != null)
        {
            Object.Destroy(gameObject);
            return;
        }
        Manager.Audio = this;
        _bgmSource = GetComponent<AudioSource>();
    }
    /// <summary>
    /// 오디오 클립 한번 재생(재생이 끝난 후 오디오 소스 풀에 반환)
    /// </summary>
    /// <param name="clip">효과음 등 오디오 클립</param>
    /// <param name="position">오디오 실행 위치</param>
    public void PlayOneShot(AudioClip clip, Vector3 position)
    {
        AudioSource source = GetSource();
        source.transform.parent = transform;
        source.transform.position = position;
        source.PlayOneShot(clip);
        StartCoroutine(ReleaseAfter(source, clip.length));
    }
    private AudioSource GetSource()
    {
        if (pool.Count > 0) return pool.Dequeue();
        return Instantiate(prefab);
    }
    private IEnumerator ReleaseAfter(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        pool.Enqueue(source);
    }
    /// <summary>
    /// bgm 설정 및 재생(만약 기존 bgm과 같다면 새로 재생하지 않음)
    /// </summary>
    /// <param name="bgm">브금 오디오 클립</param>
    public void SetBgm(AudioClip bgm)
    {
        if (_bgmSource.clip == bgm) return;
        _bgmSource.clip = bgm;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }
    /// <summary>
    /// 볼륨 설정
    /// </summary>
    /// <param name="type">볼륨 설정할 오디오 타입</param>
    /// <param name="volume">볼륨 값</param>
    public void SetVolume(AudioType type, float volume)
    {
        AudioMixer.SetFloat(type.ToString(), Mathf.Log10(volume) * 20);
    }
}