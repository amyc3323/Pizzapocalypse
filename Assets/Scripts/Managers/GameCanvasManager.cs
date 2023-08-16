using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameCanvasManager : MonoBehaviour
{
    public GameObject[] healthIcons;
    public static GameCanvasManager instance;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text pizzaAmountText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Image timerImage;
    [SerializeField] private Image redSmiley;
    [SerializeField] private Image yellowSmiley;
    [SerializeField] private Image greenSmiley;
    [Header("Reputation")]
    [SerializeField] private Image circularFill;
    [SerializeField] private Image[] stars;


    private void Awake()
    {
        instance = this;
    }
    public void UpdateHealth(int health)
    {
        return;
        for(int i = 0; i < healthIcons.Length; i++)
        {
            healthIcons[i].SetActive(i < health);
        }
    }
    public void UpdateMoney(int amount)
    {
        moneyText.text = $"${amount}";
    }
    public void UpdatePizzaAmount(int amt)
    {
        pizzaAmountText.text = amt.ToString();
    }
    public void UpdateTimer(float value)
    {
        timerImage.fillAmount = value;
        greenSmiley.fillAmount = Mathf.Clamp01((value - 0.9f)) * 10;


        yellowSmiley.fillAmount = Mathf.Clamp01((value - 0.45f) * 10) ;
        redSmiley.fillAmount = Mathf.Clamp01((value - 0f) * 10) ;
        //timerSlider.value = value;
    }
    //Value goes between 0 and 500
    public void UpdateReputation(float value)
    {
        circularFill.fillAmount = (value % 100)/100f;
        for(int i = 0; i < stars.Length; i++)
        {
            stars[i].fillAmount = Mathf.Clamp01((value - 100f * i) / 100f);
        }
    }

}
