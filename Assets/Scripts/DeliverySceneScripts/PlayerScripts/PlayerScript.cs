using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.Events;
[System.Serializable] 
public struct SurfaceInfo
{

    public float accelerationFactor;
    public float topSpeed;
}
public enum SurfaceType
{
    street,curb,grass
}
public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;
    public SurfaceType currentSurface;
    public UnityEvent changeSurfaceEvent;
    public UnityEvent throwPizzaEvent;
    [SerializeField] public Rigidbody2D rb2D { private set; get; }
    [SerializeField] GameObject playerArt;
    [SerializeField] Rigidbody2D testRotationObject;
    private Vector2 inputVector;
    private float rotationAngle;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float topSpeed = 40f;
    [SerializeField] private float enterCurbSpeedMultiplier = 0.8f;
    [SerializeField] private AnimationCurve velocityToExtraForce;
    [Header("Custom Ground Values")]
    public SurfaceInfo streetSurface;
    public SurfaceInfo curbSurface;
    public SurfaceInfo grassSurface;
    [Header("Health")]
    Vector3 origScale = new Vector3(0.75f, 0.75f, 1f);
    public int health;
    public bool isInvincible;
    public float invincibilityTime = 2f;
    public float timeBetweenFlashes = 0.5f;

    [SerializeField] int tileMax;
    public Tilemap roadTiles;
    public HashSet<Vector3Int> validRoad;
    // the path becomes a linked list
    private Dictionary<Vector3Int, Vector3Int> pathParent;
    public InputAction driving;

    public float boostMeter;
    public float boostMult = 2f;
    public bool boostPressed;
    public float boostDecrease = 0.05f;
    public float rightVelocityToBoostIncrease = 0.1f;
    public bool isDrifting = false;
    private float originalDrag;
    private float originalRotationalDrag;

    [SerializeField] float zombieSpawnChance;
    [SerializeField] GameObject zombiePrefab;

    [SerializeField] private TrailRenderer[] trail;
    [SerializeField] private Color boostingTrailColor;
    [SerializeField] private Color defaultTrailColor;
    [SerializeField] private Color driftingTrailColor;


    [Header("Sound")]
    [SerializeField] private AudioClip[] crashClips;
    [SerializeField] private AudioClip bumpSounds;
    [SerializeField] private AudioClip zombieCrushSounds;
    [SerializeField] private AudioClip bushSound;
    [SerializeField] private AnimationCurve velocityToBumpVolume;
    [SerializeField] private float velocityXScale;

    [Header("Buff Multipliers")]
    public float boostMultiplier=1f;
    public float speedMultiplier=1f;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (rb2D == null) rb2D = GetComponent<Rigidbody2D>();
        boostMeter = 0;
        boostPressed = false;
        health = 3;
        isInvincible = false;

        validRoad = new HashSet<Vector3Int>();
        pathParent = new Dictionary<Vector3Int, Vector3Int>();
        BoundsInt bounds = roadTiles.cellBounds;

        float offsety = Random.Range(0f, 50f);
        float offsetx = Random.Range(0f, 50f);
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = roadTiles.GetTile(pos);

                if (tile == null)
                    continue;
                validRoad.Add(pos);


                if ((roadTiles.GetCellCenterWorld(pos) - transform.position).sqrMagnitude < 4 * 4)
                    continue;

                Vector3 worldPos = roadTiles.GetCellCenterWorld(pos);
                if (Mathf.PerlinNoise((worldPos.x + offsetx) * 5, (worldPos.y + offsety) * 5) < zombieSpawnChance)
                {
                    //Instantiate(zombiePrefab);
                    //zombiePrefab.transform.position = roadTiles.GetCellCenterWorld(pos);
                }
                //Debug.Log("valid road " + pos);
                
            }
        }
        originalDrag = rb2D.drag;
        originalRotationalDrag = rb2D.angularDrag;
    }

    public void Boost()
    {
        boostPressed = !boostPressed;
    }

    // adjacent tiles have adjacent x y values
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            foreach (var vec in pathParent)
            {

                //Gizmos.DrawLine(roadTiles.GetCellCenterWorld(vec.Key), roadTiles.GetCellCenterWorld(vec.Value) - (roadTiles.GetCellCenterWorld(vec.Value) - roadTiles.GetCellCenterWorld(vec.Key)) * 0.2f);
                //Debug.Log(vec.Value + " " + vec.Key);
                ForGizmo(roadTiles.GetCellCenterWorld(vec.Key), (roadTiles.GetCellCenterWorld(vec.Value) - roadTiles.GetCellCenterWorld(vec.Key)), Color.white);
            }
        }
    }
    public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);

        //Debug.Log(direction);

        Vector3 right = ((direction != Vector3.zero) ? Quaternion.LookRotation(direction, Vector3.forward) : Quaternion.identity) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = ((direction != Vector3.zero) ? Quaternion.LookRotation(direction, Vector3.forward) : Quaternion.identity) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(TrailRenderer t in trail) { t.startColor = isDrifting ? driftingTrailColor : (!boostPressed ? defaultTrailColor : boostingTrailColor); }
        rb2D.angularDrag = isDrifting ? 0 : originalRotationalDrag;
        GetInputSystem();
        UpdateGround(GetCurrentGround());
        //BFSForEnemy();
    }
    private SurfaceInfo GetCurrentGround()
    {
        SurfaceType lastType = currentSurface;

        if (GridHelper.instance.streetTilemap.GetTile(GridHelper.instance.streetTilemap.WorldToCell(transform.position)) != null)
        {
            
            currentSurface = SurfaceType.street;
            if (lastType != SurfaceType.street) changeSurfaceEvent.Invoke();
            return streetSurface;
        }
        else if(GridHelper.instance.curbTilemap.GetTile(GridHelper.instance.curbTilemap.WorldToCell(transform.position)) != null)
        {
            currentSurface = SurfaceType.curb;
            if (lastType != SurfaceType.curb)
            {
                changeSurfaceEvent.Invoke(); rb2D.velocity *= enterCurbSpeedMultiplier;
            }
                return curbSurface;
        }

        currentSurface = SurfaceType.grass;
        if (lastType != SurfaceType.grass) changeSurfaceEvent.Invoke();
        return grassSurface;
    }
    private void UpdateGround(SurfaceInfo sf)
    {
        accelerationFactor = sf.accelerationFactor;
        topSpeed = sf.topSpeed;
    }
    private void GetInputSystem()
    {
        //inputVector = Vector2.up * Input.GetAxis("Vertical") + Vector2.right * Input.GetAxis("Horizontal");
        //if (driving != null) { inputVector = driving.ReadValue<Vector2>(); Debug.Log($"Input vector {inputVector}"); }
        // This will be left shift for now
        inputVector = transform.InverseTransformDirection(JoystickScript.leftJoystick);
        //Debug.Log($":Value: {inputVector}");
    }
    private void FixedUpdate()
    {

        AddEngineForce();
        ApplySteering();
        KillOrthogonalVelocity();
        //Vector2 vel = rb2D.velocity;
        //if
    }
    private void AddEngineForce()
    {
        if (inputVector.magnitude < 0.1f) { rb2D.velocity *= 0.9f; }
        float mult;
        if (boostPressed && boostMeter != 0) {
            mult = boostMult;
            boostMeter -= boostDecrease * 0.02f*1.5f; // 0.2f is basically deltaTime for Fixed update
            boostMeter = Mathf.Clamp(boostMeter, 0, 1);
        }
        else
            mult = 1;
        if (true|| !isDrifting)rb2D.AddForce(speedMultiplier*inputVector.y*(1+velocityToExtraForce.Evaluate(rb2D.velocity.magnitude))*((inputVector.y<0)?0f:1)*transform.up*accelerationFactor * mult * (isDrifting ? 0.1f : 1f) * (1 / rb2D.mass));
        
    }
    private void KillOrthogonalVelocity()
    {
        Vector2 forward = transform.up * Mathf.Clamp(Vector2.Dot(rb2D.velocity, transform.up),-topSpeed,topSpeed);
        
        Vector2 right = transform.right * Vector2.Dot(rb2D.velocity, transform.right);

        if (!isDrifting) rb2D.velocity = forward + right * 0.9f;
        else { rb2D.velocity = forward + right * driftFactor; boostMeter += boostMultiplier * right.magnitude * rightVelocityToBoostIncrease; }

        }
    private void ApplySteering()
    {

        float backwardsModifier = (inputVector.x<0?-1:1)*((inputVector.y < 0 )? inputVector.y : 0);
        testRotationObject.transform.up = JoystickScript.leftJoystick;
        //Debug.Log("Dot: " + Vector2.Dot(rb2D.velocity, transform.up));
        bool rotationIsSmaller;
        float rotation = testRotationObject.rotation % 360;

        if (rotation < 0)
        {
            rotation += 360;
        }
        rotationAngle =rotationAngle % 360;

        if (rotationAngle < 0)
        {
            rotationAngle += 360;
        }
        if (rotationAngle <= rotation) { rotationIsSmaller = true; }
        else rotationIsSmaller = false;
        rotationAngle -= Mathf.Clamp(inputVector.x-backwardsModifier,-1f,1f) * turnFactor* (Vector2.Dot(rb2D.velocity, transform.up)*0.9f+2f)*(isDrifting?0.1f:1f);

        //Debug.Log($"X axis: {inputVector.x}");

        if (rotationIsSmaller && rotationAngle > rotation) { rotationAngle = rotation; }

        else if (!rotationIsSmaller && rotationAngle < rotation) { rotationAngle = rotation; }
        rb2D.MoveRotation(rotationAngle);
    }

    private void BFSForEnemy()
    {
        // classic BFS setup
        int[,] dir = { {0, 1}, {0, -1}, { 1, 0 }, { -1, 0 } };

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Queue<Vector3Int> cellPos = new Queue<Vector3Int>();
        pathParent.Clear();

        pathParent.Add(roadTiles.WorldToCell(transform.position), roadTiles.WorldToCell(transform.position));
        cellPos.Enqueue(roadTiles.WorldToCell(transform.position));
        visited.Add(roadTiles.WorldToCell(transform.position));

        while (cellPos.Count > 0)
        {
            int size = cellPos.Count;
            for (int j = 0; j < size; j++) {
                Vector3Int curr = cellPos.Dequeue();
                for (int i = 0; i < 4; i++)
                {
                    int nx = curr.x + dir[i, 0];
                    int ny = curr.y + dir[i, 1];
                    Vector3Int nextPos = new Vector3Int(nx, ny, curr.z);

                    if (visited.Contains(nextPos))
                        continue;

                    //Debug.Log($"{validRoad.Count} {nextPos}");
                    if (validRoad.Contains(nextPos))
                    {
                        //add this to list

                        visited.Add(nextPos);
                        pathParent.Add(nextPos, curr);
                        cellPos.Enqueue(nextPos);
                    }
                }
            }
        }

        
        // yay! BFS done
    }

    public void AddBoost(float addition)
    {
        boostMeter += addition* boostMultiplier;
        Mathf.Clamp(boostMeter, 0f, 1f);
    }

    public Vector3Int GetPathParent(Vector3Int child)
    {
        return pathParent[child];
    }

    IEnumerator IFrames(ZombieBehavior zombie) {
        isInvincible = true;
        //health--;
        if (GameManager.instance.pizzaAmount>0) zombie.StartEating();

        GameManager.instance.RemovePizzas(1);
        GameCanvasManager.instance.UpdateHealth(health);
        if (health <= 0)
        {
            GameManager.instance.GameOver();
        }
        zombie.aggroed = false;
        zombie.GetRB().velocity = Vector2.zero;

        // turn off collisions
        gameObject.layer = LayerMask.NameToLayer("InvinciblePlayer");

        for (float t = 0; t < invincibilityTime; t += timeBetweenFlashes) {
            
            if (playerArt.transform.localScale == Vector3.zero) {
                playerArt.transform.localScale = origScale;
            }
            else {
                playerArt.transform.localScale = Vector3.zero;
            }

            yield return new WaitForSeconds(timeBetweenFlashes);
        }
        playerArt.transform.localScale = origScale;

        
        gameObject.layer = LayerMask.NameToLayer("Player");
        isInvincible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GlobalSoundManager.instance.playSFX(bumpSounds, velocityToBumpVolume.Evaluate(rb2D.velocity.magnitude / velocityXScale));
        //Debug.Log("hey " + collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "Obstacle":
                //AddBoost(0.1f);
                GlobalSoundManager.instance.playSFX(crashClips[Random.Range(0, crashClips.Length)]);
                break;
            case "Bush":
                GlobalSoundManager.instance.playSFX(bushSound);
                break;
            case "Zombie":
                
                // is fast enough                
                if (rb2D.velocity.sqrMagnitude > ZombieBehavior.sqrMinSpeedToCrush) {
                    AddBoost(0.1f);
                    GlobalSoundManager.instance.playSFX(zombieCrushSounds);
                    collision.gameObject.GetComponent<ZombieBehavior>().Die();
                }
                else { // no boost
                    if (!isInvincible)
                        StartCoroutine(IFrames(collision.gameObject.GetComponent<ZombieBehavior>()));                    
                }
                break;
        }

    }
}
