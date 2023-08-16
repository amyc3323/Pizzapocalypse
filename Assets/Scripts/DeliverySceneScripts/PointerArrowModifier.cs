using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FaceTransform))]
public class PointerArrowModifier : MonoBehaviour
{
    public float minSize;
    public float maxSize;
    [SerializeField] private AnimationCurve arrowSizePerDistanceCurve;
    [SerializeField] private float xScale;
    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private Color pizzeriaArrowColor;
    [SerializeField] private Color defaultArrowColor;
    private FaceTransform faceTransform;
    private void Awake()
    {
        faceTransform = GetComponent<FaceTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform target = GameManager.instance.pizzaAmount > 0 ? DeliveryTarget.instance.transform : PizzeriaBuilding.instance.transform;
        transform.localScale = Vector2.one * Mathf.Clamp(arrowSizePerDistanceCurve.Evaluate(((Vector2)target.position-(Vector2)transform.position).magnitude/xScale), minSize, maxSize);
        faceTransform.targetTransform = target;
        rend.color = GameManager.instance.pizzaAmount > 0 ? defaultArrowColor : pizzeriaArrowColor;
    }
}
