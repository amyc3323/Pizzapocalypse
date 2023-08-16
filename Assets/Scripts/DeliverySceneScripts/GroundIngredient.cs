using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundIngredient : MonoBehaviour
{
    public Ingredient currentIngredient;
    public GameObject collectionVFX;
    [SerializeField] private AudioClip[] pickSFX;

    public void Reset()
    {
        GetComponent<SpriteRenderer>().sprite = currentIngredient.type.icon;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            GlobalSoundManager.instance.playSFX(pickSFX[Random.Range(0, pickSFX.Length)], 1);
            GlobalSceneManager.instance.storedIngredients.Add(currentIngredient);
            if(collectionVFX!=null)Instantiate(collectionVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
