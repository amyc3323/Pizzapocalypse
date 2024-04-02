using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum TutorialState
{
    Joystick,
    Drift,
    Boost,
    Delivery,
    Throw,
    PizzaCount,
    Zombie,
    Timer,
}

[System.Serializable]
public class ObjectGroup
{
    public GameObject[] gameObjects;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    GameManager gameManager;
    public TutorialState state;
    public ObjectGroup[] toActivate; // each index corresponds to the enum index
    public GameObject[] objectToActivate;
    public Pizza testPizza;
    public ZombiePool zombiePool;
    public TextMeshProUGUI tutorialText;
    // 77 units per 4 lines
    // 15.4 as the margin
    public RectTransform textBackground;
    PlayerScript player;
    Rigidbody2D playerRB;
    Vector3 originPos;

    //advancement values
    [SerializeField] float sqrDistanceThreshold;
    [SerializeField] float timeToDrift;
    [SerializeField] float driftTimeLimit;
    float driftTime;
    [SerializeField] float timeToDelivery;
    public int throwDeliveries;
    int initialDeliveryCount;
    [SerializeField] int minKillCount;
    public int killCount;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        zombiePool = ZombiePool.instance;
        gameManager = GameManager.instance;
        gameManager.pizzaAmount = 0;
        state = TutorialState.Joystick;
        player = PlayerScript.instance;
        playerRB = player.GetComponent<Rigidbody2D>();
        originPos = player.transform.position;
        driftTime = 0f;
        throwDeliveries = 0;
        killCount = 0;

        //disable all those delivery locations
        foreach (DeliveryZone i in gameManager.possibleDeliveryLocations)
        {
            i.DisableZone();
        }
    }

    IEnumerator SetStateToDrift()
    {
        yield return new WaitForSeconds(timeToDrift);
        IncrementState(TutorialState.Drift);
    }

    IEnumerator SetStateToDelivery()
    {
        yield return new WaitForSeconds(timeToDelivery);
        if ((int)state == (int)TutorialState.Delivery - 1) //only run if the state has not been incremented to delivery yet
        {
            foreach (DeliveryZone i in gameManager.possibleDeliveryLocations)
            {
                i.DisableZone();
            }
        }
        IncrementState(TutorialState.Delivery);
        
        gameManager.pizzaAmount = 99;
    }

    void IncrementState(TutorialState newState)
    {
        if ((int)state == (int)newState - 1)
        {
            state++;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (GameObject i in toActivate[(int)state].gameObjects)
        {
            i.SetActive(true);
        }


        switch (state)
        {
            case TutorialState.Joystick:
                if ((player.transform.position - originPos).sqrMagnitude > sqrDistanceThreshold)
                {
                    StartCoroutine(SetStateToDrift());
                }
                return;
            case TutorialState.Drift:
                //player has to drift for a set amount of time to progress
                if (!player.isDrifting)
                {
                    driftTime = 0;
                }
                else
                {
                    driftTime += Time.deltaTime;
                }
                if (driftTime > driftTimeLimit)
                {
                    IncrementState(TutorialState.Drift + 1);
                }
                return;
            case TutorialState.Boost:
                if (player.boostPressed && player.boostMeter > 0)
                {
                    StartCoroutine(SetStateToDelivery());
                }
                return;
            case TutorialState.Delivery:
                if (gameManager.finishedDeliveries >= 3)
                {
                    IncrementState(TutorialState.Delivery + 1);
                }
                return;
            case TutorialState.Throw:
                if (throwDeliveries >= 2) //This variable is updated by DeliveryTarget.cs
                {
                    IncrementState(TutorialState.Throw + 1);
                    gameManager.pizzaAmount = 0;
                    initialDeliveryCount = gameManager.finishedDeliveries;
                }
                return;
            case TutorialState.PizzaCount:
                if (gameManager.finishedDeliveries - initialDeliveryCount >= 2) //Player has to complete a certain amount of new deliveries
                {
                    IncrementState(TutorialState.PizzaCount + 1);
                    zombiePool.poolCount = 25;
                    zombiePool.InstantiateZombies();

                    initialDeliveryCount = gameManager.finishedDeliveries;
                }
                return;
            case TutorialState.Zombie:
                if (killCount >= minKillCount && gameManager.finishedDeliveries - initialDeliveryCount >= 1)
                {
                    IncrementState(TutorialState.Zombie + 1);
                    initialDeliveryCount = gameManager.finishedDeliveries;
                }
                return;
            case TutorialState.Timer:
                if (gameManager.finishedDeliveries - initialDeliveryCount >= 3)
                {
                    //done with tutorial
                    SceneManager.LoadScene(1);
                }
                return;
        }

        if (gameManager.DeliveryTimeRatio() < 0.2f)
            gameManager.ResetDeliveryTime();
        if ((int)state > (int)TutorialState.Zombie)
            gameManager.pizzaAmount = Mathf.Clamp(gameManager.pizzaAmount, 0, 99);
        if ((int)state < (int)TutorialState.PizzaCount) // infinite pizzas before pizza count is unlocked
        {
            gameManager.pizzaAmount = 5;
        }
        //based on tutorial text, set dimension
        textBackground.sizeDelta = new Vector2(textBackground.sizeDelta.x, (77 / 4.0f) * tutorialText.textInfo.lineCount + 15.4f);
    }
}
