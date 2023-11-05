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
        state = TutorialState.Joystick;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerScript player = PlayerScript.instance;
        foreach (GameObject i in toActivate[(int)state].UIObjects)
        {
            i.SetActive(true);
        }
        //based on tutorial text, set dimension

        if (GameManager.instance.DeliveryTimeRatio() < 0.2f)
            GameManager.instance.ResetDeliveryTime();
        if ((int)state > (int)TutorialState.Zombie)
            GameManager.instance.pizzaAmount = Mathf.Clamp(GameManager.instance.pizzaAmount, 0, 99);
        textBackground.sizeDelta = new Vector2(textBackground.sizeDelta.x, (77 / 4.0f) * tutorialText.textInfo.lineCount + 15.4f);
    }
}
