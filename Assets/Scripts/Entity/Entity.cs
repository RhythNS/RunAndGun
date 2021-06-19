using Mirror;

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
