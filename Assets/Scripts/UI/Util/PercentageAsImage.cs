using UnityEngine;
using UnityEngine.UI;

public class PercentageAsImage : MonoBehaviour
{
    [SerializeField] private Image percentageImage;

    public void UpdateValue(float value)
    {
        value = Mathf.Clamp(value, 0.0f, 1.0f);
        percentageImage.fillAmount = value;
    }
}
