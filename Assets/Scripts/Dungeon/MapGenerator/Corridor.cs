using System.Collections.Generic;

using UnityEngine;

namespace MapGenerator
{
    public struct Corridor
    {
        public const int MIN_LENGTH = 10;
        public const int MAX_LENGTH = 32;

        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public Dictionary<Vector2Int, Direction> exitDirections;

        /// <summary>
        /// Places a new corridor from the given position with the length in the given direction.
        /// </summary>
        /// <param name="posX">The x-Position where the corridor should start.</param>
        /// <param name="posY">The y-Position where the corridor should start.</param>
        /// <param name="length">The length of the corridor.</param>
        /// <param name="direction">The direction where the corridor should point to.</param>
        public Corridor(int posX, int posY, int length, Direction direction) {
            Position = Vector2Int.zero;
            Size = Vector2Int.zero;
            exitDirections = new Dictionary<Vector2Int, Direction>();

            switch (direction) {
                case Direction.Up:
                    Size = new Vector2Int(2, length);
                    Position = new Vector2Int(posX, posY);

                    for (int y = 5; y < Size.y - 8; y++) {
                        exitDirections.Add(new Vector2Int(-1, y), Direction.Left);
                        exitDirections.Add(new Vector2Int(Size.x, y), Direction.Right);
                    }

                    exitDirections.Add(new Vector2Int(0, Size.y), Direction.Up);
                    break;

                case Direction.Down:
                    Size = new Vector2Int(2, length);
                    Position = new Vector2Int(posX, posY - Size.y + 1);

                    for (int y = 5; y < Size.y - 8; y++) {
                        exitDirections.Add(new Vector2Int(-1, y), Direction.Left);
                        exitDirections.Add(new Vector2Int(Size.x, y), Direction.Right);
                    }

                    exitDirections.Add(new Vector2Int(0, -1), Direction.Down);
                    break;

                case Direction.Left:
                    Size = new Vector2Int(length, 3);
                    Position = new Vector2Int(posX - Size.x + 1, posY);

                    for (int x = 5; x < Size.x - 7; x++) {
                        exitDirections.Add(new Vector2Int(x, Size.y), Direction.Up);
                        exitDirections.Add(new Vector2Int(x, -1), Direction.Down);
                    }

                    exitDirections.Add(new Vector2Int(-1, 0), Direction.Left);
                    break;

                case Direction.Right:
                    Size = new Vector2Int(length, 3);
                    Position = new Vector2Int(posX, posY);

                    for (int x = 5; x < Size.x - 7; x++) {
                        exitDirections.Add(new Vector2Int(x, -1), Direction.Down);
                        exitDirections.Add(new Vector2Int(x, Size.y), Direction.Up);
                    }

                    exitDirections.Add(new Vector2Int(Size.x, 0), Direction.Right);
                    break;
            }
        }
    }
}
