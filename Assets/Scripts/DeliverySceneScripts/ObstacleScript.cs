using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float flyAngularSpeed;
    public float minSquareFlySpeed;
    [SerializeField] float sqrNonFlySpeed;
    public bool explodable;
    public float flySpeedMin;
    bool flying;
    bool disabled;

    Vector3 origPos;
    public float sqrRespawnDist;

    public float addedBoostPerMeter;
    float boostAdded;
    public float boostAddMin, boostAddMax;

    private void Start()
    {
        disabled = false;
        flying = false;
        rb = GetComponent<Rigidbody2D>();
        boostAdded = 0;
        origPos = transform.position;
    }

    public void Explode()
    {
        //Debug.Log("EPLXOSDE");
        
        GameObject clone = Instantiate(explosion);
        clone.transform.position = this.transform.position;
        //Destroy(this.gameObject);
        DisableObstacle();
    }

    public void DisableObstacle()
    {
        disabled = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void RenableObstacle()
    {
        disabled = false;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        transform.position = origPos;
        transform.rotation = (Quaternion.Euler(Vector3.zero));
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        
    }

    private void Update()
    {
        // re-enable if player is far away enough
        if (disabled && (PlayerScript.instance.transform.position - origPos).sqrMagnitude > sqrRespawnDist)
        {
            RenableObstacle();
        }
    }

    private void FixedUpdate()
    {
        if (flying)
        {
            flying = rb.velocity.sqrMagnitude > sqrNonFlySpeed;
            
            if (!flying)
            {
                //PlayerScript.instance.AddBoost((oldPosition - transform.position).magnitude * addedBoostPerMeter);
                StartCoroutine(DelayedExplode());
            }
            else
            {
                if (boostAdded < boostAddMax)
                {
                    float boostToAdd = rb.velocity.magnitude * 0.2f * addedBoostPerMeter;
                    
                    boostToAdd = Mathf.Clamp(boostToAdd, 0, boostAddMax - boostAdded);
                    Debug.Log(boostToAdd, this);
                    boostAdded += boostToAdd;
                    PlayerScript.instance.AddBoost(boostToAdd);
                }
            }
        }

        if (!flying)
        {
            //Debug.Log("not flyoing");
            rb.freezeRotation = true;
        }
        else
        {
            rb.freezeRotation = false;
        }
    }

    IEnumerator DelayedExplode()
    {
        yield return new WaitForSeconds(0.2f);
        Explode();
    }

    public void OnCollisionEnter2D(Collision2D collider) {

        if (collider.gameObject.CompareTag("Player"))
        {
            
            PlayerScript player = PlayerScript.instance;

            if (explodable)
            {
                player.AddBoost(0.05f);
                Explode();
            }
            else if (player.rb2D.velocity.sqrMagnitude > minSquareFlySpeed)
            {
                //Debug.Log("Fly");
                //rb.AddForce((this.transform.position - player.transform.position), ForceMode2D.Impulse);

                flying = true;
                float speed = flySpeedMin + (player.rb2D.velocity.sqrMagnitude - minSquareFlySpeed);
                rb.velocity = new Vector2((transform.position - player.transform.position).normalized.x * speed, (transform.position - player.transform.position).normalized.y * speed);
                rb.angularVelocity = flyAngularSpeed;
            }
        }
        
    }
}
