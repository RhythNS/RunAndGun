using UnityEngine;

/// <summary>
/// Represents an object that can be broken ingame.
/// </summary>
public class BreakableObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D bc;

    public int index;

    void Start()
    {
        PositionConverter.AdjustZ(transform);

        sr.sprite = BreakablesDict.Instance.GetBreakable(index).full;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bc.enabled = false;
        sr.sprite = BreakablesDict.Instance.GetBreakable(index).broken;
    }
}
