/// <summary>
/// Zone that opens the resolution / fullscreen controls.
/// </summary>
public class ResolutionChangeZone : EnterZone
{
    public override void OnEnter(Player player)
    {
        UIManager.Instance.OptionsManager.ShowResolutionControl();
    }

    public override void OnExit(Player player)
    {
        UIManager.Instance.OptionsManager.HideResolutionControl();
    }
}
