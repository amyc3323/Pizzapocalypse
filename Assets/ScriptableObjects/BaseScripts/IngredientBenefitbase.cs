using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/IngredientBenefitbase")]
public class IngredientBenefitBase:ScriptableObject
{
    public virtual void Benefit() { Debug.LogError("NO BENEFIT CODED"); }
}
