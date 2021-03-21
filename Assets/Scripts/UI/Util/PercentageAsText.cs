using TMPro;
using UnityEngine;

public class PercentageAsText : MonoBehaviour
{
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private string appendToValue = "%";

    public void UpdateValue(float value)
    {
        value = Mathf.Clamp(value, 0.0f, 1.0f);
        percentageText.text = Mathf.RoundToInt(value * 100).ToString() + appendToValue;
    }
}
