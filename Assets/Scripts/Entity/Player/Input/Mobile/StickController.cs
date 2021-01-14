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
        Rect rect = rectTransform.rect;
        Vector2 circleCenter = new Vector2(rectTransform.position.x + rect.width * 0.5f, rectTransform.position.y + rect.height * 0.5f);
        float circleRadius = rect.width / 2;

        Vector2 v = eventData.position - circleCenter;
        v = MathUtil.ClampMagnitude(v, circleRadius, out float magnitude);
        Vector2 newLocation = circleCenter + v;
        magnitude /= circleRadius;

        if (magnitude > deadzone || magnitude < -deadzone)
            Output = v / circleRadius;
        else
            Output = new Vector2();

        innerStick.position = newLocation;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Down = false;
        Rect rect = rectTransform.rect;
        innerStick.transform.position = new Vector3(rectTransform.position.x + rect.width * 0.5f, rectTransform.position.y + rect.height * 0.5f);
        Output = new Vector2();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Down = true;
    }
}
