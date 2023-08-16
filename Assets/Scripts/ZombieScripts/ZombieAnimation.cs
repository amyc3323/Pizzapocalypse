using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ZombieSpriteSheet
{
    public Sprite[] sprites;
}

[RequireComponent(typeof(RequireComponent), typeof(Rigidbody2D))]
public class ZombieAnimation : MonoBehaviour
{
    private SpriteRenderer rend;
    private Rigidbody2D rb2D;
    private ZombieBehavior behavior;
    public int frame;
    public float intervalBetweenFrames;
    public float lastFrameTime;
    [SerializeField] private Transform testTransform;
    public ZombieSpriteSheet[] zombieSprites;
    public Sprite[] zombieEatAnimation;
    public Sprite idleSprite;
    public float intervalBetweenEatingFrames;
    public int eatingFrame;
    bool eatAnimActive;
    public float timeUntilNextEatFrame;

    Vector2 currentDirection;
    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        behavior = GetComponent<ZombieBehavior>();
        eatingFrame = -1;
        eatAnimActive = false;
        frame = 0;
    }
    private void Update()
    {
        if (behavior.eatingPizza) // temp && false
        {
            //10 fps like player
            timeUntilNextEatFrame -= Time.deltaTime;

            //Debug.Log(timeUntilNextEatFrame);
            if (timeUntilNextEatFrame <= 0)
            {
                eatingFrame++;
                timeUntilNextEatFrame = intervalBetweenEatingFrames;
            }
            if (eatingFrame >= zombieEatAnimation.Length)
                eatingFrame = zombieEatAnimation.Length - 2;

            //Debug.Log(eatingFrame, this);
            rend.sprite = zombieEatAnimation[eatingFrame];
        }

        if (!behavior.eatingPizza && !behavior.aggroed)
        {
            rend.sprite = idleSprite;
        }

        if (rb2D.velocity.normalized == Vector2.zero) return;
        Vector3 val = rb2D.velocity.normalized;
        testTransform.right = val;
        float rotation = testTransform.eulerAngles.z % 360;

        if (rotation < 0)
        {
            rotation += 360;
        }
        //Debug.Log($"Rotation: {rotation} Index {Mathf.FloorToInt(rotation / (360f / zombieSprites.Length))}");

        //Debug.Log($"eating {behavior.eatingPizza} aggroed {behavior.aggroed}");
        if (behavior.aggroed && !behavior.eatingPizza)
        {
            if (Time.time - lastFrameTime > intervalBetweenFrames)
            {
                frame++;
                lastFrameTime = Time.time;
            }
            if (frame >= zombieSprites[0].sprites.Length)
                frame = 0;
            rend.sprite = zombieSprites[Mathf.FloorToInt(rotation / (360f / zombieSprites.Length))].sprites[frame];
        }
        else
        {
            //do nothing for now i guess
        }
    }
}
