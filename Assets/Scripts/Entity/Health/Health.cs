using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SyncVar] private int max;
    [SyncVar(hook = nameof(OnCurrentChanged))] private int current;

    public int Max => max;
    public int Current => current;
    public int DamageTaken => max - current;

    [Command]
    public void CmdDamage(int amount)
    {
        current = Mathf.Clamp(current - amount, 0, max);
        if (current == 0)
            RpcOnDied();
    }

    [ClientRpc]
    private void RpcOnDied()
    {
        GetComponent<IDieable>().Die();
    }

    private void OnCurrentChanged(int prevHealth, int currentHealth)
    {
        // TODO: Maybe update UI or something
    }
}
