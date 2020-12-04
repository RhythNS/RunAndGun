public class DeleteOnDeath : IDieable
{
    public override void Die()
    {
        Destroy(gameObject);
    }
}
