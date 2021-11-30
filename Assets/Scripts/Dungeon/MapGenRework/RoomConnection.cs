using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.Dungeon.MapGenRework
{
    public class RoomConnection : MonoBehaviour
    {
        public bool isOccupied { get; set; } = false;

        public Vector3 GetDirection()
        {
            return transform.right;
        }
    }
}
