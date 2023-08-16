using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialState triggerState; // this tells the trigger to only function if the player is at this state
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TutorialManager.instance.state == triggerState)
        {
            // go to next state
            if (triggerState == TutorialState.Drift)
            {
                GameManager.instance.pizzaAmount = 0;
                GameCanvasManager.instance.UpdatePizzaAmount(GameManager.instance.pizzaAmount);
            }
            else if (triggerState == TutorialState.Zombie)
            {
                GameManager.instance.ResetDeliveryTime();
            }
            TutorialManager.instance.state++;
            Debug.Log("other transition", this);
        }
    }
}
