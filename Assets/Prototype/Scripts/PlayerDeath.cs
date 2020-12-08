public class PlayerDeath : IDieable
{
    public override void Die()
    {
        Destroy(GetComponent<Health>());
        GetComponent<Player>().enabled = false;

        Respawn.Instance.gameObject.SetActive(true);
    }
}
