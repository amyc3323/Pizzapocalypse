using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct SpawnIngredientInfo
{
    public Ingredient ingredientType;
    public float minSpawnDelay;
    public float maxSpawnDelay;
    public float timeOfLastSpawn;
    public float currentDelay;
    public void SetDelay(float val) { currentDelay = val; }
    public void SetTime(float val) { timeOfLastSpawn = val; }
}
public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnLocationsParent;
    [SerializeField] GameObject droppedIngredientPrefab;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private float spawnRadius;
    [SerializeField] public SpawnIngredientInfo []ingredientsInfo;
    [ContextMenu("Reset Spawn Locations")]
    public void ResetSpawnLocations()
    {
        spawnLocations = new Transform[spawnLocationsParent.childCount];
        for(int i = 0; i < spawnLocationsParent.childCount; i++)
        {
            spawnLocations[i] = spawnLocationsParent.GetChild(i);
        }
    }
    private void Start()
    {
        for(int i=0;i<ingredientsInfo.Length;i++)
        {
            ingredientsInfo[i].SetDelay(Random.Range(ingredientsInfo[i].minSpawnDelay, ingredientsInfo[i].maxSpawnDelay));
            ingredientsInfo[i].SetTime(Time.timeSinceLevelLoad);
        }
    }
    private void Update()
    {
        for (int i = 0; i < ingredientsInfo.Length; i++)
        {
            if (Time.timeSinceLevelLoad- ingredientsInfo[i].timeOfLastSpawn > ingredientsInfo[i].currentDelay)
            {
                GameObject spawn=Instantiate(droppedIngredientPrefab, spawnLocations[Random.Range(0, spawnLocations.Length)].position+(Vector3)Random.insideUnitCircle, Quaternion.identity);
                spawn.GetComponent<GroundIngredient>().currentIngredient = ingredientsInfo[i].ingredientType;
                spawn.GetComponent<GroundIngredient>().Reset();

                ingredientsInfo[i].SetDelay(Random.Range(ingredientsInfo[i].minSpawnDelay, ingredientsInfo[i].maxSpawnDelay));
                ingredientsInfo[i].SetTime(Time.timeSinceLevelLoad);
            }
        }
    }


}
