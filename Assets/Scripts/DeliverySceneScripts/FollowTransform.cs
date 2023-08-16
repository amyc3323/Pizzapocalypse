using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform followTransform;
    public Vector2 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = followTransform.position+(Vector3)offset;
    }
}
