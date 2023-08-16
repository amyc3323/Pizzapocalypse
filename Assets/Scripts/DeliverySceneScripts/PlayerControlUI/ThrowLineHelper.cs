using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLineHelper : MonoBehaviour
{
    public LineRenderer lineRenderer;
    [SerializeField] private float lengthMultiplier;
    public void Update()
    {
        if(PizzaThrowingJoystick.leftJoystick.sqrMagnitude>500f)lineRenderer.SetPosition(1, PizzaThrowingJoystick.leftJoystick * lengthMultiplier*GameManager.instance.pizzaThrowingMultiplier);
        else { lineRenderer.SetPosition(1, Vector2.zero); }
    }
}
