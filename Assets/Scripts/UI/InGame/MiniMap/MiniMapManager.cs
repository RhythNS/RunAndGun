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
        public Image background;

        public Room(Texture2D texture, DungeonRoom dungeonRoom, Image image, Image background)
        {
            this.texture = texture;
            this.dungeonRoom = dungeonRoom;
            this.image = image;
            this.background = background;
        }
    }

    public static MiniMapManager Instance { get; private set; }

    [SerializeField] private Image floorPlan;
    [SerializeField] private Image background;
    [SerializeField] private float zoomFactor = 2.0f;

    [SerializeField] private Color floorColor = Color.blue;
    [SerializeField] private Color wallColor = Color.gray;

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

        //gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Player.LocalPlayer)
        {
            Vector2 position = Player.LocalPlayer.transform.position + new Vector3(roomOffset.x, roomOffset.y, 0f);
            position.x = (roomOffset.x - position.x) * zoomFactor;
            position.y = (roomOffset.y - position.y) * zoomFactor;
            floorPlan.transform.localPosition = position;
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

        floorPlan.rectTransform.sizeDelta = new Vector2(boundingRect.width, boundingRect.height);// * zoomFactor;
        background.rectTransform.sizeDelta = new Vector2(boundingRect.width, boundingRect.height);// * zoomFactor;
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
        GameObject roomObject = new GameObject("Room_" + dungeonRoom.RoomType.ToString());
        roomObject.transform.parent = floorPlan.transform;
        GameObject roomBgObject = new GameObject("Room_" + dungeonRoom.RoomType.ToString());
        roomBgObject.transform.parent = background.transform;

        GetMinAndMaxValues(dungeonRoom, out int minX, out int minY, out int maxX, out int maxY);

        // create room layout
        Texture2D texture = CreateTextureFromRoom(dungeonRoom);
        texture.filterMode = FilterMode.Point;

        Image image = roomObject.AddComponent<Image>();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f));
        image.rectTransform.sizeDelta = new Vector2(texture.width, texture.height) * zoomFactor;
        image.rectTransform.pivot = new Vector2(0f, 0f);
        image.rectTransform.localPosition = new Vector2(minX, minY) * zoomFactor;
        image.rectTransform.localScale = Vector3.one;

        // create room background / border
        Texture2D bgTexture = CreateBackgroundTextureForRoom();
        bgTexture.filterMode = FilterMode.Point;

        Image bg = roomBgObject.AddComponent<Image>();
        bg.sprite = Sprite.Create(bgTexture, new Rect(0, 0, 4, 4), new Vector2(0f, 0f));
        bg.color = wallColor;
        bg.rectTransform.sizeDelta = new Vector2(texture.width + 2f, texture.height + 2f) * zoomFactor;
        bg.rectTransform.pivot = new Vector2(0f, 0f);
        bg.rectTransform.localPosition = new Vector2(minX - 1f, minY - 1f) * zoomFactor;
        bg.rectTransform.localScale = Vector3.one;

        Room room = new Room(texture, dungeonRoom, image, bg);
        enteredRooms[dungeonRoom] = room;
    }

    /// <summary>
    /// Sets how zoomed in the map is.
    /// </summary>
    /// <param name="newZoomLevel">The new zoom level.</param>
    public void SetZoomLevel(float newZoomLevel)
    {
        floorPlan.rectTransform.sizeDelta = floorPlan.rectTransform.sizeDelta / zoomFactor * newZoomLevel;
        background.rectTransform.sizeDelta = background.rectTransform.sizeDelta / zoomFactor * newZoomLevel;
        foreach (Room room in enteredRooms.Values)
        {
            room.image.rectTransform.sizeDelta = new Vector2(room.texture.width, room.texture.height) * newZoomLevel;
            room.image.rectTransform.localPosition = room.image.rectTransform.localPosition / zoomFactor * newZoomLevel;

            room.background.rectTransform.sizeDelta = new Vector2(room.texture.width, room.texture.height) * newZoomLevel;
            room.background.rectTransform.localPosition = room.background.rectTransform.localPosition / zoomFactor * newZoomLevel;
        }
        zoomFactor = newZoomLevel;
    }

    /// <summary>
    /// Creates a texture based on a dungeon room.
    /// </summary>
    /// <param name="dungeonRoom">The dungeon room to which a texture should be generated to.</param>
    /// <returns>The generated texture.</returns>
    private Texture2D CreateTextureFromRoom(DungeonRoom dungeonRoom)
    {
        GetMinAndMaxValues(dungeonRoom, out int minX, out int minY, out int maxX, out int maxY);

        Texture2D texture = new Texture2D(maxX - minX + 1, maxY - minY + 1, TextureFormat.ARGB32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.filterMode = FilterMode.Point;

        Color32[] cols = new Color32[texture.width * texture.height];
        for (int i = 0; i < cols.Length; i++)
            cols[i] = Color.clear;
        texture.SetPixels32(cols);

        for (int i = 0; i < dungeonRoom.walkableTiles.Count; i++)
        {
            Vector2Int pos = dungeonRoom.walkableTiles[i] - new Vector2Int(minX, minY);
            texture.SetPixel(pos.x, pos.y, floorColor);
        }
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Creates a texture based as a background to a dungeon room.
    /// </summary>
    /// <param name="dungeonRoom"></param>
    /// <returns>The generated texture.</returns>
    private Texture2D CreateBackgroundTextureForRoom()
    {
        Texture2D texture = Texture2D.whiteTexture;
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.filterMode = FilterMode.Point;

        // somehow causes the Unity Editor to change colors?!
        //Color32[] cols = new Color32[16];
        //for (int i = 0; i < cols.Length; i++)
        //    cols[i] = wallColor;
        //texture.SetPixels32(cols);
        //texture.Apply();

        return texture;
    }

    private void GetMinAndMaxValues(DungeonRoom dungeonRoom, out int minX, out int minY, out int maxX, out int maxY)
    {
        minX = minY = int.MaxValue;
        maxX = maxY = int.MinValue;

        for (int i = 0; i < dungeonRoom.walkableTiles.Count; i++)
        {
            Vector2Int pos = dungeonRoom.walkableTiles[i];
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

        for (int i = 0; i < floorPlan.transform.childCount; i++)
            Destroy(floorPlan.transform.GetChild(i));

        for (int i = 0; i < background.transform.childCount; i++)
            Destroy(background.transform.GetChild(i));
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        ClearTexturesAndRooms();
    }
}
