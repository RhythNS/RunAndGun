using Mirror;

/// <summary>
/// Abstract class of entites that can be in the game.
/// </summary>
public abstract class Entity : NetworkBehaviour
{
    public abstract EntityType EntityType { get; }

    [SyncVar(hook = nameof(OnNameChanged))] public string entityName;

    private void OnNameChanged(string oldName, string newName)
    {
        gameObject.name = newName;
    }

    public override void OnStartClient()
    {
        gameObject.name = entityName;
    }
}
