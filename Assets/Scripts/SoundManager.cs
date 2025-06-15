using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FadeMode
{
    In,
    Out
}

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource sfxLowerSource;
    public AudioSource sfxLowestSource;    
    public AudioSource musicSource;
    public float musicMaximumVolume;

    public static SoundManager instance = null;

    // O conceito do SoundManager é muito bom.
    // Mas eu queria deixar ele 100% estático. Tem que ver issae.

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip audioClip)
    {
        sfxSource.clip = audioClip;
        sfxSource.Play();
    }

    public void PlayLowSound(AudioClip audioClip)
    {
        sfxLowerSource.clip = audioClip;
        sfxLowerSource.Play();
    }

    public void PlayLowestSound(AudioClip audioClip)
    {
        sfxLowestSource.clip = audioClip;
        sfxLowestSource.Play();
    }

    public void PlayMusic(AudioClip musicClip)
    {
        musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void FadeInMusic(AudioClip musicClip, float duration)
    {
        StartCoroutine(StartFadeMusic(musicClip, duration, FadeMode.In));
    }

    public void FadeOutMusic(float duration, bool StopOnFadeOut = true)
    {
        StartCoroutine(StartFadeMusic(null, duration, FadeMode.Out, StopOnFadeOut));
    }

    private IEnumerator StartFadeMusic(AudioClip musicClip, float duration, FadeMode fadeMode, bool StopOnFadeOut = true)
    {
        float currentTime = 0;
        float start = fadeMode == FadeMode.In ? 0f : musicSource.volume;
        float targetVolume = fadeMode == FadeMode.In ? musicMaximumVolume : 0f ;

        if (fadeMode == FadeMode.In)
        {
            PlayMusic(musicClip);
        }                

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if (fadeMode == FadeMode.Out && StopOnFadeOut)
        {
            StopMusic();
        }
        yield break;
    }
}
