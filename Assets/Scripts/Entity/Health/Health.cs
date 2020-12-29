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

    [Server]
    public void SetMax(int amount)
    {
        max = amount;
    }

    public void Damage(int amount)
    {
        bool isPlayer = gameObject.layer == 26; // TODO: Replace with player layer
        if ((isPlayer && isLocalPlayer) || (!isPlayer && isServer))
            CmdDamage(amount);
    }

    /// <summary>
    /// Subtracts the specified amount from the current health total. Wenn the total
    /// reaches 0 then OnDied is called.
    /// </summary>
    [Command]
    private void CmdDamage(int amount)
    {
        current = Mathf.Clamp(current - amount, 0, max);
        if (current == 0)
            RpcOnDied();
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
        Debug.Log("HEALTH CHANGED FROM " + prevHealth + " to " + currentHealth);
    }

    private void OnMaxChanged(int prevMax, int currentMax)
    {

    }
}
