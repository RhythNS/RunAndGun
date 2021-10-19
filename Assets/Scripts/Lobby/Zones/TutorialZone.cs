﻿using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    [SerializeField] private Vector2 halfBounds;
    [SerializeField] private PlayerReviveSpawner playerReviveSpawner;
    [SerializeField] private TutorialShop shop;

    private Vector2 min, max;
    private List<PickableInWorld> activeWeapons = new List<PickableInWorld>();

    private List<Player> playersInside = new List<Player>();

    private bool active = false;

    private void Start()
    {
        Vector2 pos = transform.position;
        min = pos - halfBounds;
        max = pos + halfBounds;
    }

    public void Register()
    {
        PickableInWorld.OnSpawned += OnPickableSpawned;
        PickableInWorld.OnDeSpawned += OnPickableDeSpawned;
    }

    public void DeRegister()
    {
        PickableInWorld.OnSpawned -= OnPickableSpawned;
        PickableInWorld.OnDeSpawned -= OnPickableDeSpawned;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player) == false
            || player.IsAI == true)
            return;

        playersInside.Add(player);

        if (active == false)
            OnPlayersEntered();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player) == false
            || player.IsAI == true)
            return;

        player.ResetToDefault();
        playersInside.Remove(player);

        if (playersInside.Count == 0)
            OnAllPlayersLeft();
    }

    private void OnPlayersEntered()
    {
        active = true;
        playerReviveSpawner.Spawn();
        shop.Spawn();
    }

    private void OnAllPlayersLeft()
    {
        active = false;

        // If the server is shutting down while the player is in the tutorial zone, then
        // this causes an InvalidOperationException because the spawned list of objects
        // was modified whilst it tried to delete everything. Since we do not need to
        // delete anything when we are shutting down, we will just do nothing.
        if (ClientScene.ready == false)
            return;

        playerReviveSpawner.DeSpawn();
        shop.DeSpawn();
    }

    private void OnPickableSpawned(PickableInWorld piw)
    {
        Vector2 pos = piw.transform.position;
        if (pos.x < min.x || pos.y < min.y || pos.x > max.x || pos.y > max.y)
        {
            piw.PickedUp(false);
            piw.DespawnPickable();
            return;
        }

        if (piw.IsBuyable == false && piw.Pickable.PickableType == PickableType.Weapon)
        {
            if (activeWeapons.Count > 0)
            {
                for (int i = 0; i < activeWeapons.Count; i++)
                {
                    if (activeWeapons[i])
                        activeWeapons[i].DespawnPickable();
                }
                activeWeapons.Clear();
            }
            activeWeapons.Add(piw);
        }
    }

    private void OnPickableDeSpawned(PickableInWorld piw)
    {
        if (piw.Pickable.PickableType == PickableType.Weapon)
            activeWeapons.Remove(piw);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 size = halfBounds * 2;
        size.z = 1.0f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
