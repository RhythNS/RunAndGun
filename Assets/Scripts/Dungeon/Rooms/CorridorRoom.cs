using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Corridor;
}
