using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Tooltip("Color cannot change at runtime")]
public class HouseScript : MonoBehaviour
{
    [SerializeField]private SpriteRenderer rend;
    [SerializeField] private SpriteRenderer foundationSprite;
    private Color origColor;
    private Color transparentColor;
    [Range(0f,1f),SerializeField] private float transparentAlpha=0.1f;
    [Range(0f,1f),SerializeField] private float visibleAlpha = 1f;
    [SerializeField] private float transitionSpeed = 1f;
    public float xWidth = 5;
    private void Awake()
    {
        if (rend == null)rend = GetComponent<SpriteRenderer>();
        origColor = new Color(rend.color.r, rend.color.g, rend.color.b, visibleAlpha);
        transparentColor = new Color(origColor.r, origColor.g, origColor.b, transparentAlpha);
    }
    // Update is called once per frame
    void Update()
    {
        rend.color = Color.Lerp(rend.color,(Mathf.Abs(transform.position.x-PlayerScript.instance.transform.position.x)<xWidth&&(transform.position.y-2f) < PlayerScript.instance.transform.position.y) ? transparentColor : origColor,transitionSpeed*Time.deltaTime);
        if (foundationSprite != null)foundationSprite.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f - rend.color.a);
    }
}
