using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum TutorialState
{
    Joystick,
    Boost,
    Drift,
    Pizza,
    Zombie,
    Delivery
}

[System.Serializable]
public class UIObjectGroup
{
    public GameObject[] UIObjects;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public TutorialState state;
    public UIObjectGroup[] toActivate; // each index corresponds to the enum index
    public Pizza testPizza;
    public GameObject zombieInScene;
    public TextMeshProUGUI tutorialText;
    // 77 units per 4 lines
    // 15.4 as the margin
    public RectTransform textBackground;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.pizzaAmount = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerScript player = PlayerScript.instance;
        foreach (GameObject i in toActivate[(int)state].UIObjects)
        {
            i.SetActive(true);
        }
        switch (state)
        {
            case TutorialState.Joystick:
                if (player.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 1)
                    state++;
                tutorialText.text = "Drag the joystick to move";
                break;
            case TutorialState.Boost:
                if (player.boostPressed && player.boostMeter > 0)
                    state++;
                tutorialText.text = "Crash into an obstacle to build boost. You can press the boost button to temporarily speed up.";
                break;
            case TutorialState.Drift:
                if (player.isDrifting)
                {
                    GameManager.instance.pizzaAmount = 0;
                    GameCanvasManager.instance.UpdatePizzaAmount(GameManager.instance.pizzaAmount);
                    state++;
                }
                tutorialText.text = "Try building up speed then press the drift button when you do turns on this road. It helps maintain your momentum and increases your boost.";
                break;
            case TutorialState.Pizza:

                if (GameManager.instance.pizzaAmount > 0)
                {
                    state++;
                    
                }
                tutorialText.text = "Get Pizza from the Pizzeria. You can use the small joystick on the right to throw pizzas.";
                break;
            case TutorialState.Zombie:
                if (!zombieInScene.activeInHierarchy) // zombie was killed
                {
                    state++;
                    GameManager.instance.ResetDeliveryTime();
                }
                tutorialText.text = "Up ahead is a zombie. It really likes eating pizzas, either from your bike or from the ground. Try to kill it by crashing into it with high speed.";
                break;
            case TutorialState.Delivery:
                if (GameManager.instance.money > 0)
                {
                    // go to next scene ig
                    Debug.Log("tutorial done");
                    PlayerPrefs.SetInt("tutorial", 1);
                    SceneManager.LoadScene(1); // 1 means game scene
                }
                tutorialText.text = "The green arrow points to the delivery zone. Deliver the pizza by either throwing it or entering the zone. Make deliveries within the time limit to maintian reputation. No reputation leads to a loss.";
                break;
        }

        //based on tutorial text, set dimension

        if (GameManager.instance.DeliveryTimeRatio() < 0.2f)
            GameManager.instance.ResetDeliveryTime();
        if ((int)state > (int)TutorialState.Zombie)
            GameManager.instance.pizzaAmount = Mathf.Clamp(GameManager.instance.pizzaAmount, 0, 99);
        textBackground.sizeDelta = new Vector2(textBackground.sizeDelta.x, (77 / 4.0f) * tutorialText.textInfo.lineCount + 15.4f);
    }
}
