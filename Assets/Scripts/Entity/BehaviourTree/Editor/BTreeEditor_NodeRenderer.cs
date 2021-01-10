using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rhyth.BTree
{
    public partial class BTreeEditor : EditorWindow
    {
        private static GUIStyle boxStyle;
        private static GUIStyle connectionStyle;
        private Rect connectionBox = new Rect(0, 0, 10, 10);
        private readonly int BOX_STYLE_FONT_SIZE = 10;
        private Dictionary<BNode.Status, Color> colorForStatus;
        private Color breakPointEnabled = Color.cyan, breakPointDisabled = Color.white;

        private void OnEnable()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle();
                boxStyle.alignment = TextAnchor.MiddleCenter;
                boxStyle.normal.background = Texture2D.whiteTexture;
                boxStyle.wordWrap = true;
                boxStyle.fontSize = BOX_STYLE_FONT_SIZE;
            }
            if (connectionStyle == null)
            {
                connectionStyle = new GUIStyle();
                connectionStyle.alignment = TextAnchor.MiddleCenter;
                connectionStyle.normal.background = Texture2D.grayTexture;
            }
            colorForStatus = new Dictionary<BNode.Status, Color>();
            colorForStatus.Add(BNode.Status.Failure, Color.red);
            colorForStatus.Add(BNode.Status.Running, Color.yellow);
            colorForStatus.Add(BNode.Status.Success, Color.green);
            colorForStatus.Add(BNode.Status.Waiting, Color.white);
        }

        private void DrawMap()
        {
            Vector2 offset = this.offset + new Vector2(mapRect.width / 2, mapRect.height / 2);
            boxStyle.fontSize = (int)(BOX_STYLE_FONT_SIZE * zoomLevel);
            for (int i = 0; i < allNodes.Length; i++)
            {
                // Get and set the position of the node
                Rect toDraw = allNodes[i].boundsInEditor;
                toDraw.position *= zoomLevel;
                toDraw.width *= zoomLevel;
                toDraw.height *= zoomLevel;
                toDraw.position += offset;

                // Draw the node on screen and color it accourding to the status it in
                Color oldColor = GUI.color;
                if (inPlayMode) // if in playmode color by current status
                    GUI.color = colorForStatus[allNodes[i].CurrentStatus];
                else // if in editor mode color by breakpoint
                    GUI.color = allNodes[i].BreakPointEnabled ? breakPointEnabled : breakPointDisabled;

                GUI.Box(toDraw, allNodes[i].StringInEditor + "\n" + allNodes[i].name, boxStyle);
                GUI.color = oldColor;

                // Get the connection boxes (parent/child connection sockets)
                GetConnectionBoxes(allNodes[i], out Rect upper, out Rect lower);

                // Set positions of the 2 connection boxes
                upper.width *= zoomLevel;
                upper.height *= zoomLevel;
                upper.position = (upper.position * zoomLevel) + offset;

                lower.width *= zoomLevel;
                lower.height *= zoomLevel;
                lower.position = (lower.position * zoomLevel) + offset;

                // Draw connection boxes
                GUI.Box(upper, "", connectionStyle);
                GUI.Box(lower, "", connectionStyle);
            }

            // Draw the connections of each nodes with bezier curves
            for (int i = 0; i < allNodes.Length; i++)
            {
                BNode[] children = allNodes[i].Children;

                if (children == null)
                    continue;

                for (int j = 0; j < children.Length; j++)
                {
                    Vector2 from = (GetUpperMiddle(children[j]) * zoomLevel) + offset;
                    Vector2 to = (GetLowerMiddle(allNodes[i]) * zoomLevel) + offset;

                    Handles.DrawBezier(from, to, from + Vector2.down * 50f, to - Vector2.down * 50f, Color.black, null, 2f);
                }
            }
        }


    }
}