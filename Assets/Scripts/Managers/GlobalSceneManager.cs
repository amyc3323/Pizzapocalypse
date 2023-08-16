using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GlobalSceneManager : MonoBehaviour
{

    public static GlobalSceneManager instance { private set; get; }
    public Pizza currentPizza;
    public List<Ingredient> storedIngredients = new List<Ingredient>();
    public AudioClip playerDefeatSound;
    public UnityEvent onChangeSceneEvent;
    public int currentBuildIndex;
    [SerializeField] List<IngredientScriptableObject> allIngredients;
    public IngredientScriptableObject getIngredientType(int id,out bool success)
    {
        foreach (IngredientScriptableObject iso in allIngredients)
        {
            if (iso.trueOrder == id) { success = true; return iso; }
        }
        Debug.Log("ID out of bounds: " + id);
        success = false;
        return null;
    }
    public int getIngredientID(IngredientScriptableObject iso)
    {
        if (iso == null) return -1;
        return iso.trueOrder;
    }
    public void SaveScores(int[] scores)
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt("Highscore" + i.ToString(), scores[i]);
        }
    }

    public int[] GetHighScores()
    {
        int[] ret=new int[10];
        for (int i = 0; i < 10; i++) { ret[i] = PlayerPrefs.GetInt("Highscore" + i.ToString()); }
        Array.Sort(ret);
        return ret;
    }

    public void AddScore(int score)
    {
        int[] scores = GetHighScores();
        int cVal = score;
        for(int i = 0; i < 10; i++)
        {
            if (cVal > scores[i]) { int tmp = scores[i];scores[i] = cVal;cVal = tmp; }
        }
        SaveScores(scores);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        
    }

    private void Start()
    {
        Load();
    }

    private void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.Space))
         {
             Debug.Log("Total score: " + GetTotalHighScore());
         }*/

        GetBuildIndex();
    }

    public void GetBuildIndex()
    {
        currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        Time.timeScale = 1;
        onChangeSceneEvent.Invoke();
        Save();
        SceneManager.LoadScene(0);
        onChangeSceneEvent.RemoveAllListeners();
    }

    public void ExitGame()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClose, 1);
        if (SceneManager.GetActiveScene().buildIndex > 0) OpenMainMenu();
        else QuitGame();
    }

    public void OpenGameScene()
    {
        onChangeSceneEvent.Invoke();
        Time.timeScale = 1;
        Save();
        Debug.Log(PlayerPrefs.GetInt("tutorial"));
        if (PlayerPrefs.GetInt("tutorial") != 1) // has not played tutorial
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
        onChangeSceneEvent.RemoveAllListeners();
    }

    public void OpenPizzeria()
    {
        Time.timeScale = 1;
        onChangeSceneEvent.Invoke();
        Save();
        SceneManager.LoadScene(2);
        onChangeSceneEvent.RemoveAllListeners();
    }

    public void PlayerDie()
    {
        GlobalSoundManager.instance.playSFX(playerDefeatSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void RestartLevel()
    {
        onChangeSceneEvent.Invoke();
        Save();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        onChangeSceneEvent.RemoveAllListeners();
    }

    #region saving
    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
        else Load();
    }
    
    private void OnApplicationQuit()
    {
        Save();

    }
    public void Save()
    {
        GlobalSaveManager.instance.SaveData("CurrentPizza", currentPizza);
        
    }

    public void Load()
    {
        string data = "";
        
        if(GlobalSaveManager.instance.LoadData("CurrentPizza", out data))
        {
            currentPizza.LoadJson(data);
        }

    }
    #endregion
}
