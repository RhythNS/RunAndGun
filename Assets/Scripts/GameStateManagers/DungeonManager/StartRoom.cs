using MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StartRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Start;
}
