using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupHelper : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [Range(0f, 1f)] private static float interactableAlphaThreshold = 0.5f;
    [Range(0f, 1f), SerializeField] private float onAlpha = 1f;
    [Range(0f, 1f), SerializeField] private float offAlpha = 0f;
    [Header("Debug")]
    [SerializeField] private float transitionSpeed = 1f;
    [Range(0f, 1f), SerializeField] float targetAlpha = 0f;
    [SerializeField] private bool currentState;
    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = currentState?onAlpha:offAlpha;
        UpdateCanvasInteractable();
    }
    private void ChangeTargetAlpha(float alpha){targetAlpha= alpha; UpdateCanvasInteractable();StopAllCoroutines(); StartCoroutine(transitionAlpha()); }
    public void SetState(bool state) { currentState = state; ChangeTargetAlpha(state ? onAlpha : offAlpha); }

    public void SetOn()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiOpen, 1);
        SetState(true);
    }

    public void SetCookBookOn()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiOpen, 1);
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.bookFlipOpen, 1);
        SetState(true);
    }

    public void SetOff()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClose, 1);
        SetState(false);
    }

    public void SetCookBookOff()
    {
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.uiClose, 1);
        GlobalSoundManager.instance.playSFX(GlobalSoundManager.instance.bookFlipClose, 1);
        SetState(false);
    }

    public void ToggleState(){ SetState(!currentState); }
    private void UpdateCanvasInteractable()
    {
        canvasGroup.interactable = targetAlpha >= interactableAlphaThreshold;
        canvasGroup.blocksRaycasts = targetAlpha >= interactableAlphaThreshold;
    }

    // Update is called once per frame
    IEnumerator transitionAlpha()
    {
        while (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, transitionSpeed * 0.02f);
            UpdateCanvasInteractable();
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }
}
