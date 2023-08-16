using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PizzeriaPizza : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private IngredientScriptableObject dummyType;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject pizzaIngredientPrefab;
    public static PizzeriaPizza instance;
    [SerializeField] private List<Ingredient> defaultStartingIngredients;
    public List<Ingredient> ingredients = new List<Ingredient>();
    private List<RectTransform> visibleIngredients = new List<RectTransform>();
    int typesOfIngredients = 0;
    public bool isCooking { private set; get; }

    [SerializeField] private AudioClip cookingSound;
    [SerializeField] private AudioClip finishCookingSound;

    public Gradient cookingGradient;
    /*    private bool isDragging = false;
        private Vector2 draggingOffset;
        [SerializeField] private int maxY;
        [SerializeField] private int minY;
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            isDragging = true;
            Debug.Log("MOUSE DOWN");
            draggingOffset = pointerEventData.position - (Vector2)transform.position;
        }
        public void OnPointerUp(PointerEventData pointerEventData)
        {
            isDragging = false;    
        }*/
    public IngredientScriptableObject dummyDough;
    public void Awake()
    {
        instance = this;
        if (animator == null) { animator = GetComponent<Animator>(); }
    }
    private void Start()
    {
        Load();
        GlobalSceneManager.instance.onChangeSceneEvent.AddListener(Save);
        Debug.Log("Count: " + ingredients.Count);
        if (ingredients.Count == 0) ResetIngredients();
        else ResetVisuals();
    }
    public void ResetIngredients()
    {

        isCooking = false;
        visibleIngredients = new List<RectTransform>();
        ingredients = new List<Ingredient>();
        foreach (RectTransform rt in visibleIngredients) { Destroy(rt.gameObject); }
        foreach (Ingredient ig in defaultStartingIngredients)
        {
            GameObject visualIngredient = Instantiate(pizzaIngredientPrefab, transform);
            visualIngredient.GetComponent<Image>().sprite = ig.type.draggedIcon[0];
            AddIngredient(new Ingredient(ig.type, ig.amount), visualIngredient.GetComponent<RectTransform>());
        }
        Save();
    }

    public void ResetVisuals()
    {

        isCooking = false;
        foreach(RectTransform rt in visibleIngredients) { Destroy(rt.gameObject); }
        List<Ingredient> tempList = ingredients;
        ingredients = new List<Ingredient>();
        visibleIngredients = new List<RectTransform>();
        foreach (Ingredient ig in tempList)
        {
            if (ig == null || ig.type == null) continue;
            GameObject visualIngredient = Instantiate(pizzaIngredientPrefab, transform);
            visualIngredient.GetComponent<Image>().sprite = ig.type.draggedIcon[GetIngredientIndex(ig.type)];
            visibleIngredients.Add(visualIngredient.GetComponent<RectTransform>());
            ingredients.Add(ig);
        }
    }
    public void AddIngredient(Ingredient ingredient, RectTransform visibleIngredient)
    {
        this.visibleIngredients.Add(visibleIngredient);
        /*for(int i=0;i<ingredients.Count;i++)
        {
            if (ingredients[i].type.Equals(ingredient.type))
            {
                ingredients[i].IncreaseAmount(ingredient.amount);
                return;
            }
        }*/
     ingredients.Add(ingredient);
    }
    public int GetIngredientIndex(IngredientScriptableObject ig)
    {
        int amt = 0;
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (ingredients[i].type.Equals(ig))
            {
                amt++;
            }
        }return amt;
    }
    public void Cook()
    {
        Pizza createdPizza = new Pizza(ingredients.ToArray());
        Debug.Log($"Does Cookbook manager exist: {CookbookManager.instance == null} does the pizza exist: {createdPizza.maxPrice}");
        CookbookManager.instance.AddRecipe(createdPizza,visibleIngredients.ToArray());
        ResetIngredients();
    }
    public void Clear()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClick);
        foreach(Ingredient ig in ingredients)
        {
            if (ig.type.trueOrder < 0) continue;
            IngredientManager.instance.AddIngredient(ig);
        }
        ResetIngredients();
    }

    public void StartCookCoroutine()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClick);
        StartCoroutine(_CookCoroutine());
    }
    public IEnumerator _CookCoroutine()
    {
        GlobalSoundManager.instance.playSFX(cookingSound);
        isCooking = true;
        animator.SetTrigger("Cook");
        Image[] ingredients = new Image[visibleIngredients.Count];
        for(int i = 0; i < visibleIngredients.Count; i++)
        {
            ingredients[i] = visibleIngredients[i].GetComponent<Image>();
        }
        for (int i = 0; i < 300; i++)
        {
            foreach (Image rend in ingredients)
            {
                rend.color = cookingGradient.Evaluate(i / 300f);
            }

            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1f);

        GlobalSoundManager.instance.playSFX(finishCookingSound);
        //Animation calls the function
        //Cook();
    }
    private void Update()
    {
        /*if (isDragging)
        {
            int closestDistanceSqr=int.MaxValue;
            int closestTouchIndex=0;
            foreach(Touch t in Input.touches)
            {

            }
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(Input.mousePosition.y-draggingOffset.y, minY, maxY), transform.position.z);
        }*/
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

        for (int i = 0; i < 4; i++)
        {
            if (i>=ingredients.Count||ingredients[i] == null) { GlobalSaveManager.instance.SaveData("PizzeriaIngredient" + i.ToString(), new Ingredient(dummyDough,-1)); }
            else GlobalSaveManager.instance.SaveData("PizzeriaIngredient" + i.ToString(), ingredients[i]);
        }
    }

    public void Load()
    {
        ingredients = new List<Ingredient>();
        /*Reset*/
        isCooking = false;
        visibleIngredients = new List<RectTransform>();
        ingredients = new List<Ingredient>();
        foreach (RectTransform rt in visibleIngredients) { Destroy(rt.gameObject); }
        foreach (Ingredient ig in defaultStartingIngredients)
        {
            GameObject visualIngredient = Instantiate(pizzaIngredientPrefab, transform);
            visualIngredient.GetComponent<Image>().sprite = ig.type.draggedIcon[0];
            AddIngredient(new Ingredient(ig.type, ig.amount), visualIngredient.GetComponent<RectTransform>());
        }
        /*End of reset*/
        for (int i = 1; i < 4; i++)
        {

            string data = "";
            if(GlobalSaveManager.instance.LoadData("PizzeriaIngredient" + i.ToString(), out data))
            {
                Ingredient ig = new Ingredient(dummyType, 0);
                bool result=ig.LoadJson(data);
                if (result) ingredients.Add(ig);
            }
        }
        //foreach(Ingredient ig in ingredients) { Debug.Log($"Original Ingredients: {ig.type} {ig.amount}"); }
        //while (ingredients.Count>0&&ingredients[ingredients.Count-1]==null) ingredients.RemoveAt(ingredients.Count - 1);
    }
}
