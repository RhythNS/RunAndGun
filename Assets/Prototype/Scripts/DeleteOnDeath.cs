public class DeleteOnDeath : IDieable
{
    public override void Die()
    {
        ScoreUpdater.OnEnemyKilled();
        Destroy(gameObject);
    }
}
