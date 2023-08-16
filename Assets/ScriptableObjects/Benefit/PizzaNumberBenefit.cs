using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/PizzaNumberBenefit")]

public class PizzaNumberBenefit : IngredientBenefitBase
{
    public int increaseAmount = 1;
    public override void Benefit()
    {
        GameManager.instance.maxPizza += increaseAmount;
    }
}
