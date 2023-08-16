using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RecipeSlot : MonoBehaviour
{
    public bool isFilled = false;
    [SerializeField] private Pizza pizzaType;
    private float sizeMultiplier = 0.5f;
     Button button;
    RectTransform[] ingredients;
    [SerializeField] private CookbookDisplay cookbookDisplay;
    [SerializeField] private RectTransform selectionOutline;
    [SerializeField] public int id;
    public GameObject pizzaIngredientPrefab;
    public RectTransform referenceRect;
    private static int currentSelected;

    public Pizza GetPizza()
    {
        return pizzaType;
    }
    public void Awake()
    {
        button = GetComponent<Button>();
        button.enabled = false;
    }
    public void ReaddIngredients()
    {
        isFilled = !(pizzaType == null || pizzaType.ingredientList.Length == 0);
        if (!isFilled) return;
        button.enabled = true;
        int index = 0;
        ingredients = new RectTransform[4];
        int i = -1;
        
        foreach (Ingredient ig in pizzaType.ingredientList)
        {
            if (ig == null || ig.type == null) continue;
            GameObject visualIngredient = Instantiate(pizzaIngredientPrefab, transform);
            visualIngredient.GetComponent<Image>().sprite = ig.type.draggedIcon[Mathf.Clamp(i++,0,2)];
            RectTransform rt = visualIngredient.GetComponent<RectTransform>();
            this.ingredients[index] = rt;
            index++;

            rt.localScale = referenceRect.localScale * 2f;
            rt.anchoredPosition = referenceRect.anchoredPosition;
            rt.anchorMax = referenceRect.anchorMax;
            rt.anchorMin = referenceRect.anchorMin;
            rt.SetParent(transform,false);

            rt.localPosition = Vector2.zero;
        }
    }

    public int GetIngredientIndex(IngredientScriptableObject ig)
    {
        int amt = 0;
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (pizzaType.ingredientList[i].type.Equals(ig))
            {
                amt++;
            }
        }
        return amt;
    }
    public void UpdatePizza(Pizza pizza, RectTransform[] ingredients)
    {
        button.enabled = true;
        isFilled = true;
        pizzaType = pizza;
        this.ingredients = ingredients;
        foreach(RectTransform rt in ingredients)
        {
            rt.gameObject.SetActive(true);
            rt.localScale = referenceRect.localScale*2f;
            rt.anchoredPosition = referenceRect.anchoredPosition;
            rt.anchorMax = referenceRect.anchorMax;
            rt.anchorMin = referenceRect.anchorMin;
            rt.SetParent(transform, false);

            rt.localPosition = Vector2.zero;
        }
    }
    public void SetDisplay()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClick);
        CookbookManager.instance.currentSelectedSlot = id;
        cookbookDisplay.UpdateDisplayedPizza(pizzaType, ingredients);
        currentSelected = id;
        //cookbookDisplay.deleteButton.gameObject.SetActive(false);

    }
    public void ClearRecipe()
    {
        button.enabled = false;
        isFilled = false;
        pizzaType = null;
        foreach (RectTransform rt in ingredients)
        {
            if (rt != null && rt.gameObject != null)
            {
                Destroy(rt.gameObject);
            }
        }
        ingredients = new RectTransform[4];
    }
}
