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

        /// <summary>
        /// If this floor should have a ShopRoom.
        /// </summary>
        public bool generateShopRoom;

        /// <summary>
        /// The items that can be bought in the shop if there is one.
        /// </summary>
        public Pickable[] shopItems;


        /// <summary>
        /// Creates a new DungeonConfig.
        /// </summary>
        /// <param name="sizeX">The size of the dungeon on the x-axis.</param>
        /// <param name="sizeY">The size of the dungeon on the y-axis.</param>
        /// <param name="minRooms">The minimum amount of rooms (excluding start-, shop- and bossroom).</param>
        /// <param name="maxRooms">The maximum amount of rooms (excluding start-, shop- and bossroom).</param>
        /// <param name="corridorMinLength">The minimum length of a corridor.</param>
        /// <param name="corridorMaxLength">The maximum length of a corridor.</param>
        /// <param name="generateShopRoom">If this floor should have a ShopRoom.</param>
        /// <param name="itemsToSpawn">The items to spawn at the spawn point.</param>
        public DungeonConfig(int sizeX, int sizeY, int minRooms, int maxRooms, int corridorMinLength, int corridorMaxLength, bool generateShopRoom, Pickable[] itemsToSpawn, Pickable[] shopItems)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            this.minRooms = minRooms;
            this.maxRooms = maxRooms;

            this.corridorMinLength = Mathf.Clamp(corridorMinLength, Corridor.MIN_LENGTH, Corridor.MAX_LENGTH);
            this.corridorMaxLength = Mathf.Clamp(corridorMaxLength, Corridor.MIN_LENGTH, Corridor.MAX_LENGTH);

            this.generateShopRoom = generateShopRoom;

            this.shopItems = shopItems;
        }

        public static DungeonConfig StandardConfig => new DungeonConfig {
            sizeX = 192,
            sizeY = 192,
            minRooms = 10,
            maxRooms = 20,
            corridorMinLength = Corridor.MIN_LENGTH,
            corridorMaxLength = Corridor.MAX_LENGTH,
            generateShopRoom = true,
        };
    }
}
