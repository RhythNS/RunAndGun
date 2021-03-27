using UnityEngine;

public class OptionsUIManager : MonoBehaviour
{
    [SerializeField] private NameInput nameInput;

    private void Awake()
    {
        nameInput.gameObject.SetActive(false);
    }

    public void ShowNameInput() => nameInput.Show();
}
