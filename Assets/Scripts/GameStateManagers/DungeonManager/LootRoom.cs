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
        SpawnLoot(pickables);
        AlreadyCleared = true;
    }
}
