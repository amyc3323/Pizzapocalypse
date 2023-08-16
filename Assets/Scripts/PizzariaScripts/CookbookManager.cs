using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct RectTransformList
{
    public RectTransform[] rt;
}
public class CookbookManager : MonoBehaviour
{
    public static CookbookManager instance;
    [SerializeField] private Pizza[] defaultRecipes;
    [SerializeField] private RectTransformList[] defaultRectTransformList;
    [SerializeField] private List<RecipeSlot> recipeSlots;
    private static int xOffset = 280;
    private static int yOffset = 280;
    private static int iconWidth = 3;
    public int currentSelectedSlot;
    [SerializeField] private RectTransform selectionOutline;
    private void Awake()
    {
        instance = this;
    }
    public bool isRecipeFilled(int index)
    {
        return recipeSlots[index].isFilled;
    }
    private void Update()
    {
        if(recipeSlots[currentSelectedSlot].GetPizza().Equals(GlobalSceneManager.instance.currentPizza))selectionOutline.position = recipeSlots[currentSelectedSlot].transform.position;

    }
    public void Start()
    {

        Load();
        GlobalSceneManager.instance.onChangeSceneEvent.AddListener(Save);
        if (recipeSlots[currentSelectedSlot].GetPizza() == null || recipeSlots[currentSelectedSlot].GetPizza().minPrice == 0)
        {
            for (int i = 0; i < defaultRecipes.Length; i++)
            {
                AddRecipe(defaultRecipes[i], defaultRectTransformList[i].rt);
                currentSelectedSlot = 0;
                PlayerPrefs.SetInt("CurrentSelectedIndex", currentSelectedSlot);
            }
        }

        recipeSlots[currentSelectedSlot].SetDisplay();

        
        UpdateSelectionOutline();
        recipeSlots[currentSelectedSlot].SetDisplay();
        
    }
    public void AddRecipe(Pizza recipe, RectTransform[] ingredientArt)
    {
        foreach (RecipeSlot rs in recipeSlots)
        {

            if (!rs.isFilled)
            {
                currentSelectedSlot = rs.id; rs.UpdatePizza(recipe, ingredientArt); GlobalSceneManager.instance.currentPizza = recipe; rs.SetDisplay(); return; }
        }
        UpdateUI();
        UpdateSelectionOutline();
    }
    public void DeleteRecipe()
    {
        recipeSlots[currentSelectedSlot].ClearRecipe();

        foreach (RecipeSlot rs in recipeSlots)
        {
            if (rs.isFilled) { rs.SetDisplay();return; }
        }
    }
    [ContextMenu("Reset Layout")]
    public void ResestLayout()
    {
        for (int i = 0; i < recipeSlots.Count; i++)
        {

            recipeSlots[i].transform.localPosition = new Vector2(xOffset * (i % iconWidth)-xOffset*1.1f, -yOffset * (i / iconWidth) + yOffset);
        }

    }
    public void UpdateSelectionOutline()
    {
        selectionOutline.position = recipeSlots[currentSelectedSlot].transform.position;
    }
    public void UpdateUI() { }
    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
        else Load();
    }
    private void OnApplicationQuit()
    {
        Save();   
    }
    public void Save()
    {
        PlayerPrefs.SetInt("CurrentSelectedIndex", currentSelectedSlot);
        for(int i = 0; i < recipeSlots.Count; i++)
        {
            if(recipeSlots[i].GetPizza()!=null&&recipeSlots[i].GetPizza().minPrice!=0) GlobalSaveManager.instance.SaveData("Recipe" + i.ToString(), recipeSlots[i].GetPizza());

            else GlobalSaveManager.instance.ClearData("Recipe" + i.ToString());
        }
    }

    public void Load()
    {

        for (int i = 0; i < recipeSlots.Count; i++)
        {
            string data="";
            if (GlobalSaveManager.instance.LoadData("Recipe" + i.ToString(), out data))
            {
                recipeSlots[i].GetPizza().LoadJson(data);
                recipeSlots[i].ReaddIngredients();
            }
        }
        currentSelectedSlot = PlayerPrefs.GetInt("CurrentSelectedIndex");
    }
}
