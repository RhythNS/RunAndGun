using UnityEngine;

public struct Structure
{
    public Vector2Int Position { get; set; }
    public Vector2Int Size { get; set; }

    public bool IsRoom { get; set; }

    public Structure(int x, int y, int sizeX, int sizeY, bool isRoom) {
        Position = new Vector2Int(x, y);
        Size = new Vector2Int(sizeX, sizeY);
        IsRoom = isRoom;
    }
}
