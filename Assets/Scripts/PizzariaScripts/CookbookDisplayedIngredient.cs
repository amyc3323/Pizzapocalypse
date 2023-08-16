using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class CookbookDisplayedIngredient : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;
    private Image display;
    private void Awake()
    {
        display = GetComponent<Image>();
        display.enabled = false;
    }
    public void SetIngredient(Ingredient ig)
    {
        if (ig==null||ig.type==null) { display.enabled = false; return; }
        display.enabled = true;
        display.sprite = ig.type.icon;
        ingredient = ig;
    }
}
