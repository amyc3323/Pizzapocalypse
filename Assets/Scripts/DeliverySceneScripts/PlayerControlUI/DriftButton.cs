using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftButton : MonoBehaviour
{
    [SerializeField] private Image image;
    public Transform boostButton;
    public int buttonRadius = 90;
    private bool lastButtonState = false;
    [SerializeField] private bool switchSprites = false;

    [SerializeField] private Sprite standardSprite;
    [SerializeField] private Sprite pressedSprite;
    private void Start()
    {
        if (image == null) { image = GetComponent<Image>(); }
    }
    // Update is called once per frame
    void Update()
    {
        bool isHeld = false;
        foreach (Touch tt in Input.touches)
        {
            if ((tt.position - (Vector2)boostButton.transform.position).sqrMagnitude < buttonRadius * buttonRadius) { isHeld = true; break; }
        }
        if (!lastButtonState && isHeld) { if (switchSprites) { image.sprite = pressedSprite; } PlayerScript.instance.isDrifting = true; lastButtonState = true; }
        else if (lastButtonState && !isHeld) { if (switchSprites) { image.sprite = standardSprite; } PlayerScript.instance.isDrifting = false; lastButtonState = false; }


    }
}
