using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/BoostBenefit")]

public class BoostBenefit : IngredientBenefitBase
{
    public float boostMulti = 1.25f;
    public override void Benefit()
    {
        PlayerScript.instance.boostMultiplier *= boostMulti;
    }
}
