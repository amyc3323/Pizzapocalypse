using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/TimeBenefit")]

public class TimeBenefit : IngredientBenefitBase
{
    public float timeMulti = 1.25f;
    public override void Benefit()
    {
        GameManager.instance.timeMultiplier *= timeMulti;
    }
}
