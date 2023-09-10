using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : MonoBehaviour
{
    public static ZombiePool instance;
    public List<GameObject> pool;
    public GameObject[] zombieVariations;
    public int poolCount;
    public float velocityCompMin, velocityCompMax; // component velocity min and max
    public float minSqrPartSpeed;

    private void Awake()
    {
        instance = this;

        pool = new List<GameObject>();
        GameObject temp;
        for (int i = 0; i < poolCount; i++)
        {
            // mod 2 to get half half split of zombie types
            temp = Instantiate(zombieVariations[i % zombieVariations.Length]);
            temp.SetActive(false);

            pool.Add(temp);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < poolCount; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject zombie)
    {
        //play explode thingy
        // up 1 unit for "centering"
        Vector2 spawnLocation = zombie.transform.position + Vector3.up;
        for (int i = 0; i < 5; i++)
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                float velocityX = Random.Range(0f, velocityCompMax);
                float velocityY = Random.Range(0f, velocityCompMax);
                if (Random.Range(0f, 1f) > 0.5f)
                    velocityX = -velocityX;
                if (Random.Range(0f, 1f) > 0.5f)
                    velocityY = -velocityY;

                GameObject part = ZombieExplosionPool.instance.GetPooledObject();
                if (part == null)
                    break;

                Vector2 newVelo = new Vector2(velocityX, velocityY);
                if (newVelo.sqrMagnitude < minSqrPartSpeed)
                {
                    newVelo *= Mathf.Sqrt(minSqrPartSpeed / newVelo.sqrMagnitude);
                   
                }
                //Debug.Log(newVelo.sqrMagnitude, part);
                part.transform.position = spawnLocation;

                part.GetComponent<ZombiePartScript>().Activate();
                part.GetComponent<Rigidbody2D>().velocity = new Vector2(velocityX, velocityY);
            }
        }

        ZombieBehavior zombScript = zombie.GetComponent<ZombieBehavior>();
        zombScript.aggroed = false;
        zombScript.eatingPizza = false;
        zombScript.moveDir = Vector2.zero;
        zombScript.GetRB().velocity = Vector2.zero;

        zombie.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
