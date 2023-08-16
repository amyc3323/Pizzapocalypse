using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BoostButton : MonoBehaviour
{
    public Image image;
    public Button boostButton;
    public int buttonRadius=90;
    private Image boostButtonImage;
    public Sprite boostReadyImage;
    public Sprite boostNotReadyImage;
    private bool lastButtonState = false;
    private void Start()
    {
        boostButtonImage = boostButton.GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {
        bool isHeld=false;
        foreach(Touch tt in Input.touches)
        {
            if ((tt.position - (Vector2)boostButton.transform.position).sqrMagnitude < buttonRadius * buttonRadius) { isHeld = true;break; }
        }
        if (!lastButtonState&& isHeld) { Boost(); lastButtonState = true; }
        else if (lastButtonState && !isHeld) { Boost(); lastButtonState = false; }

        image.fillAmount = PlayerScript.instance.boostMeter;
        boostButton.interactable = image.fillAmount > 0f;
        boostButtonImage.sprite = boostButton.interactable ? boostReadyImage : boostNotReadyImage;
        
    }
    public void Boost()
    {
        Debug.Log("BOOSTING");
        PlayerScript.instance.Boost();
    }
}

