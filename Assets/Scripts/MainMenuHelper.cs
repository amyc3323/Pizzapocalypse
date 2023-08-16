using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHelper : MonoBehaviour
{
    public void OpenPizzeria()
    {
        GlobalSceneManager.instance.OpenPizzeria();
    }
    public void OpenGame()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiOpen);
        GlobalSceneManager.instance.OpenGameScene();
    }
    public void OpenOptionMenu()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiOpen);
        OptionCanvasManager.instance.OpenMenu();
    }
    public void OpenHighScore()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiOpen);
    }
}
