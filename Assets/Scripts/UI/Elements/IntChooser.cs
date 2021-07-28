using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntChooser : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TMP_Text intDisplay;

    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;
    [SerializeField] private int intValue;

    public UnityEvent<int> OnValueChanged;

    public int Value
    {
        get => intValue;

        set
        {
            if (value < minValue || value > maxValue || value == intValue)
                return;

            intValue = value;
            OnValueChanged.Invoke(value);
            UpdateDisplay();
        }
    }

    private void OnValidate()
    {
        UpdateDisplay();
    }

    private void Awake()
    {
        leftButton.onClick.AddListener(OnLeftButtonClicked);
        rightButton.onClick.AddListener(OnRightButtonClicked);

        if (intValue < minValue || intValue > maxValue)
            intValue = minValue;

        UpdateDisplay();
    }

    private void OnLeftButtonClicked() => --Value;

    private void OnRightButtonClicked() => ++Value;

    private void UpdateDisplay() => intDisplay.text = intValue.ToString();
}
