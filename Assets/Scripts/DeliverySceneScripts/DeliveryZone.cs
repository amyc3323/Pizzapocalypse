using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    public int id;
    static public int nextId = 0;
    public SpriteRenderer zoneDisplay;
    public Collider2D col;
    private void Awake()
    {
        gameObject.layer = 12;
        nextId = 0;
        col = GetComponent<Collider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        zoneDisplay = GetComponent<SpriteRenderer>();
        zoneDisplay.enabled = false;
        col.enabled = false;
        id = nextId;
        GameManager.instance.possibleDeliveryLocations.Add(this);
        nextId++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // send a signal to game manager if player and correct location
        //Debug.Log(GameManager.instance.currDeliveryZone + " " + id);
        if ((collision.CompareTag("Player")&& id == GameManager.instance.currDeliveryZone))
        {
            bool success = DeliveryTarget.instance.Deliver();
            if (success)
            {
                PlayerScript.instance.AddBoost(0.2f);
                zoneDisplay.enabled = false;
                col.enabled = false;
                Debug.Log("dlivery success");
                if (collision.CompareTag("ThrowablePizza"))
                {
                    ThrowablePizza thrownPizza = collision.GetComponent<ThrowablePizza>();
                    thrownPizza.PizzaDisappears();
                    GameManager.instance.pizzaAmount++;
                    GameCanvasManager.instance.UpdatePizzaAmount(GameManager.instance.pizzaAmount);
                }
            }
        }
        else if (collision.CompareTag("ThrowablePizza") && id == GameManager.instance.currDeliveryZone)
        {
            DeliveryTarget.instance.ThrowDeliver();
            PlayerScript.instance.AddBoost(0.2f);
            zoneDisplay.enabled = false;
            col.enabled = false;
            Debug.Log("dlivery success");
            collision.GetComponent<ThrowablePizza>().PizzaDisappears();
        }
    }
    public void DisableZone()
    {

        zoneDisplay.enabled = false;
        col.enabled = false;

    }

    [ContextMenu("Realign line renderer")]
    void AlignLineRenderer()
    {
        PolygonCollider2D trigger = GetComponent<PolygonCollider2D>();
        LineRenderer renderer = GetComponent<LineRenderer>();

        Vector3[] rendPoints = new Vector3[trigger.GetTotalPointCount()];
        int i = 0;
        foreach (Vector2 vertex in trigger.GetPath(0))
        {
            rendPoints[i] = new Vector3(vertex.x, vertex.y, 0);
            i++;
        }
        renderer.SetPositions(rendPoints);
    }

}
