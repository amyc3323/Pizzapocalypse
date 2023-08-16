using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TutoriaButtonHelper : MonoBehaviour
{
    
    public void OpenTutorial()
    {

        PlayerPrefs.SetInt("tutorial", 0);
        GlobalSceneManager.instance.OpenGameScene();
    }
}
