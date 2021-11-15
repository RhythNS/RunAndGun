public class PlayerAI : Mirror.NetworkBehaviour
{
    public Brain Brain { get; private set; }
    public BrainMover BrainMover { get; private set; }
    public Player OnPlayer { get; private set; }

    private void Awake()
    {
        Brain = GetComponent<Brain>();
        BrainMover = GetComponent<BrainMover>();
        OnPlayer = GetComponent<Player>();

        Brain.enabled = false;
    }

    public override void OnStartServer()
    {
        Brain.enabled = true;
    }
}
