using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Not made by the programmers of this project, taken as a tool.
public class TouchTracker : MonoBehaviour
{

    // This script was made because some objects base their position on a specific touch in Input.touches by keep track of
    // that touch's index.
    // However, the problem with this is that when a touch is removed from this array, due to the user lifting their
    // finger from the screen, the touches shift down the list, and makes the index reference kaput.
    // This script keeps track of touches by their index, and accounts for when a touch is removed or added to Input.touches.


    // the index is static here, and the values of this list are either -1 (no touch referred) or the index of the touch
    // in Input.touches, which, unfortunately, changes over time.
    static int[] TrackedTouches;

    static int nextEmptyIndex;
    static int newTIndex;
    static bool isEmpty;


    void Awake()
    {
        resetTrackedTouches();
    }

    public static int getNewTouch()
    { // returns -1 if it doesn't have a touch. otherwise, give out index of TrackedTouches to be used.
        return newTIndex;
    }

    void resetTrackedTouches()
    {
        TrackedTouches = new int[10000]; // lets hope the user doesn't have more than 100 fingers.
        // (or the patience to go over this limit before it resets when no touch exists)
        for (int i = 0; i < TrackedTouches.Length; i++)
        {
            TrackedTouches[i] = -1;
        }
        nextEmptyIndex = 0;
    }

    public static Touch getTTouch(int tIndex)
    { //get tracked touch!
        return Input.GetTouch(TrackedTouches[tIndex]);
    }

    public static bool hasTTouch(int tIndex)
    { // return whether touch is being HAD
        if (tIndex < 0)
        {
            return false;
        }
        if (TrackedTouches[tIndex] == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    void Update()
    {
        newTIndex = -1; // resets this every loop. if a new touch is found, it will be assigned to the index of TrackedTouches which
        // refers to the new touch.
        if (Input.touchCount <= 0 && !isEmpty)
        { // don't want to constantly rewrite array, hence this little thing.
            resetTrackedTouches();
            isEmpty = true;
        }
        if (Input.touchCount > 0)
        {
            isEmpty = false;
        }
        if (!isEmpty)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                { // new touch found!
                    newTIndex = nextEmptyIndex;
                    TrackedTouches[nextEmptyIndex] = i;
                    nextEmptyIndex++;
                }
                if (Input.GetTouch(i).phase == TouchPhase.Ended)
                { // touch ending. adjust indices accordingly.
                    for (int j = 0; j < TrackedTouches.Length; j++)
                    {
                        if (TrackedTouches[j] > i)
                        { // adjusts index of all touches that are displaced by a touch being removed.
                            TrackedTouches[j] = TrackedTouches[j] - 1;
                        }
                        else if (TrackedTouches[j] == i)
                        {
                            TrackedTouches[j] = -1;
                        }
                    }
                }
            }
        }
    }
}