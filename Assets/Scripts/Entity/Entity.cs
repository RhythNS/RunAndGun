using Mirror;

public abstract class Entity : NetworkBehaviour
{
    public abstract EntityType EntityType { get; }
}
