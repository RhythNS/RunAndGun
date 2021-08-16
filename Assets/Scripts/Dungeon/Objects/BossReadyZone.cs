﻿using Mirror;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Starts a boss fight if all players are inside this zone.
/// </summary>
public class BossReadyZone : MonoBehaviour
{
    public DungeonDoor OnDoor { get; private set; }
    private readonly List<Player> playersReady = new List<Player>();

    /// <summary>
    /// Sets all required variables.
    /// </summary>
    /// <param name="rect">The bounding rectangle of the zone.</param>
    /// <param name="onDoor">The door that is next to this zone.</param>
    public void Set(Rect rect, DungeonDoor onDoor)
    {
        OnDoor = onDoor;
        transform.position = rect.position;
        transform.localScale = rect.size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) == false)
            return;

        if (Player.LocalPlayer == null || Player.LocalPlayer.isServer)
            OnPlayerReadyToEnterChanged(player, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) == false)
            return;

        if (Player.LocalPlayer == null || Player.LocalPlayer.isServer)
            OnPlayerReadyToEnterChanged(player, false);
    }

    [Server]
    public void OnPlayerReadyToEnterChanged(Player player, bool ready)
    {
        if (!ready)
            playersReady.Remove(player);
        else
        {
            if (playersReady.Contains(player) == false)
                playersReady.Add(player);
        }

        BossRoom bossRoom = DungeonDict.Instance.BossRoom;

        if (bossRoom.CurrentState != BossRoom.State.ReadyToEnter)
            return;

        if (playersReady.Count == PlayersDict.Instance.Players.Count)
            bossRoom.OnAllPlayersReadyToEnter(this);
    }
}
