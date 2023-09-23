using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DraggedIngredient : MonoBehaviour//, IPointerUpHandler
{
    public IngredientScriptableObject ingredient;
    public Image iconImage;
    public GameObject pizzaIngredientPrefab;
    [SerializeField] private AudioClip dropVegSound;
    [SerializeField] private AudioClip dropMeatSound;

    public int pizzaRadius = 100;
    public void ChangeIngredient(IngredientScriptableObject newIngredient)
    {
        ingredient = newIngredient;
        iconImage.sprite = ingredient.draggedIcon[Mathf.Clamp(PizzeriaPizza.instance.GetIngredientIndex(newIngredient),0,2)];
    }
    private void Update()
    {
        Debug.Log((PizzeriaPizza.instance.transform.position - transform.position).sqrMagnitude < pizzaRadius * pizzaRadius);
        transform.position = Input.mousePosition;
        if (Input.GetMouseButtonUp(0)) { MouseUp(); }
    }
    public void MouseUp()
    {
        if (!PizzeriaPizza.instance.isCooking && (PizzeriaPizza.instance.transform.position-transform.position).sqrMagnitude< pizzaRadius* pizzaRadius&&PizzeriaPizza.instance.ingredients.Count<4)
        {
            GlobalSoundManager.instance.playSFX(dropVegSound);
            /*
             * if (isMeat)
             * {
             *     GlobalSoundManager.instance.playSFX(dropMeatSound);
             * }
             * else
             * {
             *     GlobalSoundManager.instance.playSFX(dropVegSound);
             * }
             * 
             */
            GameObject visualIngredient = Instantiate(pizzaIngredientPrefab, PizzeriaPizza.instance.transform);
            visualIngredient.GetComponent<Image>().sprite = iconImage.sprite;
            PizzeriaPizza.instance.AddIngredient(new Ingredient(ingredient, 1), visualIngredient.GetComponent<RectTransform>()) ;
           
        }
        else IngredientManager.instance.AddIngredient(new Ingredient(ingredient,1));
        Destroy(gameObject);
    }

}
