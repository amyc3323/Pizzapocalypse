using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HoverboxHelper : MonoBehaviour
{
    [SerializeField] private Image ingredientSprite;
    [SerializeField] private TMP_Text nameSpace;
    [SerializeField] private TMP_Text descriptionSpace;
    [SerializeField] private TMP_Text minCostSpace;
    [SerializeField] private TMP_Text maxCostSpace;
    [SerializeField] private GameObject dash;
    public static HoverboxHelper instance;
    private void Awake()
    {
        instance = this;
    }
    public void UpdateIngredient(IngredientScriptableObject ig)
    {
        if (ig == null  ) return;
        dash.SetActive(true);
        ingredientSprite.sprite = ig.icon;
        nameSpace.text = ig.name;
        descriptionSpace.text = ig.description;
        minCostSpace.text = ig.minPrice+"$";
        maxCostSpace.text = ig.maxPrice+ "$";
    }
}
