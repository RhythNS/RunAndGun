using MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LootRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => true;

    public override RoomType RoomType => RoomType.Loot;

    // Item selection

    public override void OnAllPlayersEntered() {
        SpawnLoot();
    }

    private void SpawnLoot() {
        // TODO
    }
}
