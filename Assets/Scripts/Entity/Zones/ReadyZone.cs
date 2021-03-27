public class ReadyZone : EnterZone
{
    public override void OnEnter(Player player)
    {
        player.StateCommunicator.CmdLobbySetReady(true);
    }

    public override void OnExit(Player player)
    {
        player.StateCommunicator.CmdLobbySetReady(false);
    }
}
