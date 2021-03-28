using Mirror;

public class Inventory : NetworkBehaviour
{
    private Player player;

    public SyncList<Item> Items => items;
    private readonly SyncList<Item> items = new SyncList<Item>();

    [SyncVar(hook = nameof(OnMoneyChanged))] public int money;

    public event IntChanged OnMoneyAmountChanged;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    [Server]
    public void PickUp(Item item)
    {
        item.OnPickUp(player);
        items.Add(item);
        player.Stats.OnItemsChanged(items);
    }

    [Command]
    public void CmdDropItem(Item item, bool placeInWorld)
    {
        if (items.Contains(item) == false)
            return;
        item.OnDrop(player);
        items.Remove(item);
        player.Stats.OnItemsChanged(items);

        if (placeInWorld)
            PickableInWorld.Place(item, transform.position);
    }

    private void OnMoneyChanged(int previousMoney, int currentMoney)
    {
        OnMoneyAmountChanged?.Invoke(currentMoney);
    }
}
