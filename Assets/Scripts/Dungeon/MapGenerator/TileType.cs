namespace MapGenerator
{
    /// <summary>
    /// The different types of tiles a floor tilemap can pick from.
    /// </summary>
    public enum TileType {
        /// <summary>
        /// Wall tiles.
        /// </summary>
        Wall,

        /// <summary>
        /// Floor tiles.
        /// </summary>
        Floor,

        /// <summary>
        /// Tiles that can generate a corridor.
        /// </summary>
        CorridorAccess,

        /// <summary>
        /// Placeholder type for empty tiles.
        /// </summary>
        PlaceHolder
    }
}
