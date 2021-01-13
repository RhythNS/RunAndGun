using UnityEditor;
using UnityEngine;

namespace Rhyth.BTree
{
    public partial class BTreeEditor : EditorWindow
    {
        private bool GetConnection(Vector2 position, BNode target, out bool isTop)
        {
            GetConnectionBoxes(target, out Rect upper, out Rect lower);
            if (upper.Contains(position))
            {
                isTop = true;
                return true;
            }
            else if (lower.Contains(position))
            {
                isTop = false;
                return true;
            }
            else
            {
                isTop = false;
                return false;
            }
        }

        private bool GetNodeFromPosition(Vector2 position, out BNode node)
        {
            for (int i = 0; i < allNodes.Length; i++)
            {
                if (allNodes[i].boundsInEditor.Contains(position))
                {
                    node = allNodes[i];
                    return true;
                }
            }
            node = null;
            return false;
        }

        private void GetConnectionBoxes(BNode target, out Rect upper, out Rect lower)
        {
            Rect nodeBounds = target.boundsInEditor;

            Vector2 connectionPosition = new Vector2(
                       nodeBounds.x + nodeBounds.width / 2 - connectionBox.width / 2,
                       nodeBounds.y + nodeBounds.height - connectionBox.height);
            connectionBox.position = connectionPosition;

            lower = connectionBox;
            lower.position = connectionPosition;

            upper = connectionBox;
            connectionPosition.y = nodeBounds.y;
            connectionBox.position = connectionPosition;
            upper.position = connectionPosition;
        }

        private Vector2 GetNodeMiddle(BNode node)
        {
            Vector3 middle = node.boundsInEditor.position;
            middle.x += node.boundsInEditor.width / 2;
            middle.y += node.boundsInEditor.height / 2;
            return middle;
        }

        private Vector2 GetLowerMiddle(BNode node)
        {
            Vector3 middle = GetNodeMiddle(node);
            middle.y += node.boundsInEditor.height / 2 - connectionBox.height / 2;
            return middle;
        }

        private Vector2 GetUpperMiddle(BNode node)
        {
            Vector3 middle = GetNodeMiddle(node);
            middle.y -= node.boundsInEditor.height / 2 - connectionBox.height / 2;
            return middle;
        }

    }
}