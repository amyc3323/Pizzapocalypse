using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSoundManager : MonoBehaviour
{
    public static GlobalSoundManager instance;
    [Range(0f, 1f)]
    public float globalVolume = 1f;
    [Header("Music")]
    public AudioClip[] musicClips;
    public AudioSource musicAudioSource;

    public AudioClip uiOpen;
    public AudioClip uiClose;
    public AudioClip uiClick;
    public AudioClip bookFlipOpen;
    public AudioClip bookFlipClose;
    public AudioClip fireSound;

    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Header("SFX")]
    public AudioSource sfxAudioSource;
    public AudioSource sfxLoopAudioSource;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private int previousFrameBuildIndex;



    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        previousFrameBuildIndex = GlobalSceneManager.instance.currentBuildIndex;
    }
    private void Update()
    {
        if (GlobalSceneManager.instance.currentBuildIndex != previousFrameBuildIndex)
        {
            if (musicClips[previousFrameBuildIndex] != musicClips[GlobalSceneManager.instance.currentBuildIndex])
            {
                resetMusic();
            }
            previousFrameBuildIndex = GlobalSceneManager.instance.currentBuildIndex;
        }

        if (GlobalSceneManager.instance.currentBuildIndex == 2)
        {
            playLoopSFX(fireSound);
            SetLoopSFXVolume(0.5f);
        }

        PlayMusic();
    }

    private void PlayMusic()
    {
        setMusicVolume();
        if (!musicAudioSource.isPlaying)
        {
            musicAudioSource.clip = musicClips[GlobalSceneManager.instance.currentBuildIndex];
            musicAudioSource.Play();
        }

    }
    public void resetMusic()
    {
        musicAudioSource.Stop();
    }

    public void playSFX(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip, sfxVolume * globalVolume * 2);
    }


    public void playSFX(AudioClip clip, float volume)
    {
        sfxAudioSource.PlayOneShot(clip, volume*sfxVolume * globalVolume * 2);
    }

    public void playLoopSFX(AudioClip clip)
    {
        if (clip.Equals(sfxLoopAudioSource.clip) && sfxLoopAudioSource.isPlaying) return;
        sfxLoopAudioSource.clip = clip;
        sfxLoopAudioSource.Play();
    }
    public void SetLoopSFXVolume(float volume)
    {
        sfxLoopAudioSource.volume = volume * sfxVolume * globalVolume * 2;

    }

    public void SetLoopSFXPitch(float pitch)
    {
        sfxLoopAudioSource.pitch=pitch;

    }
    public void StopLoopSFX()
    {
        sfxLoopAudioSource.clip = null;
        sfxLoopAudioSource.Stop();
    }
    public void setMusicVolume()
    {
        musicAudioSource.volume = musicVolume * globalVolume * 2;
    }
}
