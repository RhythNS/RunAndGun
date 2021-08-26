/// <summary>
/// Represents a stat for tracking something.
/// </summary>
public abstract class Stat
{
    /// <summary>
    /// The name of the stat.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// The acctual value converted into a string.
    /// </summary>
    public abstract string StringValue { get; }
}
