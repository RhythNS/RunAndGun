using UnityEngine;

public class BrainMover : MonoBehaviour
{
    private static readonly float MAGNITUDE_SQUARED_TO_REACH = 0.25f;

    public enum PathState // in late update
    {
        InProgress, Reached, NoPath
    }

    public PathState State { get; private set; }

    /// <summary>
    /// Set to false every tick. If true the agent will go to the set Destination.
    /// </summary>
    public bool ShouldMove { get; set; }
    private bool didMoveLastFrame = false;

    private Vector2 destination;
    public Vector2 Destination
    {
        get => destination;
        set
        {
            State = PathState.InProgress;
            destination = value;
            didMoveLastFrame = true;
        }
    }

    public Rect RoomBounds { get; set; }

    public Rigidbody2D Body { get; private set; }

    public float movementForce;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
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

        if (ShouldMove)
        {

        }
    }

    private void FixedUpdate()
    {
        if (State != PathState.InProgress)
            return;

        Vector2 dir = transform.position;
        dir = Destination - dir;
        if (dir.sqrMagnitude <  MAGNITUDE_SQUARED_TO_REACH)
        {

        }

        Body.AddForce(dir.normalized * movementForce);
    }
}
