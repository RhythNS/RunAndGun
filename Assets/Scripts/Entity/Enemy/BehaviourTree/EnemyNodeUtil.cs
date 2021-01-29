public static class EnemyNodeUtil
{
    public static bool TargetAlive(Health health) => health != null && health && health.Alive;
}
