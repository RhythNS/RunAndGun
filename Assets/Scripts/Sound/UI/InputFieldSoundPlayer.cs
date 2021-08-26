using TMPro;
using UnityEngine;

/// <summary>
/// Plays sounds when the input of a tmp input filed changed.
/// </summary>
public class InputFieldSoundPlayer : MonoBehaviour
{
    private string prevInput;

    private void Start()
    {
        TMP_InputField field = GetComponent<TMP_InputField>();
        field.onValueChanged.AddListener(OnInputChanged);
        field.onSelect.AddListener(OnSelect);
    }

    private void OnSelect(string input)
    {
        prevInput = input;
    }

    private void OnInputChanged(string input)
    {
        UISoundManager.Instance.PlaySound(
            input.Length > prevInput.Length ?
            UISoundManager.Sound.TextboxTyped :
            UISoundManager.Sound.TextboxBackspace);
        prevInput = input;
    }

    private void OnDestroy()
    {
        TMP_InputField field = GetComponent<TMP_InputField>();
        if (!field)
            return;

        field.onValueChanged.RemoveListener(OnInputChanged);
        field.onSelect.RemoveListener(OnSelect);
    }
}
