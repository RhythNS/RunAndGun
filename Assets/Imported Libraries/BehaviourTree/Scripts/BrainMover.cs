﻿using UnityEngine;

public class BrainMover : MonoBehaviour
{
    private static readonly float MAGNITUDE_SQUARED_TO_REACH = 0.25f;

    public enum PathState // in late update
    {
        InProgress, Reached, Unreachable, ConstantDirection
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
            if (RoomBounds.Contains(value) == false)
            {
                State = PathState.Unreachable;
                return;
            }

            State = PathState.InProgress;
            destination = value;
            didMoveLastFrame = true;
        }
    }

    private Vector2 constantDirection;
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
                        return;
                    }

                    Vector3 vec = dir.normalized * (meterPerSecond * Time.deltaTime);
                    transform.position += vec;
                    break;
                }

            case PathState.ConstantDirection:
                {
                    Vector3 vec = ConstantDirection * (meterPerSecond * Time.deltaTime);
                    transform.position += vec;
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

    private void FixedUpdate()
    {
    }
}