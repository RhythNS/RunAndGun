using Rhyth.BTree;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public BTree tree;

    public BrainMover BrainMover { get; private set; }

    private void Awake()
    {
        BrainMover = GetComponent<BrainMover>();
    }

    private void Start()
    {
        tree = tree.Clone();
        tree.AttachedBrain = this;
        tree.Setup();
    }

    private void Update()
    {
        // if the Tree has failed, successeded or is waiting then restart it
        if (tree.Status != BNode.Status.Running)
        {
            tree.Restart();
            tree.Beginn();
        }
        tree.Update();
    }

}
