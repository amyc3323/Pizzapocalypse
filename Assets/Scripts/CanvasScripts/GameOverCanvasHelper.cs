using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameOverCanvasHelper : MonoBehaviour
{
    public CanvasGroupHelper canvasGroup;
    public static GameOverCanvasHelper instance;
    public TMP_Text moneyText;
    public TMP_Text timeText;
    public TMP_Text pizzaText;
    public TMP_Text failedText;
    public void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    public void UpdateText()
    {
        moneyText.text = GameManager.instance.money + "$";
        timeText.text = ConvertToTime();
        pizzaText.text = GameManager.instance.finishedDeliveries.ToString();
        failedText.text = GameManager.instance.failedDeliveries.ToString();
    }
    public string ConvertToTime()
    {
        string ret = "";
        int time = Mathf.RoundToInt(Time.timeSinceLevelLoad);
        int min = (time % 3600) / 60;

        if (time > 60 * 60)
        {
            ret = $"{time / 3600}:";
            if (min < 10) { ret += "0"; }

        }
        ret += min + ":";
        int sec = time % 60;
        if (sec < 10) { ret += "0"; }
        ret += sec;
        return ret;
    }
    public void RestartGame()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClose);
        GlobalSceneManager.instance.RestartLevel();
    }
    public void ExitGame()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClose);
        GlobalSceneManager.instance.OpenPizzeria();
    }
}
