using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveRecipe
{
    public int minPrice;
    public int maxPrice;
    public SaveIngredient[] ingredients;
    public SaveRecipe(int minPrice, int maxPrice, Ingredient[] ing)
    {
        this.minPrice = minPrice;
        this.maxPrice = maxPrice;
        ingredients = new SaveIngredient[ing.Length];
        for (int i = 0; i < ing.Length; i++)
        {

            ingredients[i] = ing[i].toSaveIngredient();
        }
    }
}
[System.Serializable]
public class Pizza : SaveData
{
    public int minPrice;
    public int maxPrice;
    public Ingredient[] ingredientList;
    public Pizza(Ingredient[] ingredients)
    {
        ingredientList = ingredients;
        int minPrice = 10;
        int maxPrice = 15;
        foreach(Ingredient ig in ingredients)
        {
            for(int i = 0; i < ig.amount; i++)
            {
                minPrice += ig.type.minPrice;
                maxPrice += ig.type.maxPrice;
            }
        }
        this.minPrice = minPrice;
        this.maxPrice = maxPrice;
    }
    public void ActivateBenefits()
    {
       foreach(Ingredient ig in ingredientList)
        {
            for(int i = 0; i < ig.amount; i++) { ig.type.ApplyBenefits(); }
        }
    }
    public string ToJson()
    {
        string json = JsonUtility.ToJson(new SaveRecipe(minPrice,maxPrice,ingredientList));
        Debug.Log("Pizza json: "+json);
        return json;
    }
    public bool LoadJson(string json)
    {
        if (json.Equals("Success")) return false;
        SaveRecipe info = JsonUtility.FromJson<SaveRecipe>(json);
        if (info == null || info.ingredients.Length == 0) return false;
        Debug.Log($"Loaded info-> minPrice: {info.minPrice} maxPrice: {info.maxPrice} ingredients {info.ingredients}");
        minPrice = info.minPrice;
        maxPrice = info.maxPrice;
        ingredientList = new Ingredient[info.ingredients.Length];
        for(int i = 0; i < info.ingredients.Length; i++)
        {
            if (info.ingredients[i] == null) { continue; }
            bool success = true ;
            /*Debug.Log("IL: "+(ingredientList[i] == null));
            Debug.Log("GSM: " + (GlobalSceneManager.instance == null));

            Debug.Log("info: " +(info == null));

            Debug.Log("IL: " + (info.ingredients[i] == null));*/
            ingredientList[i] = new Ingredient();


            ingredientList[i].type = GlobalSceneManager.instance.getIngredientType(info.ingredients[i].id, out success);

            if (!success || ingredientList[i].amount < 0) { ingredientList[i].amount = -1; }
            else ingredientList[i].amount = 1;
        }
        return true;
    }
}
