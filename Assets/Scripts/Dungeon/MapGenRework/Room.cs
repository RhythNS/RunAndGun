using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Dungeon.MapGenRework
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        private RoomType type;

        [SerializeField]
        private RoomConnection[] connections = new RoomConnection[0];

        [SerializeField]
        private Vector3 minBounds;
        [SerializeField]
        private Vector3 maxBounds;

        [SerializeField]
        private Rect boundingRect;

        public Rect GetBoundingRect()
        {
            return boundingRect;
        }

        public RoomType GetRoomType()
        {
            return type;
        }

        public RoomConnection[] GetConnections()
        {
            return connections;
        }

        public List<RoomConnection> GetUnoccupiedConnections()
        {
            return connections.Where(connection => connection.isOccupied == false).ToList();
        }

        public bool IsInBounds(Rect rect)
        {
            return rect.Overlaps(GetBoundingRectWorld());
        }

        public Rect GetBoundingRectWorld()
        {
            return new Rect(transform.position.x + boundingRect.x, transform.position.y + boundingRect.y, boundingRect.width, boundingRect.height);
        }

        private void OnDrawGizmos()
        {

            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].isOccupied)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawLine(connections[i].transform.position, connections[i].transform.position + connections[i].transform.right);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + new Vector3(boundingRect.center.x, boundingRect.center.y), boundingRect.size);
        }
    }
}
