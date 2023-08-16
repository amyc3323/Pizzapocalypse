
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable, CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
public class RecipeScriptableObject : ScriptableObject
{
    public Sprite icon;
    public Ingredient[] ingredients;
    [Tooltip("The order it shows up in the inventory"), SerializeField] public int trueOrder;
}
