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


    public override void OnStartLocalPlayer()
    {
        Config.Instance.selectedPlayerType = characterType;
        RAGInput.AttachInput(gameObject);
    }

    [Command]
    public void CmdValidate()
    {

    }
}
