using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/BoostPowerBenefit")]

public class BoostPowerBenefit : IngredientBenefitBase
{
    public float boostPowerMulti = 1.25f;
    public override void Benefit()
    {
        PlayerScript.instance.boostPowerMultiplier *= boostPowerMulti;
    }
}
