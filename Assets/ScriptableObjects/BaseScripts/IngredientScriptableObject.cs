
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable, CreateAssetMenu(menuName = "ScriptableObjects/Ingredient")]
public class IngredientScriptableObject : ScriptableObject
{
    public string ingredientName;
    public bool isMeat;
    public Sprite icon;
    public Sprite[] draggedIcon = new Sprite[3];
    public int minPrice;
    public int maxPrice;
    public IngredientBenefitBase[] benefits;
    [Tooltip("The order it shows up in the inventory"), SerializeField] public int trueOrder;

    public string description;
    public void Print()
    {
        Debug.Log($"Name: {ingredientName}");
    }
    public void ApplyBenefits()
    {
        foreach (IngredientBenefitBase ben in benefits)
        {
            ben.Benefit();
        }
    }
}
