using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;

    [SerializeField] AudioClip mainAmbient;
    [SerializeField] float speedTuningMainAmbient;
    [SerializeField] float targetVolumeMainAmbient = 0.65f;

    [SerializeField] AudioSource mainAmbientSource;

    private List<AudioSource> additionalAmbientSource = new List<AudioSource>();

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
        StartCoroutine(PlaySound(mainAmbientSource, mainAmbient, speedTuningMainAmbient, targetVolumeMainAmbient, true));
    }

    private IEnumerator PlaySound(AudioSource source, AudioClip clip, float speedAmplify, float targetValue, bool loop, float delay = 0, bool destroyAfterEnd = false)
    {
        source.clip = clip;
        source.loop = loop;
        source.Play();

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
            StartCoroutine(PlaySound(source, clip, speedAmplify, 0, false,0,true));
        }
        if (destroyAfterEnd)
        {
            Destroy(source.gameObject);
        }
    }

    public void PlayAudio(AudioClip clip, float speedAmplify, float targetValue, bool loop, float delay)
    {
        StartCoroutine(PlaySound(GetAdditionalAudioSource(), clip, speedAmplify, targetValue, loop, delay));
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
            if (additionalAmbientSource[i].volume == 0)
                Destroy(additionalAmbientSource[i]);
        }
    }
}
