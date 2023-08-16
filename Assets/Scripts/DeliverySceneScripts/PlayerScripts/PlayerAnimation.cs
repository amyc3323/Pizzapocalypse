using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct ThrowingSprites
{
    public int index;
    public Sprite[] leftFrames;
    public Sprite[] rightFrames;
}

public class PlayerAnimation : MonoBehaviour
{
    public static PlayerAnimation instance;
    [Tooltip("Assumes all directions are equally spaced out")]
    public Sprite[] playerDirectionList;
    public Sprite[] motorcycleDirectionList;
    public Sprite[] pizzaDirectionList;
    public ThrowingSprites[] playerThrowingSprites;
    public ThrowingSprites[] motorcycleThrowingSprites;
    private int currentThrowingIndex=5;
    private float timeOfLastThrowingFrame;
    private bool isThrowingLeft;
    [SerializeField] private float minDelayBetweenFrames;
    public SpriteRenderer playerRend;
    public SpriteRenderer motorcycleRend;
    public SpriteRenderer[] pizzaRend;
    public FollowTransform followTransform;
    [SerializeField] private AnimationCurve velocityToRandomYShake;
    [SerializeField] private float shakingSpeedMultiplier;
    private float currentYShake;
    private bool isShakingUp;
    private Vector2 currentOffset;
    [Header("Surfaces")]
    [SerializeField]private Vector2 onCurbOffset;
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        
        PlayerScript.instance.changeSurfaceEvent.AddListener(UpdateSurface);

    }
    public void UpdateSurface()
    {
        currentOffset += (PlayerScript.instance.currentSurface == SurfaceType.curb ? 1 : -1)*onCurbOffset;
    }
    private void Update()
    {
        float targetVal = velocityToRandomYShake.Evaluate(PlayerScript.instance.rb2D.velocity.magnitude) *(isShakingUp ? 1 : -1);
        currentYShake = Mathf.MoveTowards(currentYShake, targetVal, Time.deltaTime * shakingSpeedMultiplier * Mathf.Abs(targetVal));
        if (currentYShake == targetVal) { isShakingUp = !isShakingUp; }
        followTransform.offset = currentOffset + Vector2.up*currentYShake;

        float rotation = (transform.localEulerAngles.z)% 360 + 11.25f;

        if (rotation < 0)
        {
            rotation += 360;
        }
        //Debug.Log($"Rotation: {rotation} Index {Mathf.FloorToInt(rotation / (360f / directionList.Length))}");
        //Debug.Log($"Sector: {rotation / (360f / playerDirectionList.Length)}");

        playerRend.sprite = playerDirectionList[Mathf.Clamp(Mathf.RoundToInt(rotation / (360f / playerDirectionList.Length)),0, playerDirectionList.Length-1)];
        motorcycleRend.sprite = motorcycleDirectionList[Mathf.Clamp(Mathf.RoundToInt(rotation / (360f / motorcycleDirectionList.Length)), 0, motorcycleDirectionList.Length - 1)];
        for(int i = 0; i < pizzaRend.Length; i++)
        {
            if (GameManager.instance.pizzaAmount > i) {
                pizzaRend[i].enabled = true;
                int sector = Mathf.Clamp(Mathf.RoundToInt(rotation / (360f / pizzaDirectionList.Length)), 0, pizzaDirectionList.Length - 1);
                pizzaRend[i].sortingOrder = Mathf.Clamp(sector, 4, 11) == sector ? 2 : 5;
                pizzaRend[i].sprite = pizzaDirectionList[sector];
            }
            else { pizzaRend[i].enabled = false; }
        }

        TryThrowingAnimations();
    }
    public void StartThrowing(Vector2 direction)
    {
        isThrowingLeft = PlayerScript.instance.transform.InverseTransformDirection(direction).x < 0;
        currentThrowingIndex = 0;
        timeOfLastThrowingFrame = Time.time;
    }
    private void TryThrowingAnimations()
    {
        if (currentThrowingIndex < 4)
        {
            if (Time.time - timeOfLastThrowingFrame > minDelayBetweenFrames) { 
                currentThrowingIndex += 1;
                timeOfLastThrowingFrame = Time.time; 
                if (currentThrowingIndex == 4) { return; }
            }

            float rotation = (transform.localEulerAngles.z) % 360 + 11.25f;

            if (rotation < 0)
            {
                rotation += 360;
            }
            ThrowingSprites pl = playerThrowingSprites[Mathf.Clamp(Mathf.RoundToInt(rotation / (360f / playerThrowingSprites.Length)), 0, playerThrowingSprites.Length - 1)];
            ThrowingSprites mt = motorcycleThrowingSprites[Mathf.Clamp(Mathf.RoundToInt(rotation / (360f / motorcycleThrowingSprites.Length)), 0, motorcycleThrowingSprites.Length - 1)];
            playerRend.sprite = isThrowingLeft?pl.leftFrames[currentThrowingIndex]: pl.rightFrames[currentThrowingIndex];
            motorcycleRend.sprite = isThrowingLeft ? mt.leftFrames[currentThrowingIndex] : mt.rightFrames[currentThrowingIndex];

        }
    }
}
