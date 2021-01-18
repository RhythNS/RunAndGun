using UnityEngine;

public class MobileUIManager : MonoBehaviour
{
    public StickController Move => move;
    [SerializeField] private StickController move;
    public StickController Aim => aim;
    [SerializeField] private StickController aim;



}
