using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIImageDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public RectTransform targetSlot;
    
    [Header("Settings")]
    public float rotationCompletionThreshold = 0.3f;

    [Header("Colors")]
    public Color defaultColor = Color.blue;
    public Color hoverColor = Color.yellow;
    public Color snappedColor = Color.green;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Image myImage;
    private Vector2 originalPosition;
    private Quaternion originalRotation;
    private float totalDistanceToTarget;
    private bool alignRightSide;
    private Vector3 dragStartWorldPosition;
    private float maxDistanceFromStart;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        myImage = GetComponent<Image>();
        
        originalPosition = rectTransform.anchoredPosition;
        originalRotation = rectTransform.localRotation;
        myImage.color = defaultColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StopAllCoroutines();
        transform.SetAsLastSibling();
        
        dragStartWorldPosition = rectTransform.position;
        totalDistanceToTarget = Vector2.Distance(dragStartWorldPosition, targetSlot.position);
        maxDistanceFromStart = 0f;

        Vector3 initialDirection = targetSlot.position - rectTransform.position;
        alignRightSide = initialDirection.x >= 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        ApplyDynamicRotation();

        myImage.color = IsOverlappingEnough() ? hoverColor : defaultColor;
    }

    private void ApplyDynamicRotation()
    {
        if (targetSlot == null || totalDistanceToTarget <= 0) return;

        // 1. Current direction to target
        Vector3 direction = targetSlot.position - rectTransform.position;
        float angleToTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 2. Target angle based on which side leads
        float targetAngle;

        if (alignRightSide)
            targetAngle = angleToTarget;
        else
            targetAngle = angleToTarget + 180f;

        if (targetAngle > 180f) targetAngle -= 360f;
        if (targetAngle < -180f) targetAngle += 360f;

        // 3. Track peak distance so rotation never regresses
        float distanceFromStart = Vector2.Distance(rectTransform.position, dragStartWorldPosition);
        maxDistanceFromStart = Mathf.Max(maxDistanceFromStart, distanceFromStart);

        float rotationFactor = maxDistanceFromStart / (totalDistanceToTarget * rotationCompletionThreshold);
        rotationFactor = Mathf.Clamp01(rotationFactor);

        // 4. Interpolate
        float originalAngle = originalRotation.eulerAngles.z;
        if (originalAngle > 180f) originalAngle -= 360f;

        float currentAngle = Mathf.Lerp(originalAngle, targetAngle, rotationFactor);
        rectTransform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsOverlappingEnough())
        {
            rectTransform.anchoredPosition = targetSlot.anchoredPosition;
            rectTransform.localRotation = originalRotation; 
            myImage.color = snappedColor;
        }
        else
        {
            StartCoroutine(ReturnHome(0.5f));
        }
    }

    private bool IsOverlappingEnough()
    {
        float distance = Vector2.Distance(rectTransform.position, targetSlot.position);
        float threshold = (targetSlot.rect.width * canvas.scaleFactor) * 0.5f;
        return distance < threshold;
    }

    IEnumerator ReturnHome(float duration)
    {
        float time = 0;
        Vector2 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.localRotation;

        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);
            rectTransform.localRotation = Quaternion.Lerp(startRot, originalRotation, t);
            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
        myImage.color = defaultColor;
    }
}