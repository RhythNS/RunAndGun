/// <summary>
/// Stat for tracking an integer.
/// </summary>
public abstract class IntStat : Stat
{
    /// <summary>
    /// The integer value.
    /// </summary>
    public int Value { get; protected set; }

    public override string StringValue => Value.ToString();

    /// <summary>
    /// Adds an integer to the value.
    /// </summary>
    public void Add(int toAdd) => Value += toAdd;
}

/// <summary>
/// Tracks kills.
/// </summary>
public class KillStat : IntStat
{
    public override string Name { get => "Kills"; }
}

/// <summary>
/// Tracks how many times bullets were spawned.
/// </summary>
public class ShotsFiredStat : IntStat
{
    public override string Name => "Shots fired";
}

/// <summary>
/// Tracks how many bullets hit.
/// </summary>
public class ShotsHitStat : IntStat
{
    public override string Name => "Shots hit";
}

/// <summary>
/// Tracks how many times the entity died.
/// </summary>
public class TimesDied : IntStat
{
    public override string Name => "Times died";
}

/// <summary>
/// Tracks how many other players the player revived.
/// </summary>
public class OtherPlayerRevived : IntStat
{
    public override string Name => "Players revived";
}

/// <summary>
/// Tracks how much damage the entity inflicted to others.
/// </summary>
public class DamageInflicted : IntStat
{
    public override string Name => "Damage inflicted";
}
