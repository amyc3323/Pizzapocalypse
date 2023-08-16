using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroupHelper))]
public class OptionCanvasManager : MonoBehaviour
{
    public static OptionCanvasManager instance;
    private CanvasGroupHelper canvasGroup;
    [SerializeField] private Slider globalVolumeSlider;

    [SerializeField] private Slider musicVolumeSlider;

    [SerializeField] private Slider sfxVolumeSlider;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        canvasGroup = GetComponent<CanvasGroupHelper>();
    }
    private void Start()
    {
        globalVolumeSlider.value = GlobalSoundManager.instance.globalVolume;

        musicVolumeSlider.value = GlobalSoundManager.instance.musicVolume;

        sfxVolumeSlider.value = GlobalSoundManager.instance.sfxVolume;

    }
    public void OpenMenu()
    {
        Time.timeScale = 0;
        canvasGroup.SetOn();
    }
    public void UpdateGlobalVolume(float val)
    {
        GlobalSoundManager.instance.globalVolume = val;
        GlobalSoundManager.instance.setMusicVolume();
    }
    public void UpdateMusicVolume(float val)
    {

        GlobalSoundManager.instance.musicVolume = val;
        GlobalSoundManager.instance.setMusicVolume();
    }
    public void UpdateSFXVolume(float val)
    {
        GlobalSoundManager.instance.sfxVolume = val;

    }
    public void CloseMenu()
    {
        StartCoroutine(delayTimeScale());
        canvasGroup.SetOff();
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClose, 1);
    }
    IEnumerator delayTimeScale()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
    }
    public void ExitGame()
    {
        GlobalSceneManager.instance.ExitGame();
    }
}
