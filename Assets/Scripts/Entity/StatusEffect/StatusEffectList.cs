using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectList : NetworkBehaviour
{
    public SyncList<StatusEffect> StatusEffects { get; private set; } = new SyncList<StatusEffect>();
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

    [Server]
    public void ServerAdd(StatusEffect effect)
    {
        StatusEffects.Add(effect);
        coroutineForEffect.Add(effect, new ExtendedCoroutine(this, DoTick(effect), null, true));
    }

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

    [Command]
    public void CmdAdd(StatusEffect effect)
    {
        ServerAdd(effect);
    }

    [Server]
    public void Clear()
    {
        while (StatusEffects.Count > 0)
            StatusEffects.RemoveAt(0);
    }

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

    [ClientRpc]
    public void RpcOnTick(int index)
    {
        if (isServer)
            return;

        if (index < 0 || index >= StatusEffects.Count)
            throw new System.Exception("Index out of bounds. Ignoring StatusEffect! " + index + "/" + StatusEffects.Count);

        StatusEffects[index].OnTick();
    }

    private IEnumerator DoTick(StatusEffect effect)
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPerTick);
            effect.OnTick();
        }
    }
}
