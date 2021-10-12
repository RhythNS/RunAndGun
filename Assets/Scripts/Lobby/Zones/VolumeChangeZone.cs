/// <summary>
/// Zone that opens the volume control.
/// </summary>
public class VolumeChangeZone : EnterZone
{
    public override void OnEnter(Player player)
    {
        UIManager.Instance.OptionsManager.ShowVolumeControl();
    }

    public override void OnExit(Player player)
    {
        UIManager.Instance.OptionsManager.HideVolumeControl();
    }
}
