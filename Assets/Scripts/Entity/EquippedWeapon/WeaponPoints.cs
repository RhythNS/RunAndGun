using UnityEngine;

public class WeaponPoints : MonoBehaviour
{
    public Vector2 EastHand => eastHand;
    [SerializeField] private Vector2 eastHand;

    public Vector2 WestHand => westHand;
    [SerializeField] private Vector2 westHand;

    public Vector2 NorthPoint => northPoint;
    [SerializeField] private Vector2 northPoint;

    public Vector2 SouthPoint => southPoint;
    [SerializeField] private Vector2 southPoint;

    public Vector2 TwoHandedMidPoint => twoHandedMidPoint;
    [SerializeField] private Vector2 twoHandedMidPoint;

    public float Radius => radius;
    [SerializeField] private float radius;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        DrawGizmoPoint(eastHand);
        DrawGizmoPoint(westHand);
        DrawGizmoPoint(northPoint);
        DrawGizmoPoint(southPoint);

        Gizmos.DrawWireSphere(transform.position + (Vector3)twoHandedMidPoint, radius);
    }

    private void DrawGizmoPoint(Vector2 point)
    {
        Vector3 position = point;
        position += transform.position;
        Gizmos.DrawLine(position + new Vector3(-0.125f, -0.125f), position + new Vector3(0.125f, 0.125f));
        Gizmos.DrawLine(position + new Vector3(-0.125f, 0.125f), position + new Vector3(0.125f, -0.125f));
    }
}
