using MapGenerator;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Element for the mini map.
/// </summary>
public class MiniMapManager : MonoBehaviour
{
    private class Room
    {
        public Texture2D texture;
        public DungeonRoom dungeonRoom;
        public Image image;

        public Room(Texture2D texture, DungeonRoom dungeonRoom, Image image)
        {
            this.texture = texture;
            this.dungeonRoom = dungeonRoom;
            this.image = image;
        }
    }

    public static MiniMapManager Instance { get; private set; }

    [SerializeField] private Image background;
    [SerializeField] private float zoomFactor = 2.0f;

    private Dictionary<DungeonRoom, Room> enteredRooms = new Dictionary<DungeonRoom, Room>();
    private Vector2Int roomOffset;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("MiniMapManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Player.LocalPlayer)
        {
            Vector2 position = Player.LocalPlayer.transform.position;
            position.x = (roomOffset.x - position.x) * zoomFactor;
            position.y = (roomOffset.y - position.y) * zoomFactor;
            background.transform.localPosition = position;
        }
    }

    /// <summary>
    /// Called when a new level was generated.
    /// </summary>
    public void OnNewLevelGenerated()
    {
        Dungeon dungeon = DungeonDict.Instance.dungeon;
        RectInt boundingRect = new RectInt(0,0, dungeon.Size.x, dungeon.Size.y);

        background.rectTransform.sizeDelta = new Vector2(boundingRect.width, boundingRect.height) * zoomFactor;
        roomOffset = boundingRect.size / 2;
    }

    /// <summary>
    /// Called when the level was deleted.
    /// </summary>
    public void OnLevelDeleted()
    {
        ClearTexturesAndRooms();
    }

    /// <summary>
    /// Called when entered a dungeon room.
    /// </summary>
    /// <param name="dungeonRoom">The room that was entered.</param>
    public void OnRoomEntered(DungeonRoom dungeonRoom)
    {
        if (dungeonRoom == null || enteredRooms.ContainsKey(dungeonRoom) == true)
            return;

        MiniMapNewRoomMessage newRoomMessage = new MiniMapNewRoomMessage
        {
            roomId = dungeonRoom.id
        };
        enteredRooms.Add(dungeonRoom, null);
        NetworkServer.SendToAll(newRoomMessage);
    }

    /// <summary>
    /// Callback for when a new room was entered.
    /// </summary>
    /// <param name="dungeonRoom">The room that was entered.</param>
    public void OnNewRoomEntered(DungeonRoom dungeonRoom)
    {
        Texture2D texture = CreateTexture(dungeonRoom);
        texture.filterMode = FilterMode.Point;

        GameObject roomObject = new GameObject("Room");
        roomObject.transform.parent = background.transform;

        GetMinAndMaxValues(dungeonRoom, out int minX, out int minY, out int maxX, out int maxY);
        Image image = roomObject.AddComponent<Image>();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.rectTransform.sizeDelta = new Vector2(texture.width * zoomFactor, texture.height * zoomFactor);

        Vector2 position = new Vector2(minX, minY) * zoomFactor;
        image.rectTransform.localPosition = position;

        Room room = new Room(texture, dungeonRoom, image);
        enteredRooms[dungeonRoom] = room;
    }

    /// <summary>
    /// Sets how zoomed in the map is.
    /// </summary>
    /// <param name="newZoomLevel">The new zoom level.</param>
    public void SetZoomLevel(float newZoomLevel)
    {
        background.rectTransform.sizeDelta = background.rectTransform.sizeDelta / zoomFactor * newZoomLevel;
        foreach (Room room in enteredRooms.Values)
        {
            room.image.rectTransform.sizeDelta = new Vector2(room.texture.width * newZoomLevel, room.texture.height * newZoomLevel);
            room.image.rectTransform.localPosition = room.image.rectTransform.localPosition / zoomFactor * newZoomLevel;
        }
        zoomFactor = newZoomLevel;
    }

    /// <summary>
    /// Creates a texture based on a dungeon room.
    /// </summary>
    /// <param name="dungeonRoom">The dungeon room to which a texture should be generated to.</param>
    /// <returns>The generated texture.</returns>
    private Texture2D CreateTexture(DungeonRoom dungeonRoom)
    {
        GetMinAndMaxValues(dungeonRoom, out int minX, out int minY, out int maxX, out int maxY);
        Texture2D texture = new Texture2D(maxX - minX, maxY - minY, TextureFormat.ARGB32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        for (int i = 0; i < dungeonRoom.walkableTiles.Count; i++)
        {
            Vector2Int pos = dungeonRoom.walkableTiles[i] - new Vector2Int(minX, minY);
            texture.SetPixel(pos.x, pos.y, Color.blue);
        }
        texture.Apply();
        return texture;
    }

    private void GetMinAndMaxValues(DungeonRoom dungeonRoom, out int minX, out int minY, out int maxX, out int maxY)
    {
        minX = minY = int.MaxValue;
        maxX = maxY = int.MinValue;

        for (int i = 0; i < dungeonRoom.walkableTiles.Count; i++)
        {
            Vector2Int pos = dungeonRoom.walkableTiles[i] - roomOffset;
            if (minX > pos.x)
                minX = pos.x;
            if (minY > pos.y)
                minY = pos.y;
            if (maxX < pos.x)
                maxX = pos.x;
            if (maxY < pos.y)
                maxY = pos.y;
        }
    }

    /// <summary>
    /// Clears all textures from memory and removes all rooms.
    /// </summary>
    private void ClearTexturesAndRooms()
    {
        foreach (Room rooms in enteredRooms.Values)
        {
            if (rooms == null)
                continue;

            Destroy(rooms.texture);
        }
        enteredRooms.Clear();

        for (int i = 0; i < background.transform.childCount; i++)
        {
            Destroy(background.transform.GetChild(i));
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        ClearTexturesAndRooms();
    }
}
