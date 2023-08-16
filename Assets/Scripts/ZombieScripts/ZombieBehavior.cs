using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class ZombieBehavior : MonoBehaviour
{

    [SerializeField] bool debug1, debug2;
    [SerializeField] bool debug;
    [SerializeField] bool DONOTCHECKMARKTHISVARIABLE; // this is only for tutorial level, do not mind it

    public Color debugColor;
    public float sqrAggroRange;
    public float speed;
    public bool aggroed;

    [SerializeField] float castLength;

    private AudioSource zombieSource;
    [SerializeField] private float volume = 1;
    [SerializeField] private AudioClip[] dieSounds;
    [SerializeField] private AudioClip eatSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip[] growlSounds;

    private bool turnAggro = false;

    float idleTimer;
    float timeUntilIdleMove;

    private float growlInterval;
    
    [SerializeField] Rigidbody2D rb;
    public Vector2 moveDir;
    Vector2[] rayVectors;
    public static float sqrMinSpeedToCrush = 4f;

    Vector3 target;
    bool targettingPlayer;
    LinkedList<GameObject> pizzaTargetQueue; // acts more like a deque but who cares
    public bool eatingPizza;


    public ZombieAnimation animation;
    float despawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        idleTimer = 0;
        despawnTimer = 0;
        eatingPizza = false;
        animation = GetComponent<ZombieAnimation>();
        zombieSource = GetComponent<AudioSource>();

        pizzaTargetQueue = new LinkedList<GameObject>();
        targettingPlayer = true; // just target the player
        ResetTimeUntilMove();
        InitRayVector();
        //StartCoroutine(Growl());
    }

    // Update is called once per frame
    void Update()
    {
        float sqrDist = (PlayerScript.instance.transform.position - transform.position).sqrMagnitude;

        //Debug.Log($"state agro: {aggroed} eating: {eatingPizza}", this);

        if (!DONOTCHECKMARKTHISVARIABLE && sqrDist > ZombieSpawner.instance.sqrDespawnDist) {

            ZombiePool.instance.ReturnToPool(gameObject);
            
            return; //just in case
        }
        debugColor = GetComponent<SpriteRenderer>().color;
        //want the zombie to speed up once the player leaves the agro range
        if (!eatingPizza)
        {
            if (sqrDist < sqrAggroRange)
            {
                if (!aggroed)
                {
                    animation.lastFrameTime = Time.time;
                    animation.frame = 0;
                }

                aggroed = true;
            }
        }

        if (eatingPizza)
        {
            moveDir = Vector2.zero;
        }
        else if (aggroed)
        {
            FollowPlayer();
        }
        else
        {
            //idle
            //idleTimer += Time.deltaTime;
            //IdleMovement();
        }

        if (turnAggro == false && aggroed == true)
        {
            turnAggro = aggroed;
            zombieSource.PlayOneShot(growlSounds[Random.Range(0, growlSounds.Length)], volume * GlobalSoundManager.instance.sfxVolume * GlobalSoundManager.instance.globalVolume);
        }
        else if (aggroed == false)
        {
            turnAggro = aggroed;
        }
    }

   

    public void Die()
    {
        eatingPizza = false;
        aggroed = false;
        ZombiePool.instance.ReturnToPool(gameObject);
        GlobalSoundManager.instance.playSFX(dieSounds[Random.Range(0, dieSounds.Length)], 1.5f * GlobalSoundManager.instance.sfxVolume * GlobalSoundManager.instance.globalVolume);
    }

    public void SetTarget(GameObject pizzabox)
    {
        target = pizzabox.transform.position;
        targettingPlayer = false;
        pizzaTargetQueue.AddLast(pizzabox);
        //Debug.Log("pizza", this);
    }

    public void PizzaExpires()
    {
        pizzaTargetQueue.RemoveFirst();
        
        //Debug.Log($"After deletion {pizzaTargetQueue.Count}");
        targettingPlayer = true;
    }

    public Rigidbody2D GetRB()
    {
        return rb;
    }

    private void FixedUpdate()
    {
        //Debug.Log("set velo " + moveDir, this);
        if (PlayerScript.instance.isInvincible && false)
        {
            //rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity =GameManager.instance.zombieSpeedMultplier*moveDir * speed;
            Debug.DrawRay(transform.position, rb.velocity, Color.blue);
        }
    }

    void IdleMovement()
    {
        //interval between movement happens based on a random number
        if (idleTimer > timeUntilIdleMove)
        {
            //Debug.Log("new move");
            idleTimer = 0;
            ResetTimeUntilMove();
            StartCoroutine(MoveZombie());
        }
    }

    IEnumerator MoveZombie()
    {
        rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 2;
        yield return new WaitForSeconds(1);
        if (!aggroed)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void ResetTimeUntilMove()
    {
        timeUntilIdleMove = Random.Range(3f, 7f);
    }

    private void FollowPlayer()
    {
        PlayerScript player = PlayerScript.instance;
        //use the information based on the player thingy

        //Vector3 nextPosition = player.roadTiles.GetCellCenterWorld(player.GetPathParent(player.roadTiles.WorldToCell(transform.position)));

        //moveDir = (nextPosition - transform.position).normalized;
        bool clearSight = !Physics2D.Raycast(transform.position, PlayerScript.instance.transform.position - this.transform.position, castLength, LayerMask.GetMask("Curb"));

        // cannot move freely when hugging curb
        /*
        int roadCount = 0;
        Vector3Int currCellPos = player.GetPathParent(player.roadTiles.WorldToCell(transform.position));
        currCellPos += Vector3Int.up;
        if (player.roadTiles.GetTile(currCellPos) != null)
        {
            roadCount++;
        }
        currCellPos += Vector3Int.down * 2;
        if (player.roadTiles.GetTile(currCellPos) != null)
        {
            roadCount++;
        }
        currCellPos += Vector3Int.up;
        currCellPos += Vector3Int.right;
        if (player.roadTiles.GetTile(currCellPos) != null)
        {
            roadCount++;
        }
        currCellPos += Vector3Int.left * 2;
        if (player.roadTiles.GetTile(currCellPos) != null)
        {
            roadCount++;
        }

        bool adjacentToCurb = roadCount == 4;
        */

        if (pizzaTargetQueue.Count == 0) // no pizzas on ground, target player instead
        {
            target = PlayerScript.instance.transform.position;
        }
        else
        {
            //Debug.Log(pizzaTargetQueue.Count);
            target = pizzaTargetQueue.First.Value.transform.position;
        }
        
        if (!eatingPizza) 
        {
            moveDir = (target - this.transform.position).normalized;
        }
        else if (eatingPizza)
        {
            moveDir = Vector2.zero;
        }
        //Debug.DrawLine(transform.position, new Vector2(nextPosition.x, nextPosition.y), Color.red);
        Debug.DrawRay(transform.position, moveDir * 2f, Color.red);
        Debug.DrawLine(transform.position, target, Color.blue);


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Rigidbody2D obstacleRB = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (obstacleRB.velocity.sqrMagnitude > sqrMinSpeedToCrush)
            {
                Die();
            }
        }
        else if (collision.gameObject.CompareTag("ThrowablePizza"))
        {
            Rigidbody2D pizzaRB = collision.gameObject.GetComponent<Rigidbody2D>();
            // fast enough to kill zombie
            if (pizzaRB.velocity.sqrMagnitude > sqrMinSpeedToCrush)
            {
                PlayerScript.instance.AddBoost(0.1f);
                Die();
            }
            else // eat the pizza!
            {
                if (!eatingPizza)
                    StartCoroutine(EatPizza(collision.gameObject.GetComponent<ThrowablePizza>()));
            }
        }
    }

    public void StartEating()
    {
        StartCoroutine(EatPizza());
    }

    IEnumerator EatPizza(ThrowablePizza pizza)
    {
        animation.lastFrameTime = Time.time;
        animation.timeUntilNextEatFrame = animation.intervalBetweenEatingFrames;
        animation.eatingFrame = 0;
        eatingPizza = true;
        aggroed = false;
        pizza.ZombieEatPizza();
        zombieSource.PlayOneShot(eatSound, volume * 2 * GlobalSoundManager.instance.sfxVolume * GlobalSoundManager.instance.globalVolume);

        yield return new WaitForSeconds(3);
        eatingPizza = false;
        aggroed = false;

    }

    IEnumerator EatPizza()
    {
        animation.lastFrameTime = Time.time;
        animation.timeUntilNextEatFrame = animation.intervalBetweenEatingFrames;
        animation.eatingFrame = 0;
        eatingPizza = true;
        aggroed = false;
        zombieSource.PlayOneShot(eatSound, volume * 2 * GlobalSoundManager.instance.sfxVolume * GlobalSoundManager.instance.globalVolume);

        yield return new WaitForSeconds(3);
        eatingPizza = false;
        aggroed = false;
    }

    /*
    IEnumerator Growl()
    {
        growlInterval = Random.Range(10, 30);
        Debug.Log(growlInterval);
        yield return new WaitForSeconds(growlInterval);

        if (eatingPizza == false)
        {
            zombieSource.PlayOneShot(growlSounds[Random.Range(0, growlSounds.Length)], volume);
        }

        StartCoroutine(Growl());
    }
    */

    void InitRayVector()
    {
        rayVectors = new Vector2[4];
        Vector2 rayVector = Quaternion.AngleAxis(26.66f, Vector3.forward) * Vector2.right * castLength;
        rayVectors[0] = rayVector;
        rayVectors[2] = -rayVector;
        rayVector = Quaternion.AngleAxis(126.57f, Vector3.forward) * rayVector;
        rayVectors[1] = rayVector;
        rayVectors[3] = -rayVector;
    }
}
