﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapGenerator
{
    /// <summary>
    /// Config used to initialize a Dungeon.
    /// </summary>
    [System.Serializable]
    public struct DungeonConfig
    {
        /// <summary>
        /// The seed to use for the generation process.
        /// </summary>
        public int seed;

        /// <summary>
        /// The size of the dungeon on the x-axis.
        /// </summary>
        public int sizeX;
        /// <summary>
        /// The size of the dungeon on the y-axis.
        /// </summary>
        public int sizeY;

        /// <summary>
        /// The minimum amount of rooms (excluding start-, shop- and bossroom).
        /// </summary>
        public int minRooms;
        /// <summary>
        /// The maximum amount of rooms (excluding start-, shop- and bossroom).
        /// </summary>
        public int maxRooms;

        /// <summary>
        /// The minimum length of a corridor.
        /// </summary>
        public int corridorMinLength;
        /// <summary>
        /// The maximum length of a corridor.
        /// </summary>
        public int corridorMaxLength;

        public Pickable[] itemsToSpawn;

        /// <summary>
        /// Creates a new DungeonConfig.
        /// </summary>
        /// <param name="seed">The seed to use for the generation process.</param>
        /// <param name="sizeX">The size of the dungeon on the x-axis.</param>
        /// <param name="sizeY">The size of the dungeon on the y-axis.</param>
        /// <param name="minRooms">The minimum amount of rooms (excluding start-, shop- and bossroom).</param>
        /// <param name="maxRooms">The maximum amount of rooms (excluding start-, shop- and bossroom).</param>
        /// <param name="corridorMinLength">The minimum length of a corridor.</param>
        /// <param name="corridorMaxLength">The maximum length of a corridor.</param>
        public DungeonConfig(int seed, int sizeX, int sizeY, int minRooms, int maxRooms, int corridorMinLength, int corridorMaxLength, Pickable[] itemsToSpawn) {
            this.seed = seed;

            this.sizeX = sizeX;
            this.sizeY = sizeY;

            this.minRooms = minRooms;
            this.maxRooms = maxRooms;

            this.corridorMinLength = Mathf.Clamp(corridorMinLength, Corridor.MIN_LENGTH, Corridor.MAX_LENGTH);
            this.corridorMaxLength = Mathf.Clamp(corridorMaxLength, Corridor.MIN_LENGTH, Corridor.MAX_LENGTH);

            this.itemsToSpawn = itemsToSpawn;
        }
    }
}