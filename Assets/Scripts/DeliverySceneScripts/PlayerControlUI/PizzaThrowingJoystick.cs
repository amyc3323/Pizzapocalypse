using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PizzaThrowingJoystick : MonoBehaviour
{
    public static Vector2 leftJoystick;
    [Header("Pizza Throwing")]
    public ThrowablePizza throwablePizzaPrefab;
    [Header("Joystick Variables")]
    public float range;
    [SerializeField] private Vector2 origPosition;
    [SerializeField] private int sqrAdjustmentDistance;
    public bool isHeld;
    private int currentTouchIndex;
    [SerializeField] private Transform joystickBackground;
    [SerializeField] private AudioClip throwSound;
    // Start is called before the first frame update
    void Start()
    {
        origPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHeld)
        {
            int newTouchIndex = TouchTracker.getNewTouch();


            if (newTouchIndex != -1 && TouchTracker.hasTTouch(newTouchIndex))
            {
                Touch currentTouch = TouchTracker.getTTouch(newTouchIndex);

                if ((currentTouch.position - origPosition).sqrMagnitude < range * range)
                {
                    isHeld = true;
                    currentTouchIndex = newTouchIndex;
                }
            }
        }
        if (isHeld)
        {
            if (!TouchTracker.hasTTouch(currentTouchIndex))
            {
                ReleasePizza();
                isHeld = false;
                transform.position = origPosition;
            }
            else
            {
                Vector2 touchPosition = TouchTracker.getTTouch(currentTouchIndex).position;
                transform.position = touchPosition;
                if (((Vector2)origPosition - touchPosition).sqrMagnitude > range * range) { transform.position = (touchPosition - (Vector2)origPosition).normalized * range + origPosition; }

            }
        }
        leftJoystick = (Vector2)transform.position - origPosition;
        Debug.Log($"LJ: {leftJoystick}");

    }
    private void ReleasePizza()
    {
        if (((Vector2)transform.position-origPosition).sqrMagnitude> 500f && GameManager.instance.RemovePizzas(1))
        {
            StartCoroutine(throwPizza(transform.position));
            PlayerScript.instance.throwPizzaEvent.Invoke();
            PlayerAnimation.instance.StartThrowing(((Vector2)transform.position - origPosition).normalized);
            GlobalSoundManager.instance.playSFX(throwSound, 2);
        }
    }
    IEnumerator throwPizza(Vector2 joyPos)
    {
        yield return new WaitForSeconds(0.2f);
        ThrowablePizza instance = Instantiate(throwablePizzaPrefab);
        instance.ResetValues(((Vector2)joyPos - origPosition), ((Vector2)joyPos - origPosition).magnitude);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
