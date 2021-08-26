using UnityEngine;

/// <summary>
/// A gamerule to change specific aspects of the game.
/// </summary>
public abstract class GameRule : ScriptableObject
{
    /// <summary>
    /// What the gamerule changed.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Enables the rule.
    /// </summary>
    public abstract void Enable();

    /// <summary>
    /// Disables the rule.
    /// </summary>
    public abstract void Disable();
}
