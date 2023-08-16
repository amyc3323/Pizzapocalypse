using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzariaCanvasManager : MonoBehaviour
{
    public static PizzariaCanvasManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this; 
    }
    public void ExitPizzeria()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClose);
        GlobalSceneManager.instance.OpenMainMenu();
    }
}
