using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the object that places world effects onto health targets.
/// </summary>
public class WorldEffectInWorld : NetworkBehaviour
{
    public Health Inflicter { get; private set; }

    private SyncList<WorldEffect> effects = new SyncList<WorldEffect>();
    [SyncVar] private GameObject inflicterObj;

    private Dictionary<Health, ExtendedCoroutine> coroutineForHealth = new Dictionary<Health, ExtendedCoroutine>();
    private bool hasTickables;
    private static readonly float secondsPerTick = 1.0f;

    [Server]
    public static WorldEffectInWorld Place(WorldEffectInWorld prefab, WorldEffect[] effects, Health inflicter, Vector3 position, Quaternion quaternion)
    {
        WorldEffectInWorld toSpawn = Instantiate(prefab, position, quaternion);
        toSpawn.Init(inflicter, effects);
        NetworkServer.Spawn(toSpawn.gameObject);
        return toSpawn;
    }

    /// <summary>
    /// Inits all fields.
    /// </summary>
    /// <param name="inflicter">The Health that started this worldeffect. Can be null.</param>
    /// <param name="effects">The world effects.</param>
    [Server]
    public void Init(Health inflicter, WorldEffect[] effects)
    {
        Inflicter = inflicter;
        inflicterObj = inflicter != null ? inflicter.gameObject : null;

        this.effects = new SyncList<WorldEffect>();
        for (int i = 0; i < effects.Length; i++)
        {
            this.effects.Add(Instantiate(effects[i]));
            if (this.effects[i].Type == WorldEffect.TriggerType.EveryTick)
                hasTickables = true;
        }
    }

    public override void OnStartClient()
    {
        if (isServer == true)
            return;

        if (inflicterObj != null)
            Inflicter = inflicterObj.GetComponent<Health>();

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].Type == WorldEffect.TriggerType.EveryTick)
            {
                hasTickables = true;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Health target) == false || target.Alive == false)
            return;

        if (collision.TryGetComponent(out Player player) && player.IsAuthorityResponsible == false)
            return;

        bool healthWasAlreadyOn = coroutineForHealth.ContainsKey(target);

        // Add an entry for the health. If this WorldEffectInWorld has tickable effects then start the coroutine
        // for that target.
        coroutineForHealth[target] = hasTickables ? new ExtendedCoroutine(target, DoTick(target), startNow: true) : null;

        for (int i = 0; i < effects.Count; i++)
        {
            switch (effects[i].Type)
            {
                case WorldEffect.TriggerType.Once:
                    if (healthWasAlreadyOn == false)
                        effects[i].OnEnter(target, this);
                    break;

                case WorldEffect.TriggerType.EveryReEnter:
                case WorldEffect.TriggerType.EveryTick:
                    effects[i].OnEnter(target, this);
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Health target) == false)
            return;

        if (hasTickables)
            coroutineForHealth[target].Stop(false);
    }

    private IEnumerator DoTick(Health target)
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPerTick);
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].Type == WorldEffect.TriggerType.EveryTick)
                    effects[i].OnTick(target, this);
            }
        }
    }
}
