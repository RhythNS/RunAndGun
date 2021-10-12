/// <summary>
/// Zone that opens the game mode configurer.
/// </summary>
public class ConfigureGameModeZone : EnterZone
{
    public override void OnEnter(Player player)
    {
        UIManager.Instance.OptionsManager.ShowGameModeConfig();
    }

    public override void OnExit(Player player)
    {
        UIManager.Instance.OptionsManager.HideGameModeConfig();
    }
}
