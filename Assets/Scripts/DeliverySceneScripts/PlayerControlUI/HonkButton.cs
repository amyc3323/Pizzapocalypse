using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HonkButton : MonoBehaviour
{
    [SerializeField] private Image image;

    public Transform boostButton;

    public AudioClip[] honkClips;
    private static int currentIndex;
    public int buttonRadius = 90;
    private bool lastButtonState = false;

    [SerializeField] private bool switchSprites = false;

    [SerializeField] private Sprite standardSprite;
    [SerializeField] private Sprite pressedSprite;
    private void Awake()
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
        if (!lastButtonState && isHeld) { if (switchSprites) { image.sprite = pressedSprite; } lastButtonState = true; Honk(); }
        else if (lastButtonState && !isHeld) { if (switchSprites) { image.sprite = pressedSprite; } lastButtonState = false; }


    }
    public void Honk()
    {
        GlobalSoundManager.instance.playSFX(honkClips[currentIndex++]);
        currentIndex %= honkClips.Length;
    }
}
