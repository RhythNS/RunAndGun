using UnityEngine;

public abstract class GameRule : ScriptableObject
{
    public abstract string Description { get; }

    public abstract void Enable();

    public abstract void Disable();
}
