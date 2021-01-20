using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapGenerator
{
    class RoomBounds : MonoBehaviour
    {
        public Rect Bounds { get; set; }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(Bounds.xMin, Bounds.yMin, -1f), new Vector3(Bounds.xMin, Bounds.yMax, -1f));
            Gizmos.DrawLine(new Vector3(Bounds.xMin, Bounds.yMin, -1f), new Vector3(Bounds.xMax, Bounds.yMin, -1f));
            Gizmos.DrawLine(new Vector3(Bounds.xMax, Bounds.yMax, -1f), new Vector3(Bounds.xMax, Bounds.yMin, -1f));
            Gizmos.DrawLine(new Vector3(Bounds.xMax, Bounds.yMax, -1f), new Vector3(Bounds.xMin, Bounds.yMax, -1f));
        }
    }
}
