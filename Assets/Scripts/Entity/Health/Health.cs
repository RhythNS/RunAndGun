using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    /// <summary>
    /// The max amount of hitpoints.
    /// </summary>
    [SyncVar(hook = nameof(OnMaxChanged))] private int max = 200;
    /// <summary>
    /// The current amount of hitpoints.
    /// </summary>
    [SyncVar(hook = nameof(OnCurrentChanged))] private int current = 200;
    /// <summary>
    /// The current amount of defence points.
    /// </summary>
    [SyncVar(hook = nameof(OnDefenceChanged))] private int defence = 10;

    /// <summary>
    /// The current amount defence points.
    /// </summary>
    public int Defence => defence;
    /// <summary>
    /// The max amount of hitpoints.
    /// </summary>
    public int Max => max;
    /// <summary>
    /// The current amount of hitpoints.
    /// </summary>
    public int Current => current;
    /// <summary>
    /// How much damage in total was taken.
    /// </summary>
    public int DamageTaken => max - current;
    /// <summary>
    /// Checks if entity is still alive.
    /// </summary>
    public bool Alive => current > 0;

    public EntityType EntityType { get; private set; }

    private void Awake()
    {
        EntityType = GetComponent<Entity>().EntityType;
    }

    private void OnEnable()
    {
        AliveHealthDict.Instance.Register(this);
    }

    private void OnDisable()
    {
        AliveHealthDict.Instance.DeRegister(this);
    }

    [Server]
    public void SetMax(int amount)
    {
        max = amount;
        if (current > max)
            current = max;
        // TODO: If the max goes up, this the current also go up?
    }

    [Server]
    public void SetDefence(int amount)
    {
        defence = amount;
    }

    [Server]
    public void Revive(int amount)
    {
        if (enabled)
        {
            Debug.Log("Cant revive " + gameObject.name + "! He is not dead!");
            return;
        }

        enabled = true;
        current = Mathf.Clamp(amount, 0, max);
    }

    public void Damage(int amount)
    {
        if (!enabled)
            return;

        bool isPlayer = EntityType == EntityType.Player;
        if (isPlayer && isLocalPlayer)
            CmdDamage(amount);
        else if (!isPlayer && isServer)
            ServerDamage(amount);
    }

    /// <summary>
    /// Subtracts the specified amount from the current health total. Wenn the total
    /// reaches 0 then OnDied is called.
    /// </summary>
    [Command]
    private void CmdDamage(int amount)
    {
        ServerDamage(amount);
    }

    [Server]
    private void ServerDamage(int amount)
    {
        if (!enabled)
            return;

        // TODO: Add defence to this
        current = Mathf.Clamp(current - amount, 0, max);
        if (current == 0)
        {
            Debug.Log(gameObject.name + " has died!");
            enabled = false;
            RpcOnDied();
        }
    }

    /// <summary>
    /// Called when the health points reached 0.
    /// </summary>
    [ClientRpc]
    private void RpcOnDied()
    {
        GetComponent<IDieable>().Die();
    }

    /// <summary>
    /// Called when the current health amount changed.
    /// </summary>
    /// <param name="prevHealth">The previous health amount.</param>
    /// <param name="currentHealth">The current health amount.</param>
    private void OnCurrentChanged(int prevHealth, int currentHealth)
    {
        // TODO: Maybe update UI or something
        Debug.Log(gameObject.name + " health changed from " + prevHealth + " to " + currentHealth);
    }

    private void OnMaxChanged(int prevMax, int currentMax)
    {

    }

    private void OnDefenceChanged(int prevDefence, int currentDefence)
    {

    }
}
