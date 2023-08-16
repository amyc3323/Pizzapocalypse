using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/ZombieSpeedBenefit")]

public class ZombieSpeedBenefit : IngredientBenefitBase
{
    public float speedMulti = 0.8f;
    public override void Benefit()
    {
        GameManager.instance.zombieSpeedMultplier *= speedMulti;
    }
}
