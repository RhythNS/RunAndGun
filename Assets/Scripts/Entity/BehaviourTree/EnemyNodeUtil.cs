/// <summary>
/// Collection of helper methods for Behaviour Tree nodes.
/// </summary>
public static class EnemyNodeUtil
{
    /// <summary>
    /// Checks if a target is still alive. Also performs null checks.
    /// </summary>
    public static bool TargetAlive(Health health) => health != null && health && health.Alive;
}
