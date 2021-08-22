using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the object that places world effects onto health targets.
/// </summary>
public class WorldEffectInWorld : MonoBehaviour
{
    public Health Inflicter { get; private set; }

    private Dictionary<Health, ExtendedCoroutine> coroutineForHealth = new Dictionary<Health, ExtendedCoroutine>();
    private WorldEffect[] effects;
    private bool hasTickables;

    private static readonly float secondsPerTick = 1.0f;

    /// <summary>
    /// Inits all fields.
    /// </summary>
    /// <param name="inflicter">The Health that started this worldeffect. Can be null.</param>
    /// <param name="effects">The world effects.</param>
    public void Init(Health inflicter, WorldEffect[] effects)
    {
        Inflicter = inflicter;

        this.effects = new WorldEffect[effects.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            this.effects[i] = Instantiate(effects[i]);
            if (this.effects[i].Type == WorldEffect.TriggerType.EveryTick)
                hasTickables = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Health target) == false || target.Alive == false)
            return;

        // If this is a player and the player is not the local player then return.
        // This is because this should be client authoritive.
        if (collision.TryGetComponent(out Player player) && player.isLocalPlayer == false)
            return;

        bool healthWasAlreadyOn = coroutineForHealth.ContainsKey(target);

        // Add an entry for the health. If this WorldEffectInWorld has tickable effects then start the coroutine
        // for that target.
        coroutineForHealth[target] = hasTickables ? new ExtendedCoroutine(target, DoTick(target), startNow: true) : null;

        for (int i = 0; i < effects.Length; i++)
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
            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i].Type == WorldEffect.TriggerType.EveryTick)
                    effects[i].OnTick(target, this);
            }
        }
    }
}
