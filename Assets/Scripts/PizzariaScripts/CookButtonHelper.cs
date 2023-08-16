using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button),typeof(Image))]
public class CookButtonHelper : MonoBehaviour
{
    public static float pauseDuration = 8;
    private Button button;
    private Image image;
    [SerializeField]private GameObject remakePizza;
    private bool targetButtonState=true;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        targetButtonState = true;
    }
    private void Update()
    {
        if (targetButtonState&&!button.enabled)
        {
            for(int i = 0; i < 9; i++) { 
                if (!CookbookManager.instance.isRecipeFilled(i))
                {
                    button.enabled = true;
                    image.enabled = true;
                    
                    break;
                } 
            }
        }
    }
    public void Cook()
    {
        StartCoroutine(_Cook());
    }
    public IEnumerator _Cook()
    {
        image.enabled = false;
        button.enabled = false;
        targetButtonState = false;
        remakePizza.SetActive(false);
        yield return new WaitForSeconds(pauseDuration);
        targetButtonState = true;
        remakePizza.SetActive(true);
    }
}
