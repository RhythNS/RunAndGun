using MapGenerator;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            position.x = roomOffset.x - position.x * 0.5f;
            position.y = roomOffset.y - position.y * 0.5f;
            background.transform.localPosition = position;
        }
    }

    public void OnNewLevelGenerated()
    {
        Dungeon dungeon = DungeonDict.Instance.dungeon;
        RectInt boundingRect = new RectInt(0,0, dungeon.Size.x, dungeon.Size.y);

        background.rectTransform.sizeDelta = new Vector2(boundingRect.width, boundingRect.height);
        roomOffset = boundingRect.size / 2;
    }

    public void OnLevelDeleted()
    {
        ClearTexturesAndRooms();
    }

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

    public void OnNewRoomEntered(DungeonRoom dungeonRoom)
    {
        Texture2D texture = CreateTexture(dungeonRoom);
        texture.filterMode = FilterMode.Point;

        GameObject roomObject = new GameObject("Room");
        roomObject.transform.parent = background.transform;

        GetMinAndMaxValues(dungeonRoom, out int minX, out int minY, out int maxX, out int maxY);
        Image image = roomObject.AddComponent<Image>();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);

        //        Vector2 position = new Vector2((maxX - minX) * 0.5f, (maxY - minY) * 0.5f) - roomOffset;
        Vector2 position = new Vector2(minX * 0.5f, minY * 0.5f) + roomOffset;
        image.rectTransform.localPosition = position;

        Room room = new Room(texture, dungeonRoom, image);
        enteredRooms[dungeonRoom] = room;
    }

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
