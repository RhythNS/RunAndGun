using UnityEngine;
using UnityEngine.UI;

public class DashUpdater : MonoBehaviour
{
    private static DashUpdater instance;
    private Slider slider;

    private void Awake()
    {
        instance = this;
        slider = GetComponent<Slider>();
    }

    public static void UpdatePercentage(float perc)
    {
        instance.slider.value = perc;
    }

}
