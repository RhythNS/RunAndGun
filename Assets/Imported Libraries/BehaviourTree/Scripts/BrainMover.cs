using UnityEngine;

public class BrainMover : MonoBehaviour
{
    /// <summary>
    /// The squared distance to mark a node as "reached".
    /// </summary>
    private static readonly float MAGNITUDE_SQUARED_TO_REACH = 0.15f * 0.15f;

    public enum PathState // in late update
    {
        InProgress, Reached, Unreachable, ConstantDirection
    }

    /// <summary>
    /// The current state of the path.
    /// </summary>
    public PathState State { get => state; 
        private set => state = value; }
    [SerializeField] private PathState state = PathState.Reached;

    /// <summary>
    /// Set to false every tick. If true the agent will go to the set Destination.
    /// </summary>
    public bool ShouldMove { get; set; }
    private bool didMoveLastFrame = false;

    /// <summary>
    /// The body of the object that should be moved.
    /// </summary>
    private Rigidbody2D body;

    [SerializeField] private Vector2 destination;
    public Vector2 Destination
    {
        get => destination;
        set
        {
            if (RoomBounds.Contains(value) == false)
            {
                Vector2 min = RoomBounds.min, max = RoomBounds.max;
                value = MathUtil.VectorClamp(value, min, max);
                /*
                State = PathState.Unreachable;
                return;
                 */
            }

            State = PathState.InProgress;
            destination = value;
            didMoveLastFrame = true;
            ShouldMove = true;
        }
    }

    [SerializeField] private Vector2 constantDirection;
    public Vector2 ConstantDirection
    {
        get => constantDirection;
        set
        {
            State = PathState.ConstantDirection;
            // This should normaly be normalized before saving, though I think that pepole will normalize this value
            // before passing it to here. That is why I am not normalizing it here.
            constantDirection = value;
        }
    }

    public Rect RoomBounds { get; set; }

    public float meterPerSecond;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        didMoveLastFrame = false;
        State = PathState.Reached;
    }

    private void Update()
    {
        switch (State)
        {
            case PathState.InProgress:
                {
                    Vector2 dir = transform.position;
                    dir = Destination - dir;
                    if (dir.sqrMagnitude < MAGNITUDE_SQUARED_TO_REACH)
                    {
                        State = PathState.Reached;
                    }

                    Vector3 vec = dir.normalized * (meterPerSecond * Time.deltaTime);
                    body.MovePosition(transform.position + vec);
                    break;
                }

            case PathState.ConstantDirection:
                {
                    Vector3 vec = ConstantDirection * (meterPerSecond * Time.deltaTime);
                    body.MovePosition(transform.position + vec);
                    break;
                }
        }
    }

    private void LateUpdate()
    {
        // If the should move is different from the last frame then update wheter the AI Agent should move or not
        if (ShouldMove != didMoveLastFrame)
        {
            if (!ShouldMove)
                State = PathState.Reached;

            didMoveLastFrame = ShouldMove;
        }
        ShouldMove = false;
    }
}
