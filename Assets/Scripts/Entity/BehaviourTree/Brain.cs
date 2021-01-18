using Rhyth.BTree;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public BTree tree;

    /// <summary>
    /// Set to false every tick. If true the agent will go to the set Destination.
    /// </summary>
    public bool ShouldMove { get; set; }
    private bool didMoveLastFrame;

    public Vector3 Destination
    {
        get => destination;
        set
        {
            destination = value;
            didMoveLastFrame = true;
        }
    }

    private Vector3 destination;

    private void Start()
    {
        tree = tree.Clone();
        tree.AttachedBrain = this;
        didMoveLastFrame = false;
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

    private void LateUpdate()
    {
        // If the should move is different from the last frame then update wheter the AI Agent should move or not
        if (ShouldMove != didMoveLastFrame)
        {
            // set move to = !ShouldMove;
            didMoveLastFrame = ShouldMove;
        }
        ShouldMove = false;
    }
}
