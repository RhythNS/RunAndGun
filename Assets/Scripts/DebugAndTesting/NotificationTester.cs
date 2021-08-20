using UnityEngine;

/// <summary>
/// Sends strings in order to notifications.
/// </summary>
public class NotificationTester : MonoBehaviour
{
    [SerializeField] private string[] messages;
    private int atMessage;

    private void Start()
    {
        Debug.Log("Write debug messages to noficiation manager with N");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (++atMessage >= messages.Length)
                atMessage = 0;

            UIManager.Instance.ShowNotification(messages[atMessage]);
        }
    }
}
