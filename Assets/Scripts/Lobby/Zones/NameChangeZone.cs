/// <summary>
/// Zone that opens the name changer.
/// </summary>
public class NameChangeZone : EnterZone
{
    public override void OnEnter(Player player)
    {
        UIManager.Instance.OptionsManager.ShowNameInput();
    }

    public override void OnExit(Player player)
    {
        UIManager.Instance.OptionsManager.HideNameInput();
    }
}
