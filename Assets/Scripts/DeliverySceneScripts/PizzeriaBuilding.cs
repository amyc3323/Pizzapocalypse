using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//should use a circle trigger
public class PizzeriaBuilding : MonoBehaviour
{
    public static PizzeriaBuilding instance { private set; get; }
    private void Awake()
    {
        instance = this;
    }
    void OnTriggerStay2D(Collider2D collision) {
        
        if (collision.gameObject.CompareTag("Player")) {
            GameManager.instance.RefillPizzas();
        }
    }

}
