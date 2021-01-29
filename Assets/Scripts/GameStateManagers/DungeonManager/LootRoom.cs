using MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class LootRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => true;

    public override RoomType RoomType => RoomType.Loot;

    public Pickable[] pickables;

    public override void OnAllPlayersEntered() {
        SpawnLoot();
    }

    private void SpawnLoot() {
        List<Vector2Int> lootSpawns = new List<Vector2Int>();

        int maxIterations = pickables.Length * 25;
        int iterations = 0;

        while (lootSpawns.Count < pickables.Length) {
            int rnd = UnityEngine.Random.Range(0, walkableTiles.Count);
            Vector2Int pos = walkableTiles[rnd];

            bool found = false;
            for (int x = -2; x < 2; x++) {
                for (int y = -2; y < 2; y++) {
                    if (lootSpawns.Contains(new Vector2Int(x, y))) {
                        found = true;
                        break;
                    }
                }
            }

            if (!found) {
                PickableInWorld.Place(pickables[lootSpawns.Count], new Vector3(pos.x, pos.y, 0f));

                lootSpawns.Add(pos);
            }

            iterations++;
            if (iterations >= maxIterations)
                break;
        }
    }
}
