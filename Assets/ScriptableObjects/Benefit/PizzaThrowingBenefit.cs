using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/PizzaThrowingBenefit")]

public class PizzaThrowingBenefit : IngredientBenefitBase
{
    public float throwingMulti = 2f;
    public override void Benefit()
    {
        GameManager.instance.pizzaThrowingMultiplier *= throwingMulti;
    }
}
