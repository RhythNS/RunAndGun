using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float focusPointStrength = 0.25f;
    [SerializeField] private float z;
    [SerializeField] private Transform toFollow;

    public Transform ToFollow
    {
        get => toFollow;
        set
        {
            enabled = value != null;
            toFollow = value;
        }
    }

    public Vector2 focusPoint = new Vector2(0.0f, 0.0f);

    private void Update()
    {
        Vector2 pos = ToFollow.position;
        Vector2 dir = focusPoint - pos;
        pos += dir * focusPointStrength;

        transform.position = new Vector3(pos.x, pos.y, z);
    }
}
