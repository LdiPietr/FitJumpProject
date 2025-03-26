using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip jumpSound;
    public AudioClip shield;
    public AudioClip jet;
    public AudioClip enemyDeath;
    public AudioClip gameOverSound;

    public bool isMusicPlaying;
    public bool isSFXPlaying;

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

    private void Update()
    {
        isMusicPlaying = musicSource.volume > 0;
        isSFXPlaying = sfxSource.volume > 0;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void PlayGameOverSound()
    {
        sfxSource.PlayOneShot(gameOverSound);
    }
}