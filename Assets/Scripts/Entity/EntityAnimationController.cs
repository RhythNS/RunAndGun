using Mirror;
using UnityEngine;

public abstract class EntityAnimationController : MonoBehaviour
{
    protected Animator Animator { get; private set; }
    protected Rigidbody2D Body { get; private set; }
    protected EquippedWeapon Weapon { get; private set; }
    protected NetworkAnimator NetworkAnimator { get; private set; }
    public Direction CurDirection { get; private set; } = Direction.Down;
    public event DirectionChanged DirectionChanged;

    private static readonly float DEADZONE = 0.1f;

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Body = GetComponent<Rigidbody2D>();
        Weapon = GetComponent<EquippedWeapon>();
        NetworkAnimator = GetComponent<NetworkAnimator>();
    }

    protected void SetDirection(Vector2 direction)
    {
        Direction? newDir = null;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < -DEADZONE)
                newDir = Direction.Left;
            else if (direction.x > DEADZONE)
                newDir = Direction.Right;
        }
        else
        {
            if (direction.y < -DEADZONE)
                newDir = Direction.Down;
            else if (direction.y > DEADZONE)
                newDir = Direction.Up;
        }

        if (newDir != null && newDir != CurDirection)
            OnDirectionChanged(newDir.Value);
    }

    private void OnDirectionChanged(Direction direction)
    {
        CurDirection = direction;
        DirectionChanged?.Invoke(direction);
        InnerOnDirectionChanged(direction);
    }

    protected abstract void InnerOnDirectionChanged(Direction direction);
}
