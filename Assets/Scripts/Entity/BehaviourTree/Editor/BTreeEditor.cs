using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rhyth.BTree
{
    // This class is a heavily modified version of the MapEditor from the Modularity project
    public partial class BTreeEditor : EditorWindow
    {
        private SerializedObject tree;
        private string treePath;
        private ScriptableObject selectedObject;

        private bool inPlayMode;
        private Brain lastSelectedBrain;

        private bool lockTreeView;

        private Value[] allValues;
        private BNode[] allNodes;
        private BNode rootNode;
        private Dictionary<Type, CustomBNodeEditor> typeForCustomEditor;

        private Type[] valueTypes, nodeTypes;
        private Vector2 offset = new Vector2();
        private Rect mapRect;
        private float zoomLevel = 1f;
        private readonly float MIN_ZOOM_LEVEL = 0.5f, MAX_ZOOM_LEVEL = 2f;

        private bool debug = false;

        // from https://answers.unity.com/questions/634110/associate-my-custom-asset-with-a-custom-editorwind.html
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            UnityEngine.Object tree = Selection.activeObject;
            if (tree is BTree)
            {
                OpenWindow(tree as BTree);
                return true; //catch open file
            }

            return false; // let unity open the file
        }

        public void OpenSubTree(BTree originalTree, BTree clonedTree)
        {
            if (clonedTree != null)
            {
                tree = new SerializedObject(clonedTree);
                List<BNode> nodeList = new List<BNode>();
                GetChildren(clonedTree.Root, nodeList);
                allNodes = nodeList.ToArray();
            }

            treePath = AssetDatabase.GetAssetPath(originalTree);
            lockTreeView = true;
        }

        private void ReloadAfterRecompile()
        {
            tree = new SerializedObject(AssetDatabase.LoadAssetAtPath<BTree>(treePath));
            valueTypes = GetDerivedTypes(typeof(Value));
            nodeTypes = GetDerivedTypes(typeof(BNode));

            GetCustomEditors();
        }

        public static void OpenWindow(BTree tree)
        {
            BTreeEditor treeEditor = GetWindow<BTreeEditor>();
            if (treeEditor.lockTreeView == true)
            {
                treeEditor = CreateInstance<BTreeEditor>();
                treeEditor.Show();
            }
            treeEditor.titleContent = new GUIContent("Behaviour Editor");
            treeEditor.treePath = AssetDatabase.GetAssetPath(tree);
            treeEditor.tree = null;
        }

        // used to redraw the window even if it is not in focus
        private void Update() => Repaint();

        private void OnGUI()
        {
            if (inPlayMode && EditorApplication.isPlaying == false) // if editor just stopped playing
                tree = null;

            inPlayMode = EditorApplication.isPlaying;

            if (inPlayMode == true)
            {
                SetInPlayModeReferences();
                if (tree == null)
                    return;
            }
            else
            { // Not in play mode
                if (treePath == null || treePath.Length == 0)
                {
                    EditorGUILayout.LabelField("I can not find that tree :(");
                    return;
                }

                if (tree == null)
                    ReloadAfterRecompile();

                SetInEditModeReferences();
            }

            EditorGUILayout.BeginHorizontal();

            float widthOfSides = 250;

            // left side tree info
            Rect rect = position;
            rect.x = 0;
            rect.y = 0;
            rect.width = widthOfSides;
            GUILayout.BeginArea(rect);
            DrawTreeInfo();
            GUILayout.EndArea();

            // EventProcessor
            ProcessEvents(Event.current);
            tree.Update();

            // right side inspector
            rect.x = position.width - rect.width;
            GUILayout.BeginArea(rect);
            DrawInspector();
            GUILayout.EndArea();

            // middle node map
            rect.x = widthOfSides;
            rect.width = position.width - (widthOfSides * 2);
            mapRect = rect;
            GUILayout.BeginArea(rect);
            DrawMap();
            GUILayout.EndArea();

            EditorGUILayout.EndHorizontal();

            tree.ApplyModifiedProperties();

            if (GUI.changed)
                Repaint();
        }

    }
}