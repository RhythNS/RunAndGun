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
        private BNode[] allNodesForTypes;

        private Vector2 offset = new Vector2();
        private Rect mapRect;
        private Vector2 scroll1, scroll2;
        private float zoomLevel = 1f;
        private readonly float MIN_ZOOM_LEVEL = 0.5f, MAX_ZOOM_LEVEL = 2f;

        private readonly string NODES_WITH_NO_CHILDREN_STRING = "Leaf Nodes";
        private readonly string NODES_WITH_CHILDREN_STRING = "Decorator Nodes";
        private readonly string NODES_INHERIT_FROM_BOOLNODE_STRING = "Bool Nodes";

        [SerializeField] private GUISkin skin;

        private readonly bool DEBUG = false;

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
            allNodesForTypes = new BNode[nodeTypes.Length];
            for (int i = 0; i < nodeTypes.Length; i++)
                allNodesForTypes[i] = (BNode)CreateInstance(nodeTypes[i]);

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
            GUI.skin = skin;

            if (inPlayMode && EditorApplication.isPlaying == false) // if editor just stopped playing
                tree = null;

            inPlayMode = EditorApplication.isPlaying;

            if (tree == null)
            {
                Reload(true);
            }

            EditorGUILayout.BeginHorizontal();

            float widthOfSides = 250;

            // tree info
            Rect rect = position;
            rect.x = 0;
            rect.y = 0;
            rect.width = widthOfSides;
            rect.height *= 0.5f;

            GUILayout.BeginArea(rect);
            scroll1 = GUILayout.BeginScrollView(scroll1);
            DrawTreeInfo();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            // inspector
            rect.y = rect.height;
            GUILayout.BeginArea(rect);
            scroll2 = GUILayout.BeginScrollView(scroll2);
            DrawInspector();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            // EventProcessor
            ProcessEvents(Event.current);
            tree.Update();

            // node map
            rect.x = widthOfSides;
            rect.y = 0;
            rect.width = position.width - widthOfSides;
            rect.height = position.height;
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