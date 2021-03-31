using UnityEngine;
using UnityEngine.UI;

public class MobileUIManager : MonoBehaviour
{
    public StickController Move => move;
    [SerializeField] private StickController move;
    public StickController Aim => aim;
    [SerializeField] private StickController aim;

    public Button PickUpButton => pickupButton;
    [SerializeField] private Button pickupButton;

    public Button ReviveButton => reviveButton;
    [SerializeField] private Button reviveButton;

    public Button DashButton => dashButton;
    [SerializeField] private Button dashButton;
}
