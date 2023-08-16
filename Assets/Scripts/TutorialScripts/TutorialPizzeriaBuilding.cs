using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPizzeriaBuilding : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.pizzaAmount = Mathf.Max(99, GameManager.instance.pizzaAmount);
            GameCanvasManager.instance.UpdatePizzaAmount(GameManager.instance.pizzaAmount);
        }
    }
}
