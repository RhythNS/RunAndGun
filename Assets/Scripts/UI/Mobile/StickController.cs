using UnityEngine;
using UnityEngine.EventSystems;

public class StickController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool Down { get; private set; }

    [SerializeField] private float deadzone = 0.1f;
    [SerializeField] private RectTransform innerStick;
    private RectTransform rectTransform;

    public Vector2 Output { get; private set; }

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 circleSize = rectTransform.lossyScale * rectTransform.rect.size;
        Vector2 circleCenter = (Vector2)rectTransform.transform.position + (circleSize * 0.5f);
        float circleRadius = rectTransform.rect.width * 0.5f;

        Vector2 v = eventData.position - circleCenter;
        v = MathUtil.ClampMagnitude(v, circleRadius, out float magnitude);
        magnitude /= circleRadius;

        if (magnitude > deadzone || magnitude < -deadzone)
            Output = v / circleRadius;
        else
            Output = new Vector2();

        innerStick.anchoredPosition = v;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Down = false;
        innerStick.anchoredPosition = new Vector2();
        Output = new Vector2();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Down = true;
    }
}
