using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningIcon : MonoBehaviour
{
    public Transform followTransform;
    [SerializeField] private float sideOffset = 50f;
    private void Update()
    {
        Debug.Log($"Camera dimensions ({Camera.main.pixelWidth / 2},{Camera.main.pixelHeight})");

        Vector2 targetLocation = Camera.main.WorldToScreenPoint(followTransform.transform.position);
        float posScreenSlope = Camera.main.pixelWidth/ Camera.main.pixelHeight;
        if (Mathf.Abs(targetLocation.x) >= Camera.main.pixelWidth / 2 || Mathf.Abs(targetLocation.y) >= Camera.main.pixelHeight / 2)
        {
            targetLocation.Normalize();
            if (targetLocation.y == 0)
            {
                transform.localPosition = targetLocation*= Mathf.Abs((Camera.main.pixelWidth / 2 - sideOffset) / (targetLocation.x));
                return;
            }
            float locSlope = targetLocation.x / targetLocation.y;
            if (Mathf.Abs(locSlope) >= 1) targetLocation *= Mathf.Abs((Camera.main.pixelHeight/2-sideOffset) / targetLocation.y);
            else targetLocation *= Mathf.Abs((Camera.main.pixelWidth / 2 - sideOffset) / (targetLocation.x));
            transform.localPosition = targetLocation;

        }
        else
        {
            WarningIconPooler.instance.ReturnWarningIcon(this);
        }
    }
}
