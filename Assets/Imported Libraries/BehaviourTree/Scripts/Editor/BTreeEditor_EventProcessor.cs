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

        private class ConnectionConstructor
        {
            public BNode origin;
            public bool isTop;
            public Vector2 position;
            public Vector2 mousePos;

            public ConnectionConstructor(BNode origin, Vector2 position, bool isTop, Vector2 mousePos)
            {
                this.origin = origin;
                this.position = position;
                this.isTop = isTop;
                this.mousePos = mousePos;
            }
        }

        private class NodeMover
        {
            public BNode node;
            public Vector2 clickedPosition;

            public NodeMover(BNode node, Vector2 clickedPosition)
            {
                this.node = node;
                this.clickedPosition = clickedPosition;
            }
        }

        private DragType currentDrag;
        private bool dragged;
        private NodeMover nodeMover;
        private ConnectionConstructor connectionConstructor;

        private void ProcessEvents(Event e)
        {
            if (!mapRect.Contains(e.mousePosition)) // if mouse not inside the middle map area
                return;

            bool used = true;
            Vector2 mousePos = (e.mousePosition - offset - mapRect.position - new Vector2(mapRect.width / 2, mapRect.height / 2)) / zoomLevel;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0) // left click
                    {
                        dragged = false;

                        currentDrag = DragType.Position;

                        if (!GetNodeFromPosition(mousePos, out BNode node)) // is mouse hovering over box
                            break;

                        // check if the click was on one of the connection boxes and the node was not the root node
                        if (inPlayMode == false && GetConnection(mousePos, node, out bool isTop) && isTop && node != rootNode)
                        {
                            currentDrag = DragType.NodeConnection;

                            RemoveConnection(node);

                            Vector3 connectionMiddlePos = isTop ? GetUpperMiddle(node) : GetLowerMiddle(node);

                            connectionConstructor = new ConnectionConstructor(node, connectionMiddlePos, isTop, mousePos);
                        }
                        else // click was not on the connection boxes
                        {
                            currentDrag = DragType.Node;
                            nodeMover = new NodeMover(node, node.boundsInEditor.position - mousePos);
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

                                Rect toChange = nodeMover.node.boundsInEditor;
                                Vector2 newPos = mousePos + nodeMover.clickedPosition;
                                newPos /= SNAPPING_PIXELS;
                                newPos.x = Mathf.Round(newPos.x);
                                newPos.y = Mathf.Round(newPos.y);
                                toChange.position = newPos * SNAPPING_PIXELS;

                                nodeMover.node.boundsInEditor = toChange;
                                break;
                            case DragType.Position:
                                offset += e.delta;
                                break;
                            case DragType.NodeConnection:
                                connectionConstructor.mousePos = mousePos;
                                break;
                        }
                        GUI.changed = true;

                    MouseDragFinished:
                        dragged = true;

                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0) // left click
                    {
                        if (dragged == false)
                        {
                            selectedObject = null;
                            if (nodeMover != null)
                            {
                                selectedObject = nodeMover.node;
                            }
                        }
                        else // if dragged == true
                        {
                            if (currentDrag == DragType.Node) // Save the position of the dragged node 
                            {
                                Rect toChange = nodeMover.node.boundsInEditor;
                                ShapesUtil.RectRoundToNextInt(ref toChange);
                                nodeMover.node.boundsInEditor = toChange;

                                // The node was moved so the order of the parented children might be wrong
                                if (GetParentNode(nodeMover.node, out BNode parent))
                                    SortChildren(parent);

                                AssetDatabase.SaveAssets();
                            }
                            else if (currentDrag == DragType.NodeConnection) // try to connect the node to its new parent
                            {
                                TryConnectNodeFromPosition(mousePos);
                            } // end if currentDrag is NodeConnection
                        } // end if dragged
                        dragged = false;
                        currentDrag = DragType.None;
                        nodeMover = null;
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
                                nodeMenu.AddItem(new GUIContent("Delete"), false, () =>
                                {
                                    RemoveNode(deleteNode);
                                    Reload();
                                });
                                nodeMenu.ShowAsContext();
                            }
                            else // mouse was not over node
                            {
                                // show menu for creating a new node
                                GenericMenu nodeMenu = new GenericMenu();
                                for (int i = 0; i < nodeTypes.Length; i++)
                                {
                                    Type type = nodeTypes[i];

                                    int maxNumberOfChilds = allNodesForTypes[i].MaxNumberOfChildren;

                                    string nodeName = nodeTypes[i].Name;
                                    if (nodeName.EndsWith("node", StringComparison.OrdinalIgnoreCase))
                                        nodeName = nodeName.Substring(0, nodeName.Length - 4);

                                    string prefix;
                                    if (type.IsSubclassOf(typeof(BoolNode)))
                                        prefix = NODES_INHERIT_FROM_BOOLNODE_STRING;
                                    else if (maxNumberOfChilds == -1 || maxNumberOfChilds > 0)
                                        prefix = NODES_WITH_CHILDREN_STRING;
                                    else
                                        prefix = NODES_WITH_NO_CHILDREN_STRING;

                                    nodeMenu.AddItem(new GUIContent(prefix + "/" + nodeName, allNodesForTypes[i].StringToolTip), false, () =>
                                    {
                                        BNode createNode = (BNode)CreateInstance(type);
                                        createNode.name = "New " + createNode.GetType().Name;
                                        createNode.boundsInEditor = new Rect(mousePos, new Vector2(80, 80));
                                        AssetDatabase.AddObjectToAsset(createNode, tree.targetObject);
                                        AssetDatabase.SaveAssets();
                                        Reload();
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

        /// <summary>
        /// Trys to connect the node from where the user dragged the mouse from to where the mouse cursor is currently over.
        /// </summary>
        /// <param name="mousePos">The current mouse position.</param>
        private void TryConnectNodeFromPosition(Vector2 mousePos)
        {
            if (GetNodeFromPosition(mousePos, out BNode newParent)) // if mouse is over hovering over a node
            {
                if (newParent == connectionConstructor.origin) // if newParent is self
                    return;

                // if parent already has the maximum number of children
                if (newParent.MaxNumberOfChildren != -1 &&
                    newParent.Children.Length >= newParent.MaxNumberOfChildren)
                {
                    Debug.LogWarning("Node has already the max number of children!");
                    return;
                }

                BNode child = connectionConstructor.origin;
                if (NodeIsChild(child, newParent)) // If the node is in any way already connected (no loops)
                {
                    Debug.LogWarning("Node could not be connected. Connecting these nodes would result in a loop!");
                    return;
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
                        return;
                    }
                }

                // Everything is fine, add the connection and sort the parents children
                AddToArray(newParent, child, true);
                SortChildren(newParent);
            }
        }
    }
}