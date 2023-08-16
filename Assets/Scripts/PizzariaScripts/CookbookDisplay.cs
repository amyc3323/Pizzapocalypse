using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookbookDisplay : MonoBehaviour
{
    [SerializeField] private CookbookDisplayedIngredient[] ingredientsImage;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Pizza currentPizza;
    [SerializeField] private GameObject[] pizzaIngredientsDisplay;
    [SerializeField] private Button selectButton;
    [SerializeField] public Button deleteButton;
    bool firstFrame = true;
    // Update is called once per frame
    private void Start()
    {
        deleteButton.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(firstFrame)SelectPizza();
        firstFrame = false;
        deleteButton.gameObject.SetActive(!currentPizza.Equals(GlobalSceneManager.instance.currentPizza));

    }
    public void UpdateDisplayedPizza(Pizza pizza, RectTransform[] ingredients)
    {
        currentPizza = pizza;
        Debug.Log($"{currentPizza.minPrice }");
        Debug.Log($"{selectButton.gameObject }");
        selectButton.gameObject.SetActive(!currentPizza.Equals(GlobalSceneManager.instance.currentPizza));
        deleteButton.gameObject.SetActive(!currentPizza.Equals(GlobalSceneManager.instance.currentPizza));
        for (int i = 0; i < ingredientsImage.Length; i++) {

            if (pizza.ingredientList.Length <= i) { ingredientsImage[i].SetIngredient((default)); }
            else { ingredientsImage[i].SetIngredient(pizza.ingredientList[i]); }
        }
        for (int i = 0; i < pizzaIngredientsDisplay.Length; i++)
        {
            Destroy(pizzaIngredientsDisplay[i]);

        }
        costText.text = $"{pizza.minPrice} - {pizza.maxPrice} $";
        if (ingredients == null) return;
        pizzaIngredientsDisplay = new GameObject[ingredients.Length];
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i] == null|| ingredients[i].gameObject==null) continue;
            GameObject newIg=Instantiate(ingredients[i].gameObject, transform);
            pizzaIngredientsDisplay[i] =newIg;
            newIg.transform.localScale /= 1.5f;
            newIg.transform.localPosition = Vector2.up*38+Vector2.left*25;
            

        }
    }
    public void SelectPizza()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClick);
        GlobalSceneManager.instance.currentPizza = currentPizza;
        selectButton.gameObject.SetActive(false);

    }
    public void DeletePizza()
    {
        if (GlobalSceneManager.instance.currentPizza == currentPizza) return;
        else { CookbookManager.instance.DeleteRecipe(); }

    }
}
