using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTransform : MonoBehaviour
{
    public Transform targetTransform;
    

    // Update is called once per frame
    void Update()
    {
        transform.right = Vector3.Lerp(transform.right,(targetTransform.position-transform.position).normalized,Time.deltaTime*5);
        transform.localPosition = transform.right * 1;
        transform.localScale = Vector3.one;
    }
}
