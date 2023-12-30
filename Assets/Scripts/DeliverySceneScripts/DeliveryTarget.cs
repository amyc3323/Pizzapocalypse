using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTarget : MonoBehaviour
{
    public static DeliveryTarget instance;
    public Pizza pizzaType;
    public ParticleSystem confetti;
    //Tutorial manager may or may not exist
    TutorialManager tutorialManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tutorialManager = TutorialManager.instance; // could be null
    }

    public void ResetDelivery(Pizza pizza)
    {
        pizzaType = pizza;
    }

    public bool Deliver()
    {
        Vector3 recordPos = transform.position;
        bool success = GameManager.instance.FinishDelivery(pizzaType);
        if (success)
        {
            StartCoroutine(PlayConfetti(recordPos));
        }
        return success;
        //Instantiate(deliveryVFX, PlayerScript.GetInstance().transform.position, Quaternion.identity);
    }
    public void ThrowDeliver()
    {
        Vector3 recordPos = transform.position;
        GameManager.instance.FinishThrowingDelivery(pizzaType);
        //checks if in tutorial
        if (tutorialManager != null)
        {
            tutorialManager.throwDeliveries++;
        }
        
        StartCoroutine(PlayConfetti(recordPos));
    }

    IEnumerator PlayConfetti(Vector3 position)
    {
        confetti.gameObject.SetActive(true);
        confetti.Play();
        confetti.transform.position = position + Vector3.up * 2;
        yield return new WaitForSeconds(1f);
        confetti.gameObject.SetActive(false);
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameManager.instance.FinishDelivery(pizzaType);
            Instantiate(deliveryVFX, PlayerScript.GetInstance().transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
    */
}
