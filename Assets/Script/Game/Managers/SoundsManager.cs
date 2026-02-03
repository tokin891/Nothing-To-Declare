using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;

    [SerializeField] AudioClip mainAmbient;
    [SerializeField] float speedTuningMainAmbient;
    [SerializeField] float targetVolumeMainAmbient = 0.65f;
    [SerializeField] AudioClip[] fsClip;

    public AudioSource mainAmbientSource;
    private List<AudioSource> additionalAmbientSource = new List<AudioSource>();
    private AudioSource fsSource;

    private float timerClearUslessAdditionalAmbient;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if(Time.time > timerClearUslessAdditionalAmbient)
        {
            timerClearUslessAdditionalAmbient = Time.time + 3;
            CheckIsTheresUslessAudioSource();
        }
    }

    public void Initialize()
    {
        if (mainAmbient != null)
        {
            PlayMainAmbient();
        }
    }

    private void PlayMainAmbient()
    {
        StartCoroutine(SetVolumeAudioSourceAndPlay(mainAmbientSource, speedTuningMainAmbient, targetVolumeMainAmbient, true, mainAmbient));
    }

    private IEnumerator SetVolumeAudioSourceAndPlay(AudioSource source, float speedAmplify, float targetValue, bool loop, AudioClip clip = null, float delay = 0, bool destroyAfterEnd = false)
    {
        float volumeStart = source.volume;
        if(clip != null)
        {
            source.clip = clip;
            source.Play();
        }
        source.loop = loop;

        float volume = source.volume;
        bool AddVolume = true;
        if(targetValue < source.volume)
        {
            AddVolume = false;
            speedAmplify = -speedAmplify;
        }

        while(((source.volume < targetValue) && AddVolume) || ((source.volume > targetValue) && !AddVolume))
        {
            source.volume += Time.deltaTime * speedAmplify;
            yield return null;
        }
        source.volume = targetValue;

        if(delay != 0)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(SetVolumeAudioSourceAndPlay(source, speedAmplify, 0, false, null, 0, true));
        }
        if (destroyAfterEnd)
        {
            Destroy(source.gameObject);
        }
    }
    private IEnumerator SetVolumeAudioSourceAndReturnIt(AudioSource source, float speedAmplify, float targetValue, bool loop,float delay, AudioClip clip = null)
    {
        float volumeStart = source.volume;
        Coroutine cr = StartCoroutine(SetVolumeAudioSourceAndPlay(source, speedAmplify, targetValue, loop, clip));
        yield return new WaitForSeconds(delay);
        if (cr != null)
        {
            StopCoroutine(cr);
        }
        StartCoroutine(SetVolumeAudioSourceAndPlay(source, speedAmplify, volumeStart, loop, clip));
    }

    private IEnumerator PlayAudioCoroutine(AudioSource source, AudioClip clip, float targetValue, bool destroy, bool loop)
    {
        source.volume = targetValue;
        source.clip = clip; 
        source.loop = loop;
        source.Play();
        if (destroy)
        {
            yield return new WaitForSeconds(clip.length);
            Destroy(source.gameObject);
        }
    }

    public void PlayAudio(AudioClip clip, float speedAmplify, float targetValue, bool loop, float delay)
    {
        StartCoroutine(SetVolumeAudioSourceAndPlay(GetAdditionalAudioSource(), speedAmplify, targetValue, loop, clip,delay));
    }

    public void PlayAudioAndBlendWithAudioSourceForTime(AudioClip clip, float speedAmplify, float targetValue, bool loop, float delay, AudioSource blendSource, float blendTargetValue)
    {
        StartCoroutine(SetVolumeAudioSourceAndReturnIt(blendSource, speedAmplify, blendTargetValue, true, delay));
        StartCoroutine(SetVolumeAudioSourceAndPlay(GetAdditionalAudioSource(), speedAmplify, targetValue, loop, clip, delay));
    }

    public void PlayOneTimeClip(AudioClip clip, float targetValue)
    {
        StartCoroutine(PlayAudioCoroutine(GetAdditionalAudioSource(), clip, targetValue, true, false));
    }

    public void PlayOneTimeClipWithoutDestroying(AudioClip clip, float targetValue, bool loop)
    {
        StartCoroutine(PlayAudioCoroutine(GetAdditionalAudioSource(), clip, targetValue, false, loop));
    }

    public void PlayFS()
    {
        if (fsSource == null)
            fsSource = GetAdditionalAudioSource();
        StartCoroutine(PlayAudioCoroutine(fsSource, fsClip[Random.Range(0,fsClip.Length)], 0.75f, false, false));
    }

    public void DestroyAudioSourceByClipFromAdditionalAudio(AudioClip clip)
    {
        foreach (AudioSource source in additionalAmbientSource)
            if (source != null && source.clip == clip)
                Destroy(source.gameObject);
    }

    private AudioSource GetAdditionalAudioSource()
    {
        GameObject audioObject = new GameObject();
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.volume = 0.05f;
        additionalAmbientSource.Add(source);
        return source;
    }

    private void CheckIsTheresUslessAudioSource()
    {
        for (int i = 0; i < additionalAmbientSource.Count; i++)
        {
            if (additionalAmbientSource[i] != null && additionalAmbientSource[i].volume == 0)
                Destroy(additionalAmbientSource[i]);
        }
    }
}
