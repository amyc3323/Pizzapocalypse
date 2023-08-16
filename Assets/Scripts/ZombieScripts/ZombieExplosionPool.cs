using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieExplosionPool : MonoBehaviour
{
    public static ZombieExplosionPool instance;
    public List<GameObject> pool;
    public GameObject partObject;
    public int poolCount;
    public Sprite[] zombiePartSprites;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        pool = new List<GameObject>();
        GameObject temp;
        for (int i = 0; i < poolCount; i++)
        {
            temp = Instantiate(partObject);
            temp.SetActive(false);

            pool.Add(temp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < poolCount; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].GetComponent<SpriteRenderer>().sprite = zombiePartSprites[Random.Range(0, zombiePartSprites.Length)];
                return pool[i];
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
