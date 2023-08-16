using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(inactiveAfterSeconds());
    }

    IEnumerator inactiveAfterSeconds()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetComponent<Rigidbody2D>().velocity, this);
    }
}
