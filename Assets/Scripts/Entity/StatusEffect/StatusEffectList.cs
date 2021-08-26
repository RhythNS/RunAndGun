using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages status effects on an entity.
/// </summary>
public class StatusEffectList : NetworkBehaviour
{
    /// <summary>
    /// List of status effects that is synced between each entity and server.
    /// </summary>
    public SyncList<StatusEffect> StatusEffects { get; private set; } = new SyncList<StatusEffect>();
    /// <summary>
    /// The health of the entity that the list is on.
    /// </summary>
    public Health Health { get; private set; }

    private Dictionary<StatusEffect, ExtendedCoroutine> coroutineForEffect = new Dictionary<StatusEffect, ExtendedCoroutine>();

    private static readonly float secondsPerTick = 1.0f;

    private void Awake()
    {
        Health = GetComponent<Health>();
    }

    private void Start()
    {
        StatusEffects.Callback += OnStatusEffectChanged;
    }

    /// <summary>
    /// Adds a status effect to the list. Should only be called from the server.
    /// </summary>
    /// <param name="effect">The effect to be added.</param>
    /// <param name="inflicter">The health that inflicted the status effect. Can be null.</param>
    [Server]
    public void ServerAdd(StatusEffect effect, Health inflicter)
    {
        effect.Inflicter = inflicter;

        if (effect.IsInstant)
        {
            effect.OnList = this;
            effect.OnPickup();
            effect.OnDrop();
            RpcInstantStatusEffect(effect);
            return;
        }

        // Iterate over each effect to see if an effect of the same type is already in the list.
        for (int i = 0; i < StatusEffects.Count; i++)
        {
            if (StatusEffects[i].GetType() == effect.GetType())
            {
                StatusEffects[i].OnEffectAlreadyInList(effect);
                RpcOnEffectAlreadyInList(i, effect);
                return;
            }
        }

        StatusEffects.Add(effect);
        coroutineForEffect.Add(effect, new ExtendedCoroutine(this, DoTick(effect), null, true));
    }

    /// <summary>
    /// Removes a status effect from the list.
    /// </summary>
    /// <param name="effect">The effect to be removed.</param>
    [Server]
    public void ServerRemove(StatusEffect effect)
    {
        StatusEffects.Remove(effect);
        if (coroutineForEffect.TryGetValue(effect, out ExtendedCoroutine coroutine))
        {
            coroutine.Stop();
            coroutineForEffect.Remove(effect);
        }
    }

    /// <summary>
    /// Adds a status effect to the list.
    /// </summary>
    /// <param name="effect">The effect to be added.</param>
    /// <param name="inflicter">The health that inflicted the status effect. Can be null.</param>
    public void Add(StatusEffect effect, Health inflicter)
    {
        if (isServer)
            ServerAdd(effect, inflicter);
        else
            CmdAdd(effect, inflicter);
    }

    /// <summary>
    /// Issues a command to the server to add a status effect to the list.
    /// </summary>
    /// <param name="effect">The effect to be added.</param>
    /// <param name="inflicter">The health that inflicted the status effect. Can be null.</param>
    [Command]
    public void CmdAdd(StatusEffect effect, Health inflicter)
    {
        ServerAdd(effect, inflicter);
    }

    /// <summary>
    /// Clears all status effects. Should only be called from the server.
    /// </summary>
    [Server]
    public void Clear()
    {
        while (StatusEffects.Count > 0)
            StatusEffects.RemoveAt(0);
    }

    /// <summary>
    /// Called on the client when an instant status effect was picked up.
    /// </summary>
    /// <param name="effect">The effect that was picked up.</param>
    [ClientRpc]
    public void RpcInstantStatusEffect(StatusEffect effect)
    {
        if (isServer)
            return;

        effect.OnList = this;
        effect.OnPickup();
        effect.OnDrop();
    }

    /// <summary>
    /// Called on the client when a similar effect was already on the list.
    /// </summary>
    /// <param name="index">The index of the status effect that was already in the list.</param>
    /// <param name="other">The effect that was trying to be added.</param>
    [ClientRpc]
    public void RpcOnEffectAlreadyInList(int index, StatusEffect other)
    {
        if (isServer)
            return;

        other.OnList = this;
        StatusEffects[index].OnEffectAlreadyInList(other);
    }

    /// <summary>
    /// Called when the status effect list changed in any way.
    /// </summary>
    /// <param name="op">In what way the status effect changed.</param>
    /// <param name="itemIndex">The index of the item affected. Only works in certain operations.</param>
    /// <param name="oldItem">The old status effect. Only works in certain operations.</param>
    /// <param name="newItem">The new status effect. Only works in certain operations.</param>
    private void OnStatusEffectChanged(SyncList<StatusEffect>.Operation op, int itemIndex, StatusEffect oldItem, StatusEffect newItem)
    {
        if (newItem)
            newItem.OnList = this;

        switch (op)
        {
            case SyncList<StatusEffect>.Operation.OP_ADD:
            case SyncList<StatusEffect>.Operation.OP_INSERT:
                newItem.OnPickup();
                break;
            case SyncList<StatusEffect>.Operation.OP_REMOVEAT:
                oldItem.OnDrop();
                break;
            case SyncList<StatusEffect>.Operation.OP_SET:
                oldItem.OnDrop();
                newItem.OnPickup();
                break;
            case SyncList<StatusEffect>.Operation.OP_CLEAR:
                Debug.LogWarning("Clear was called on SyncList. Please use StatusEffectList.Clear()!");
                coroutineForEffect.Clear();
                StopAllCoroutines();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Called in the client when a status effect ticked.
    /// </summary>
    /// <param name="index">The index of the status effect that ticked.</param>
    [ClientRpc]
    public void RpcOnTick(int index)
    {
        if (isServer)
            return;

        if (index < 0 || index >= StatusEffects.Count)
            throw new System.Exception("Index out of bounds. Ignoring StatusEffect! " + index + "/" + StatusEffects.Count);

        StatusEffects[index].OnTick();
    }

    /// <summary>
    /// Coroutine for handling the tick on status effect.
    /// </summary>
    /// <param name="effect">The effect to tick.</param>
    private IEnumerator DoTick(StatusEffect effect)
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPerTick);
            effect.OnTick();
        }
    }
}
