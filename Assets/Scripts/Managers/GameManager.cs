using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public int pizzaAmount;
    [SerializeField] public List<DeliveryZone> possibleDeliveryLocations = new List<DeliveryZone>();
    List<int> randomizedIndex;
    int currInd;
    [SerializeField] private float minDistance;
    [SerializeField] private Pizza[] pizzaList;
    private int totalPizzaWeight;
    [SerializeField] public int money { private set; get; }
    public int currDeliveryZone = -1;

    public AudioClip successfulDeliveryClip;
    public AudioClip failedDeliveryClip;
    public AudioClip gameOverClip;
    public AudioClip refillClip;

    [SerializeField] private float remainingDeliveryTime=20;
    [SerializeField] private float originalDeliveryTime=20;
    public int finishedDeliveries=0;
    private static int finishedDeliveriesUnit=5;
    public int failedDeliveries = 0;
    [SerializeField]private int reputation;
    [SerializeField] private int reputationLostPerDelivery = 20;

    [Header("Buff Multipliers")]
    public float timeMultiplier=1f;
    public float reputationMultiplier=1f;
    public float pizzaThrowingMultiplier=1f;
    public float zombieSpeedMultplier=1f;
    public int maxPizza=5;

    private float CalculateDeliveryTime(float distance, int finishedDeliveries)
    {
        return timeMultiplier*1.5f*(5+distance / (1 + Mathf.Sqrt(finishedDeliveries*2   )));
    }
    public void AddMoney(int amt)
    {
        money += amt;
        GameCanvasManager.instance.UpdateMoney(money);
    }

    private void SpawnZombies()
    {

    }
    public void GameOver()
    {
        GlobalSoundManager.instance.playSFX(gameOverClip);
       GlobalSceneManager.instance.AddScore(money); 
        GameOverCanvasHelper.instance.UpdateText(); 
        GameOverCanvasHelper.instance.canvasGroup.SetOn(); 
        Time.timeScale = 0;
        GlobalSoundManager.instance.StopLoopSFX();

    }
    public bool FinishDelivery(Pizza pizza)
    {
        finishedDeliveries++;
        if (pizzaAmount > 0) {

            reputation += CalculateReputationFromTime();
            reputation = Mathf.Clamp(reputation, 0, 500);
            GameCanvasManager.instance.UpdateReputation(reputation);
            GlobalSoundManager.instance.playSFX(successfulDeliveryClip);
            AddMoney(Random.Range(pizza.minPrice, pizza.maxPrice));
            pizzaAmount -= 1;
            GameCanvasManager.instance.UpdatePizzaAmount(pizzaAmount);
            Debug.Log("next deliv");
            NextDelivery();
            return true;
        }

        return false;
    }

    public void FinishThrowingDelivery(Pizza pizza)
    {
        finishedDeliveries++;

            reputation += CalculateReputationFromTime();
            reputation = Mathf.Clamp(reputation, 0, 500);
            GameCanvasManager.instance.UpdateReputation(reputation);
            GlobalSoundManager.instance.playSFX(successfulDeliveryClip);
            AddMoney(Random.Range(pizza.minPrice, pizza.maxPrice));
            GameCanvasManager.instance.UpdatePizzaAmount(pizzaAmount);
            NextDelivery();
    }
    public int CalculateReputationFromTime()
    {
        return Mathf.RoundToInt(reputationMultiplier*8 * (remainingDeliveryTime / originalDeliveryTime) + 2);
    }
    public void FailedDelivery()
    {
        failedDeliveries++;
        reputation -= reputationLostPerDelivery;
        if (reputation <= 0)
        {
            GlobalSoundManager.instance.StopLoopSFX();
            GlobalSceneManager.instance.AddScore(money); GameOverCanvasHelper.instance.UpdateText(); GameOverCanvasHelper.instance.canvasGroup.SetOn();Time.timeScale = 0; }
        reputation = Mathf.Clamp(reputation, 0, 500);
        GameCanvasManager.instance.UpdateReputation(reputation);
        GlobalSoundManager.instance.playSFX(failedDeliveryClip, 2);
        possibleDeliveryLocations[currDeliveryZone].DisableZone();
        NextDelivery();


    }
    public bool RemovePizzas(int amt)
    {
        if (pizzaAmount >= amt) {
            pizzaAmount -= amt;
            GameCanvasManager.instance.UpdatePizzaAmount(pizzaAmount);
            return true; 
        }
        return false;
    }
    public void NextDelivery()
    {
        // this is for the old system
        /*
        for (int i = 0; i < possibleDeliveryLocations.Count; i++)
        {
            DeliveryZone del = possibleDeliveryLocations[i];
            if ((del.transform.position - PlayerScript.Instance.transform.position).sqrMagnitude < minDistance * minDistance) { continue; }

            del.zoneDisplay.enabled = true;
            currDeliveryZone = i;
            DeliveryTarget.instance.transform.position = del.transform.position; break;
        }
        */
        // new randomized system 
        possibleDeliveryLocations[randomizedIndex[currInd]].zoneDisplay.enabled = true;
        possibleDeliveryLocations[randomizedIndex[currInd]].col.enabled = true;
        currDeliveryZone = randomizedIndex[currInd];
        DeliveryTarget.instance.transform.position = possibleDeliveryLocations[randomizedIndex[currInd]].transform.position;
        originalDeliveryTime= CalculateDeliveryTime((DeliveryTarget.instance.transform.position - PlayerScript.instance.transform.position).magnitude, finishedDeliveries);
        remainingDeliveryTime = originalDeliveryTime;
        currInd++;
        if (currInd >= randomizedIndex.Count)
            currInd = 0;
        DeliveryTarget.instance.ResetDelivery(GetRandomPizza());
    }

    public void ResetDeliveryTime()
    {
        remainingDeliveryTime = originalDeliveryTime;
    }

    public float DeliveryTimeRatio()
    {
        return remainingDeliveryTime / originalDeliveryTime;
    }

    public void RandomizeDeliveryList()
    {
        //Fisher-Yates Algorithm and also Durstenfeld
        int size = possibleDeliveryLocations.Count;
        //init
        randomizedIndex = new List<int>();
        for (int i = 0; i < size; i++)
        {
            randomizedIndex.Add(i);
        }

        for(int i = size - 1; i >= 0; i--)
        {
            int swapInd = Random.Range(0, i + 1);
            int temp = randomizedIndex[i];
            randomizedIndex[i] = randomizedIndex[swapInd];
            randomizedIndex[swapInd] = temp;
        }

        // yay randomied
    }

    public Pizza GetRandomPizza()
    {
        if (GlobalSceneManager.instance == null)
        {
            return TutorialManager.instance.testPizza;
        }
        return GlobalSceneManager.instance.currentPizza;
    }
    //private void UpdateTotalPizzaWeight(){int t = 0; foreach(Pizza p in pizzaList) { t += p.weight; }totalPizzaWeight = t;}

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(Ingredient ig in GlobalSceneManager.instance.currentPizza.ingredientList)
        {
            foreach(IngredientBenefitBase benefit in ig.type.benefits)
            {
                benefit.Benefit();
            }
        }
        Debug.Log("PizzaThrowingMultiplier: " + pizzaThrowingMultiplier);
        Physics2D.IgnoreLayerCollision(0, 9, pizzaThrowingMultiplier > 1f);
        currInd = 0;
        GameCanvasManager.instance.UpdatePizzaAmount(pizzaAmount);
        GameCanvasManager.instance.UpdateMoney(money);
        GameCanvasManager.instance.UpdateReputation(reputation);

    }

    // Update is called once per frame
    void Update()
    {
        if (randomizedIndex == null)
        {
            RandomizeDeliveryList();
            NextDelivery();
        }


        if (remainingDeliveryTime <= 0)
        {
            Debug.Log($"Remaining Time: {remainingDeliveryTime}");
            FailedDelivery();
        }
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        remainingDeliveryTime -= Time.deltaTime;
        GameCanvasManager.instance.UpdateTimer(Mathf.Clamp01(remainingDeliveryTime / originalDeliveryTime));
    }

    public void RefillPizzas()
    {
        if (pizzaAmount != maxPizza)
        {
            pizzaAmount = maxPizza;
            GameCanvasManager.instance.UpdatePizzaAmount(pizzaAmount);
            GlobalSoundManager.instance.playSFX(refillClip);
        }
    }
    private void OnApplicationPause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
}
