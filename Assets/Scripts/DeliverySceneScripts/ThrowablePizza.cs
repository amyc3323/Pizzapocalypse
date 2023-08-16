using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class ThrowablePizza : MonoBehaviour
{

    private Rigidbody2D rb2D;
    [SerializeField] private float physicalRadius = 0.5f;
    [Header("Attracting Variables")]
    [SerializeField] private float pizzaAttractingRange;

    [SerializeField] private LayerMask zombieLayermask;
    private Queue<ZombieBehavior> affectedZombies;
    [SerializeField] private float maxAttractionDuration;
    [SerializeField] private float postEatenAttractionDuration;
    private float timeAffected = float.PositiveInfinity;
    private float timeEaten = float.PositiveInfinity;

    [Header("Throwing Variables")]
    [SerializeField] private float magnitudeMultiplier;
    [Header("Debug")]
    [SerializeField] private float minSqrMagnitude = 0.1f;
    [SerializeField] private float speed;
    [SerializeField] private bool isThrowing = true;

    [SerializeField] private AudioClip collideSound;

    public void ResetValues(Vector2 direction, float magnitude)
    {
        speed = magnitude;
        isThrowing = true;
        transform.position = PlayerScript.instance.transform.position;
        rb2D.AddForce(direction * magnitude * magnitudeMultiplier*GameManager.instance.pizzaThrowingMultiplier, ForceMode2D.Impulse);
    }
    private void Awake()
    {
        if (rb2D == null) rb2D = GetComponent<Rigidbody2D>();
        affectedZombies = new Queue<ZombieBehavior>();
        Debug.Log("Hello world", this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<DeliveryTarget>(out DeliveryTarget del))
        {

            //SUCCESSFULLY DELIVERED
            rb2D.drag = 3;
            foreach (ZombieBehavior zb in affectedZombies)
            {
                zb.PizzaExpires();
            }

            Debug.Log("Destrouy after thingy");
            Destroy(gameObject,0.1f);
            this.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit");
        GlobalSoundManager.instance.playSFX(collideSound, 1);
    }

    private void Update()
    {
        if (!isThrowing&&GameManager.instance.pizzaAmount<GameManager.instance.maxPizza)
        {
            if ((transform.position-PlayerScript.instance.transform.position).sqrMagnitude<physicalRadius*physicalRadius)
            {
                //OMG REMOVING -1 PIZZAS ACTUALLY ADDS ONE :OOO -Henry
                // this is dumb - ethan
                Debug.Log("regain pizza");
                GameManager.instance.RemovePizzas(-1);
                foreach (ZombieBehavior zb in affectedZombies)
                {
                    zb.PizzaExpires();
                }
                Destroy(gameObject);

                return; // just in case it wants to run more
            }
        }
        
        if (!isThrowing && (Time.time - timeAffected > maxAttractionDuration||Time.time-timeEaten>postEatenAttractionDuration))
        {
            foreach(ZombieBehavior zb in affectedZombies)
            {
                zb.PizzaExpires();
            }
            Destroy(gameObject);
        }
        if (isThrowing && rb2D.velocity.sqrMagnitude < minSqrMagnitude)
        {
            isThrowing = false;
            AttractZombies();
        }

    }

    public void PizzaDisappears()
    {
        foreach (ZombieBehavior zb in affectedZombies)
        {
            zb.PizzaExpires();
        }

        Debug.Log("Destrouy after thingy");
        Destroy(gameObject, 0.1f);
    }

    public void ZombieEatPizza()
    {
        foreach (ZombieBehavior zb in affectedZombies)
        {
            Debug.Log(zb, zb);
            zb.PizzaExpires();
        }
        
        Destroy(gameObject);
    }

    public void AttractZombies()
    {
        Collider2D[] hitZombies = Physics2D.OverlapCircleAll(transform.position, pizzaAttractingRange, zombieLayermask);
        foreach(Collider2D zombie in hitZombies)
        {
            if(zombie.TryGetComponent<ZombieBehavior>(out ZombieBehavior zb))
            {
                zb.SetTarget(gameObject);
                affectedZombies.Enqueue(zb);
            }
        }
        timeAffected = Time.time;

    }
}
