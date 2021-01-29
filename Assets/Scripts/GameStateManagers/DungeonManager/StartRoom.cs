using MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class StartRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Start;

    public void SpawnItems(Vector3 pos) {
        for (int i = 0; i < PlayersDict.Instance.Players.Count; i++) {
            PickableInWorld.Place(PickableDict.Instance.GetWeapon(1), pos);
        }
    }
}
