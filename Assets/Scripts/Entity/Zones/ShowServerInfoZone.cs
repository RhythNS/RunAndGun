public class ShowServerInfoZone : EnterZone
{
    public override void OnEnter(Player player)
    {
        UIManager.Instance.OptionsManager.ShowServerInfo();
    }

    public override void OnExit(Player player)
    {
        UIManager.Instance.OptionsManager.HideServerInfo();
    }
}
