using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HighScoreManager : MonoBehaviour
{
    int[] scores;
    public TMP_Text[] highscoreText;
    private void Start()
    {
        scores = GlobalSceneManager.instance.GetHighScores();
        UpdateDisplay();
    }
    public void UpdateDisplay()
    {
        for(int i = 0; i < 10; i++)
        {
            Debug.Log("Score: " + scores[9-i]);
            if (scores[9 - i] == 0) { highscoreText[i].text = ""; }
            else highscoreText[i].text = scores[9-i]+"$";
        }
    }
}
