﻿using Mirror;

/// <summary>
/// Message is sent when the game is about to start.
/// </summary>
public struct StartGameMessage : NetworkMessage
{
    /// <summary>
    /// The seed of the level.
    /// </summary>
    public int levelSeed;
}
