using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Âm thanh nền")]
    public AudioClip mainThemeSound;
    public AudioClip endingThemeSound;
    public AudioSource musicSource;

    [Header("Âm thanh hiệu ứng")]
    public AudioClip noiseSound;
    public AudioClip birdSound;
    public AudioClip catSound;
    public AudioClip alarmSound;
    public AudioClip tickSound;
    public AudioClip UISound;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ------------------ Âm thanh nền ------------------

    public void PlayMainTheme()
    {
        PlayMusic(mainThemeSound);
    }

    public void PlayEndingTheme()
    {
        PlayMusic(endingThemeSound);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // ------------------ Âm thanh hiệu ứng ------------------

    public void PlayNoise()
    {
        PlaySFX(noiseSound);
    }

    public void PlayBird()
    {
        PlaySFX(birdSound);
    }

    public void PlayCat()
    {
        PlaySFX(catSound);
    }

    public void PlayAlarm()
    {
        PlaySFX(alarmSound);
    }

    public void PlayTick()
    {
        PlaySFX(tickSound);
    }

    public void PlayUISound()
    {
        PlaySFX(UISound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayCustomSFX(AudioClip clip)
    {
        PlaySFX(clip);
    }

    public void StopSFX()
    {
        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }
}
