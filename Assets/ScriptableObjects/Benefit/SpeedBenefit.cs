using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/SpeedBenefitBase")]

public class SpeedBenefit : IngredientBenefitBase
{
    public float speedMulti = 1.25f;
    public override void Benefit()
    {
        PlayerScript.instance.speedMultiplier*=speedMulti;
    }
}
