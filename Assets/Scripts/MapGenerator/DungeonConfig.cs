using System;
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
    public struct DungeonConfig
    {
        public Vector2Int maxSize;

        public int seed;

        public int minRooms;
        public int maxRooms;

        public int corridorMinLength;
        public int corridorMaxLength;


    }
}
