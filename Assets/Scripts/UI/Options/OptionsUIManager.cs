using UnityEngine;

/// <summary>
/// Manager for all option panels.
/// </summary>
public class OptionsUIManager : MonoBehaviour
{
    [SerializeField] private NameInput nameInput;
    [SerializeField] private ServerConnect serverConnect;
    [SerializeField] private ServerInfo serverInfo;
    [SerializeField] private VolumeControl volumeControl;
    [SerializeField] private ResolutionScreen resolutionScreen;
    [SerializeField] private GameModeConfig gameModeConfig;
    [SerializeField] private LostConnectionScreen lostConnectionScreen;

    private void Awake()
    {
        nameInput.gameObject.SetActive(false);
    }

    public void ShowNameInput() => nameInput.Show();
    public void HideNameInput() => nameInput.Hide();

    public void ShowJoinServer() => serverConnect.Show();
    public void HideJoinServer() => serverConnect.Hide();

    public void ShowServerInfo() => serverInfo.Show();
    public void HideServerInfo() => serverInfo.Hide();

    public void ShowVolumeControl() => volumeControl.Show();
    public void HideVolumeControl() => volumeControl.Hide();
    
    public void ShowGameModeConfig() => gameModeConfig.Show();
    public void HideGameModeConfig() => gameModeConfig.Hide();

    public void ShowResolutionControl() => resolutionScreen.Show();
    public void HideResolutionControl() => resolutionScreen.Hide();

    public void ShowLostConnectionScreen() => lostConnectionScreen.Show();
}
