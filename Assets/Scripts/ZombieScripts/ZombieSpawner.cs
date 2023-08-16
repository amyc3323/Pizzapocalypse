using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZombieSpawner : MonoBehaviour
{
    public static ZombieSpawner instance;
    [SerializeField] Tilemap tilemap;
    [SerializeField] float zombieSpawnChance;
    [SerializeField] float perlinScale;
    public List<Vector3Int> validTileList;
    public float minSpawnDist;
    float time;
    public float maxSpawnDist;
    public float sqrDespawnDist;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);

                if (tile == null)
                    continue;

                validTileList.Add(pos);
                if ((tilemap.GetCellCenterWorld(pos) - PlayerScript.instance.transform.position).sqrMagnitude < minSpawnDist * minSpawnDist)
                    continue;


                if (Random.Range(0f,1f) < zombieSpawnChance)
                {
                    GameObject temp = ZombiePool.instance.GetPooledObject();
                    if (temp != null)
                    {
                        temp.SetActive(true);
                        temp.transform.position = tilemap.GetCellCenterWorld(pos);
                    }
                }

            }
        }


        time = 0;
    }

    public void SpawnZombie()
    {
        GameObject toInstantiate = ZombiePool.instance.GetPooledObject();
        //can't spawn a zombie
        if (toInstantiate == null)
            return;
        //might be expensive
        int limit = 10;

        //Debug.Log("spawnFunction");
        for (int i = 0; i < limit; i++) {
            //choose a random direction + magnitude

            Vector3 playerPos = PlayerScript.instance.transform.position;
            // should be normalized
            float magnitude = Random.Range(minSpawnDist, maxSpawnDist);

            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
            
            Vector3 tryPos = new Vector3(direction.x, direction.y) + playerPos;
            
            Vector3Int cellPos = tilemap.WorldToCell(tryPos);
            
            if (tilemap.GetTile(cellPos) == null)
            {
                continue;
            }

            //found valid tile position
            toInstantiate.SetActive(true);
            //Debug.DrawRay(playerPos, direction, Color.yellow, 2f);
            toInstantiate.transform.position = tryPos;
            //Debug.Log($"foundvalid {(playerPos - tryPos).magnitude} magnitude {magnitude}", toInstantiate);
            //Debug.Log((playerPos - tryPos) + " " + direction + " " + direction.magnitude);
            break;
        }

        //Debug.Log("spawn new zombie", toInstantiate);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.3f)
        {
            SpawnZombie();
            time = 0;
        }
        //CalcNoise();
    }
}
