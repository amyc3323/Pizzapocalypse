using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveIngredient { public int id; public int amount; public SaveIngredient(int id, int amount) { this.id = id; this.amount = amount; } }
[System.Serializable]
public class Ingredient: SaveData
{
    public  IngredientScriptableObject type;
    public int amount;
    public void Print()
    {
        Debug.Log($"Name: {type.name} Amount: {amount}");
    }
    public Ingredient(IngredientScriptableObject ingredient, int amount)
    {
        type = ingredient;
        this.amount = amount;
    }
    public Ingredient() { }
    public void IncreaseAmount(int amt) { amount += amt; }

    public SaveIngredient toSaveIngredient()
    {
        return new SaveIngredient(GlobalSceneManager.instance.getIngredientID(type), amount);
    }
    public string ToJson()
    {
        string json = JsonUtility.ToJson(toSaveIngredient());
        Debug.Log("Ingredient json: " + json);
        return json;
    }
    public bool LoadJson(string json)
    {
        if (json == "Success"){
            amount = -1;
        return false; }
        SaveIngredient info = JsonUtility.FromJson<SaveIngredient>(json);
        bool success=true;
        if (info == null) { amount = -1; return false; }
        IngredientScriptableObject iso = GlobalSceneManager.instance.getIngredientType(info.id, out success);

        if(!success||info.amount<0) { amount = -1; return false; }
        Debug.Log($"Loaded info-> Type: {iso} Amount: {info.amount}");
        type = iso;
        amount = info.amount;
        return true;
    }
}
public class IngredientManager : MonoBehaviour
{
    public static IngredientManager instance;
    
    [SerializeField] private Ingredient[] defaultIngredients;
    [SerializeField]private  IngredientSlot[] ingredientSlots;
    private static int xOffset=50;
    private static int yOffset =60;
    private static int iconWidth=4;
    Dictionary<IngredientScriptableObject, IngredientSlot> ingredientToSlotDictionary;
    public void Awake()
    {
        instance = this;
        ingredientToSlotDictionary = new Dictionary<IngredientScriptableObject, IngredientSlot>();

    }
    
    
    public void AddIngredient(Ingredient ingredient)
    {
        ingredient.Print();
        if (ingredientToSlotDictionary.ContainsKey(ingredient.type)) 
        {
            ingredientToSlotDictionary[ingredient.type].IncreaseAmount(ingredient.amount);

        }
        else
        {
            ingredientSlots[ingredient.type.trueOrder].AddIngredient(ingredient);
            ingredientToSlotDictionary.Add(ingredient.type, ingredientSlots[ingredient.type.trueOrder]);
        }
    }
    public bool UseIngredient(Ingredient ingredient)
    {
        if (ingredientToSlotDictionary.TryGetValue(ingredient.type, out IngredientSlot slot))
        {
            return slot.UseAmount(ingredient.amount);
        }
        else  return false; 
    }
    private void Start()
    {
        //foreach(Ingredient ingredient in defaultIngredients) { AddIngredient(ingredient); }
        Load();
        while (GlobalSceneManager.instance.storedIngredients.Count > 0)
        {
            AddIngredient(GlobalSceneManager.instance.storedIngredients[0]);

            GlobalSceneManager.instance.storedIngredients.RemoveAt(0);
        }
        GlobalSceneManager.instance.onChangeSceneEvent.AddListener(Save);

    }
    [ContextMenu("Reset Layout")]
    public void ResestLayout()
    {
        for(int i = 0; i < ingredientSlots.Length;i++)
        {
            ingredientSlots[i].transform.localPosition = new Vector2(xOffset * (i % iconWidth)-xOffset, -yOffset * (i / iconWidth)+yOffset);
        }
    }
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
        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            GlobalSaveManager.instance.SaveData("Ingredient" + i.ToString(), ingredientSlots[i].convertToIngredient());
        }
    }

    public void Load()
    {

        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            Ingredient ig=new Ingredient(defaultIngredients[0].type,0);
            string data = "";
            if (GlobalSaveManager.instance.LoadData("Ingredient" + i.ToString(), out data))
            {
                
                bool result=ig.LoadJson(data);
                
                if(result)ingredientSlots[i].ConvertFromIngredient(ig);
            }
            else { ingredientSlots[i].Clear(); }
        }
    }
}
