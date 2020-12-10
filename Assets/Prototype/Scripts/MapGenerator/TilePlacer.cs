using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacer : MonoBehaviour
{
    [System.Serializable]
    public struct RoomTileset
    {
        // 012
        // 345
        // 678
        public Tile[] tiles;
    }

    [SerializeField] private RoomTileset[] roomTilesets;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private Weapon startWeapon;
    [SerializeField] private Weapon[] weaponDrops;
    [SerializeField] private InstantPickup[] pickups;

    [SerializeField] private Stairs stairs;


    [SerializeField] private EnemySpawner.WeaponCanMove[] weapons;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Enemy enemyPrefab;


    private GameObject parent;

    public void Fill(Dungeon dungeon, Tilemap tileMap, Map.Tile floor)
    {
        if (parent || (parent = GameObject.Find("Parent")))
            DestroyImmediate(parent);
        parent = new GameObject("Parent");

        Structure startRoom = new Structure(), endRoom = new Structure();
        for (int x = 0; x < dungeon.SizeX; x++)
        {
            for (int y = 0; y < dungeon.SizeY; y++)
            {
                if (dungeon[x, y] == Map.Tile.Floor)
                {
                    foreach (var room in dungeon.Rooms)
                    {
                        if (x >= room.Position.x && x < room.Position.x + room.Size.x &&
                            y >= room.Position.y && y < room.Position.y + room.Size.y)
                        {
                            startRoom = room;
                            goto FoundStart;
                        }
                    }
                }
            }
        }
    FoundStart:

        Player player = Instantiate(playerPrefab, tileMap.CellToWorld(
            new Vector3Int(startRoom.Position.x + startRoom.Size.x / 2,
            startRoom.Position.y + startRoom.Size.y / 2, 0)),
            Quaternion.identity);
        player.transform.parent = parent.transform;

        GameObject weaponDrop = new GameObject("StartingWeapon");
        weaponDrop.AddComponent<ReplaceableWeapon>().weapon = startWeapon;
        weaponDrop.transform.position = tileMap.CellToWorld(
            GetRandomPosFromRoom(startRoom)
            );
        weaponDrop.transform.parent = parent.transform;

        for (int x = dungeon.SizeX - 1; x >= 0; x--)
        {
            for (int y = dungeon.SizeY - 1; y >= 0; y--)
            {
                if (dungeon[x, y] == Map.Tile.Floor)
                {
                    foreach (var room in dungeon.Rooms)
                    {
                        if (x >= room.Position.x && x < room.Position.x + room.Size.x &&
                            y >= room.Position.y && y < room.Position.y + room.Size.y)
                        {
                            endRoom = room;
                            goto FoundEnd;
                        }
                    }
                }
            }
        }
    FoundEnd:

        Stairs endStairs = Instantiate(stairs, tileMap.CellToWorld(
            GetRandomPosFromRoom(endRoom)), Quaternion.identity);
        endStairs.transform.parent = parent.transform;


        for (int x = 0; x < dungeon.SizeX; x++)
        {
            for (int y = 0; y < dungeon.SizeY; y++)
            {
                if (dungeon[x, y] == floor)
                {
                    tileMap.SetTile(new Vector3Int(x, y, 0), roomTilesets[0].tiles[4]);
                }
            }
        }

        foreach (var room in dungeon.Rooms)
        {
            RoomTileset tileset;
            int roomSize = room.Size.x * room.Size.y;

            if (room.Position == startRoom.Position || room.Position == endRoom.Position)
                tileset = roomTilesets[0];
            else if (roomSize < 17)
            {
                tileset = roomTilesets[1];
                //Vector3 dropPoint = tileMap.CellToWorld(GetRandomPosFromRoom(room));
                Vector3 dropPoint = tileMap.CellToWorld(new Vector3Int(room.Position.x + room.Size.x / 2, room.Position.y + room.Size.y / 2, 0));

                GameObject dropObj;
                if (Random.value < 0.5f)
                {
                    dropObj = new GameObject("Weapon Drop");
                    dropObj.AddComponent<ReplaceableWeapon>().weapon = weaponDrops[Random.Range(0, weaponDrops.Length)];
                }
                else
                {
                    dropObj = Instantiate(pickups[Random.Range(0, pickups.Length)]).gameObject;
                }
                dropObj.transform.position = dropPoint;
                dropObj.transform.parent = parent.transform;
            }
            else // room size is big
            {
                tileset = roomTilesets[2];
                int enemyAmount = Mathf.Max(1, roomSize / 20);
                for (int i = 0; i < enemyAmount; i++)
                {
                    EnemySpawner.WeaponCanMove wcm = weapons[Random.Range(0, weapons.Length)];

                    Enemy enemy = Instantiate(enemyPrefab, tileMap.CellToWorld(GetRandomPosFromRoom(room)), Quaternion.identity);
                    enemy.Set(wcm.weapon, player, moveSpeed, wcm.canMove);
                    enemy.transform.parent = parent.transform;
                }
            }

            for (int x = room.Position.x; x < room.Position.x + room.Size.x; x++)
            {
                for (int y = room.Position.y; y < room.Position.y + room.Size.y; y++)
                {
                    tileMap.SetTile(new Vector3Int(x, y, 0), tileset.tiles[4]);
                }
            }

            for (int i = room.Position.x + 1; i < room.Position.x + room.Size.x - 1; i++)
            {
                tileMap.SetTile(new Vector3Int(i, room.Position.y, 0), tileset.tiles[7]);
                tileMap.SetTile(new Vector3Int(i, room.Position.y + room.Size.y - 1, 0), tileset.tiles[1]);
            }
            for (int i = room.Position.y + 1; i < room.Position.y + room.Size.y - 1; i++)
            {
                tileMap.SetTile(new Vector3Int(room.Position.x, i, 0), tileset.tiles[3]);
                tileMap.SetTile(new Vector3Int(room.Position.x + room.Size.x - 1, i, 0), tileset.tiles[5]);
            }

            tileMap.SetTile(new Vector3Int(room.Position.x, room.Position.y, 0), tileset.tiles[6]);
            tileMap.SetTile(new Vector3Int(room.Position.x, room.Position.y + room.Size.y - 1, 0), tileset.tiles[0]);
            tileMap.SetTile(new Vector3Int(room.Position.x + room.Size.x - 1, room.Position.y, 0), tileset.tiles[8]);
            tileMap.SetTile(new Vector3Int(room.Position.x + room.Size.x - 1, room.Position.y + room.Size.y - 1, 0), tileset.tiles[2]);
        }
    }

    private Vector3Int GetRandomPosFromRoom(Structure structure)
    {
        return new Vector3Int(
                Random.Range(structure.Position.x, structure.Position.x + structure.Size.x),
                Random.Range(structure.Position.y, structure.Position.y + structure.Size.y),
                0);
    }

}
