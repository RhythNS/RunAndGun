using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public CharacterType CharacterType => characterType;
    [SerializeField] private CharacterType characterType;

    public RAGInput Input { get; private set; }
    public Stats Stats { get; private set; }
    public Status Status { get; private set; }
    //public Inventory Inventory { get; private set; }
    //public EquippedWeapon EquippedWeapon { get; private set; }

    private void Awake()
    {
        Stats = GetComponent<Stats>();
        Status = GetComponent<Status>();
    }

    private void Start()
    {
        Init("Some");
    }

    /// <summary>
    /// Called when player is created and assigend to a connection.
    /// </summary>
    /// <param name="name">The username of the connection.</param>
    public void Init(string name)
    {
        gameObject.name = name;

        // Add input method if I am local player.
        if (isLocalPlayer)
            RAGInput.AttachInput(gameObject);
    }

    [Command]
    public void CmdValidate()
    {

    }
}
