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

    private void OnDrawGizmosSelected()
    {
        DrawGizmoPoint(eastHand);
        DrawGizmoPoint(westHand);
        DrawGizmoPoint(northPoint);
        DrawGizmoPoint(southPoint);
    }

    private void DrawGizmoPoint(Vector2 point)
    {
        Vector3 position = point;
        position += transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position + new Vector3(-0.125f, -0.125f), position + new Vector3(0.125f, 0.125f));
        Gizmos.DrawLine(position + new Vector3(-0.125f, 0.125f), position + new Vector3(0.125f, -0.125f));
    }
}
