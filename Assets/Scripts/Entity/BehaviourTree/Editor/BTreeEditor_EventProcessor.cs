using System;
using UnityEditor;
using UnityEngine;

namespace Rhyth.BTree
{
    public partial class BTreeEditor : EditorWindow
    {
        private enum DragType
        {
            None, Node, Position, NodeConnection
        }

        private struct ConnectionConstructor
        {
            public BNode origin;
            public bool isTop;
            public Vector2 position;

            public ConnectionConstructor(BNode origin, Vector2 position, bool isTop)
            {
                this.origin = origin;
                this.position = position;
                this.isTop = isTop;
            }
        }

        private DragType currentDrag;
        private bool dragged;
        private BNode clickedNode;
        private ConnectionConstructor? connectionConstructor;

        private void ProcessEvents(Event e)
        {
            if (!mapRect.Contains(e.mousePosition)) // if mouse not inside the middle map area
                return;

            bool used = true;
            Vector2 mousePos = (e.mousePosition - offset - mapRect.position
                - new Vector2(mapRect.width / 2, mapRect.height / 2)) / zoomLevel;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0) // left click
                    {
                        dragged = false;

                        currentDrag = DragType.Position;

                        if (!GetNodeFromPosition(mousePos, out BNode node)) // is mouse hovering over box
                            break;

                        // check if the click was on one of the connection boxes
                        if (inPlayMode == false && GetConnection(mousePos, node, out bool isTop) && isTop)
                        {
                            if (node == rootNode)
                            {
                                Debug.LogWarning("Root node cannot have parents! Change the root node first before adding new parents to this node!");
                                break;
                            }

                            currentDrag = DragType.NodeConnection;

                            RemoveConnection(node);

                            Vector3 connectionMiddlePos = isTop ? GetUpperMiddle(node) : GetLowerMiddle(node);

                            connectionConstructor = new ConnectionConstructor(node, connectionMiddlePos, isTop);
                        }
                        else // click was not on the connection boxes
                        {
                            currentDrag = DragType.Node;
                            clickedNode = node;
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0) // left click
                    {
                        switch (currentDrag)
                        {
                            case DragType.None:
                                goto MouseDragFinished; // skip the changed set to true flag
                            case DragType.Node:
                                if (inPlayMode == true)
                                    break;

                                Vector2 toAdd = e.delta;

                                Rect toChange = clickedNode.boundsInEditor;
                                toChange.position += toAdd / zoomLevel;
                                clickedNode.boundsInEditor = toChange;
                                break;
                            case DragType.Position:
                                offset += e.delta;
                                break;
                            case DragType.NodeConnection:
                                Vector2 nodeOffset = offset + new Vector2(mapRect.width / 2, mapRect.height / 2);

                                BNode node = connectionConstructor.Value.origin;
                                Vector2 from = (GetUpperMiddle(node)) + nodeOffset;
                                Vector2 to = from - new Vector2(100, 100);

                                Handles.DrawBezier(from, to, from + Vector2.down * 50f, to - Vector2.down * 50f, Color.black, null, 2f);
                                break;
                        }
                        GUI.changed = true;
                        dragged = true;

                    MouseDragFinished:;
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0) // left click
                    {
                        if (dragged == false)
                        {
                            if (clickedNode != null)
                            {
                                selectedObject = clickedNode;
                            }
                        }
                        else // if dragged == true
                        {
                            if (currentDrag == DragType.Node) // Save the position of the dragged node 
                            {
                                Rect toChange = clickedNode.boundsInEditor;
                                ShapesUtil.RectRoundToNextInt(ref toChange);
                                clickedNode.boundsInEditor = toChange;

                                // The node was moved so the order of the parented children might be wrong
                                if (GetParentNode(clickedNode, out BNode parent))
                                    SortChildren(parent);

                                AssetDatabase.SaveAssets();
                            }
                            else if (currentDrag == DragType.NodeConnection) // try to connect the node to its new parent
                            {
                                if (GetNodeFromPosition(mousePos, out BNode newParent)) // if mouse is over hovering over a node
                                {
                                    if (newParent == connectionConstructor.Value.origin) // if newParent is self
                                        goto NodeConnectionFailed;

                                    // if parent already has the maximum number of children
                                    if (newParent.MaxNumberOfChildren != -1 &&
                                        newParent.Children.Length >= newParent.MaxNumberOfChildren)
                                    {
                                        Debug.LogWarning("Node has already the max number of children!");
                                        goto NodeConnectionFailed;
                                    }

                                    BNode child = connectionConstructor.Value.origin;
                                    if (NodeIsChild(child, newParent)) // If the node is in any way already connected (no loops)
                                    {
                                        Debug.LogWarning("Node could not be connected. Connecting these nodes would result in a loop!");
                                        goto NodeConnectionFailed;
                                    }

                                    // Check if the child node type is allowed on the parent
                                    Type[] allowedTypes = newParent.AllowedChildrenTypes;
                                    if (allowedTypes != null && allowedTypes.Length != 0)
                                    {
                                        bool isAllowed = false;
                                        for (int i = 0; i < allowedTypes.Length; i++)
                                        {
                                            if (child.GetType() == allowedTypes[i] || child.GetType().IsSubclassOf(allowedTypes[i]))
                                            {
                                                isAllowed = true;
                                                break;
                                            }
                                        }
                                        if (isAllowed == false)
                                        {
                                            Debug.LogWarning("The type " + child.GetType() + " is not allowed to be a child of this node.");
                                            goto NodeConnectionFailed;
                                        }
                                    }

                                    // Everything is fine, add the connection and sort the parents children
                                    AddToArray(newParent, child, true);
                                    SortChildren(newParent);

                                NodeConnectionFailed:;
                                }
                            } // end if currentDrag is NodeConnection
                        } // end if dragged
                        dragged = false;
                        currentDrag = DragType.None;
                        clickedNode = null;
                        connectionConstructor = null;
                    }
                    else if (e.button == 1) // right click 
                    {
                        if (inPlayMode == true)
                            break;

                        if (!dragged)
                        {
                            if (GetNodeFromPosition(mousePos, out BNode deleteNode)) // is mouse hovering over node
                            {
                                // Show delete menu for node
                                GenericMenu nodeMenu = new GenericMenu();
                                nodeMenu.AddItem(new GUIContent("Delete"), false, () => RemoveNode(deleteNode));
                                nodeMenu.ShowAsContext();
                            }
                            else // mouse was not over node
                            {
                                // show menu for creating a new node
                                GenericMenu nodeMenu = new GenericMenu();
                                for (int i = 0; i < nodeTypes.Length; i++)
                                {
                                    Type type = nodeTypes[i];
                                    string nodeName = nodeTypes[i].Name;
                                    if (nodeName.EndsWith("node", StringComparison.OrdinalIgnoreCase))
                                        nodeName = nodeName.Substring(0, nodeName.Length - 4);

                                    nodeMenu.AddItem(new GUIContent(nodeName), false, () =>
                                    {
                                        BNode createNode = (BNode)CreateInstance(type);
                                        createNode.name = "New " + createNode.GetType().Name;
                                        createNode.boundsInEditor = new Rect(mousePos, new Vector2(80, 80));
                                        AssetDatabase.AddObjectToAsset(createNode, tree.targetObject);
                                        AssetDatabase.SaveAssets();
                                    });
                                }
                                nodeMenu.ShowAsContext();
                            }
                        }
                    }
                    break;
                case EventType.ScrollWheel:
                    zoomLevel = Mathf.Clamp(zoomLevel - e.delta.y / 50, MIN_ZOOM_LEVEL, MAX_ZOOM_LEVEL);
                    GUI.changed = true;
                    break;
                default:
                    used = false;
                    break;
            }
            if (used) // consume event if used
                e.Use();
        }
    }
}