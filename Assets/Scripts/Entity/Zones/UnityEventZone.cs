using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Zone that raises unity events.
/// </summary>
public class UnityEventZone : EnterZone
{
    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

    public override void OnEnter(Player player)
    {
        onEnter.Invoke();
    }

    public override void OnExit(Player player)
    {
        onExit.Invoke();
    }
}
