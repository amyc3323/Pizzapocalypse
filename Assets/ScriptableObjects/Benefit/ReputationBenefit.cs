using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/ReputationBenefit")]

public class ReputationBenefit : IngredientBenefitBase
{
    public float reputationMulti = 1.25f;
    public override void Benefit()
    {
        GameManager.instance.reputationMultiplier *= reputationMulti;
    }
}
