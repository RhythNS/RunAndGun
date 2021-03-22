using UnityEngine;

public class ToggleLoadingScreen : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("ToggleLoadingScreen is enabled with L for enabling and O for disabling");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            GetComponent<LoadingScreenManager>().Show();
        else if (Input.GetKeyDown(KeyCode.O))
            GetComponent<LoadingScreenManager>().Hide();
    }
}
