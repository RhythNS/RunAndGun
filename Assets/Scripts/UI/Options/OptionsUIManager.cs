using UnityEngine;

public class OptionsUIManager : MonoBehaviour
{
    [SerializeField] private NameInput nameInput;
    [SerializeField] private ServerConnect serverConnect;
    [SerializeField] private ServerInfo serverInfo;

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
}
