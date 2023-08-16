using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class IngredientSlot : MonoBehaviour,IPointerDownHandler
{
    public IngredientScriptableObject ingredient;

    public DraggedIngredient draggedIngredient;
    [SerializeField] public Image iconImage;
    [SerializeField] private TMP_Text amountText;
    public int amount { private set; get; }
    private void Awake()
    {
        iconImage.enabled = false;
        amountText.text = "";

    }
    public void Clear()
    {

        iconImage.enabled = false;
        amountText.text = "";
    }
    public void AddIngredient(Ingredient ingredient)
    {
        iconImage.enabled = true;
        this.ingredient = ingredient.type;
        iconImage.sprite = ingredient.type.icon;
        this.amount += ingredient.amount;
        amountText.text = amount.ToString();
    }
    public void IncreaseAmount(int amt)
    {
        amount += amt;
        amountText.text = amount.ToString();

    }
    public bool UseAmount(int amt) {
        if (amount < amt) return false; 
        amount -= amt;
        amountText.text = amount.ToString();
        return true;
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClick);
        HoverboxHelper.instance.UpdateIngredient(ingredient);
        if (UseAmount(1)) {
            DraggedIngredient di = Instantiate(draggedIngredient, Input.mousePosition, Quaternion.identity);
            di.ChangeIngredient(ingredient);
            di.transform.parent = PizzariaCanvasManager.instance.transform;
        }
    }
    public Ingredient convertToIngredient()
    {
        return new Ingredient(ingredient, amount);
    }
    public void ConvertFromIngredient(Ingredient ingredient)
    {

        if (ingredient == null||ingredient.type==null||ingredient.type.trueOrder<0) return;
        iconImage.enabled = true;
        this.ingredient = ingredient.type;
        iconImage.sprite = ingredient.type.icon;
        this.amount = ingredient.amount;
        amountText.text = amount.ToString();
    }
}
