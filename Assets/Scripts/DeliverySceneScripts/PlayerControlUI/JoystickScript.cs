using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickScript : MonoBehaviour
{
    public static Vector2 leftJoystick = Vector2.zero;
    public float range;
    public Vector2 origPosition;
    [SerializeField] private Vector2 trueOrigPosition;
    [SerializeField] private int sqrAdjustmentDistance;
    public bool isHeld;
    private int currentTouchIndex;
    [SerializeField] private Transform joystickBackground;
    // Start is called before the first frame update
    void Start()
    {
        trueOrigPosition = transform.position;
        origPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHeld) {
            int newTouchIndex = TouchTracker.getNewTouch();


            if (newTouchIndex != -1&&TouchTracker.hasTTouch(newTouchIndex))
            {
                Touch currentTouch = TouchTracker.getTTouch(newTouchIndex);

                if ((currentTouch.position - trueOrigPosition).sqrMagnitude < sqrAdjustmentDistance)
                {
                    isHeld = true;
                    origPosition = currentTouch.position;
                    joystickBackground.position = currentTouch.position;
                    currentTouchIndex = newTouchIndex;
                }
            }
        }
        if (isHeld)
        {
            if (!TouchTracker.hasTTouch(currentTouchIndex))
            {
                isHeld = false;
                leftJoystick = Vector2.zero;
                transform.position = origPosition;
            }
            else
            {
                Vector2 touchPosition= TouchTracker.getTTouch(currentTouchIndex).position;
                transform.position = touchPosition;
                if (((Vector2)origPosition - touchPosition).sqrMagnitude > range * range) { transform.position = (touchPosition- (Vector2)origPosition).normalized * range+origPosition; }
                leftJoystick = ((Vector2)transform.position - origPosition) / range; 
                if (leftJoystick.sqrMagnitude > 1f) leftJoystick.Normalize();
                
            }
        }
        
    }
}
